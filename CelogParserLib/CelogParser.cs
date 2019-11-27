using CelogParserLib;
using CelogParserLib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
using CelogParserLib.Functional;
using System.Diagnostics;

namespace CelogParserLib
{
    public partial class CelogParser
    {
        readonly byte[] _Buffer;
        readonly int _Length;
        readonly Dictionary<ushort, CelogEventHandler> _Handlers = new Dictionary<ushort, CelogEventHandler>();
        public Encoding Encoding { get; set; } = Encoding.Default;

        public CelogParser(Stream input)
        {
            var stream = input ?? throw new ArgumentNullException(nameof(input));
            if (!stream.CanRead) throw new ArgumentException();
            _Length = (int)(stream.Length - stream.Position);
            _Buffer = new byte[stream.Length - stream.Position];
            stream.Read(_Buffer, 0, _Length);
        }

        public CelogParser(String path)
        {
            _Buffer = File.ReadAllBytes(path);
            _Length = _Buffer.Length;
        }

        public void RegisterHandler(ushort id, CelogEventHandler handler)
        {
            if (id > CELID_MAX || CELID_USER > id)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            else
            {
                _Handlers.Add(id, handler ?? throw new ArgumentNullException(nameof(handler)));
            }
        }

        struct ProgressDispatcher
        {
            int _LastProgressReported;
            int _Progress;
            readonly int _Length;

            public ProgressDispatcher(int length)
            {
                _Length = length;
                _LastProgressReported = Environment.TickCount;
                _Progress = 0;
            }

            public void Dispatch(int pos, EventHandler<int>? progressCallback)
            {
                // progress
                if (_Progress != (pos * 100 / _Length) && (Environment.TickCount - _LastProgressReported) > 100)
                {
                    _Progress = (pos * 100) / _Length;
                    progressCallback?.Invoke(this, _Progress);
                    _LastProgressReported = Environment.TickCount;
                }
            }
        }


        //https://msdn.microsoft.com/ja-jp/library/dd229307.aspx
        public Celog CeLogToObject(EventHandler<int>? progressCallback = null)
        {
            using var stream = new BinaryReader(new MemoryStream(_Buffer, 0, _Length, writable: false), Encoding);

            //Debug.WriteLine("[CeLog] Start parse.(_Length={0})", _Length);
            var timeline = new List<CelogData>(capacity: _Length / 12); // (header, timestamp, payload) * 4 bytes
            var threads = new List<CelogThreadCreate>()
            {
                CelogThreadCreate.CpuInIdle,
            };
            var processes = new List<CelogProcessCreate>();
            var events = new List<CelogEventCreate>();
            var mutexes = new List<CelogMutexCreate>();
            var semaphores = new List<CelogSemCreate>();
            var modules = new List<CelogModuleLoad>();
            var warnings = new List<CelogWarningInfo>();
            Span<byte> buf = stackalloc byte[(int)CEL_HEADER_LENGTH_MASK + 1];
            var freq = 1193180U;
            var defaultQuantum = 100U;
            CelogThreadCreate? currentThread = null;
            var progress = new ProgressDispatcher(_Length);
            var lasttimestamp = TimeSpan.Zero;
            //TryProcessLogMarker(buffer, ref freq, ref defaultQuantum, out var numCPUs, out var isV3);

            while (stream.PeekChar() != -1)
            {
                var header = new CEL_HEADER_V3(stream.ReadUInt32());
                //Debug.WriteLine("[CeLog] Header Readed {0}", header.ToString());

                var timestamp = header.fTimeStamp ?
                    TimeSpan.FromTicks((long)(stream.ReadUInt32() * (10_000_000UL) / freq)) : TimeSpan.Zero;

                if (header.fFileTime)
                {
                    var _ = stream.ReadFileTime();
                    // TODO
                }

                if (!stream.BaseStream.Position.TryConvertToInt32(out var pos))
                    throw new IndexOutOfRangeException();

                //fixup
                var readLength = (header.Length + 3) & ~3;

                //Debug.WriteLine("[CeLog] Data Read {0} Bytes, Position {1}", readLength, pos);

                var bufInfFile = buf.Slice(0, readLength);

                if (stream.Read(bufInfFile) < bufInfFile.Length)
                    break;

                var slice = bufInfFile.Slice(0, header.Length);
                object? result = ProcessLogData(timestamp, header.ID, slice, processes, threads, events, semaphores, mutexes, modules, out var warning, ref freq, ref defaultQuantum);
                System.Diagnostics.Debug.Assert(header.ID == CELID_LOG_MARKER || result != null || slice.IsEmpty);

                if (warning != null)
                {
                    warnings.Add(warning);
                }

                if (timestamp != TimeSpan.Zero)
                {
                    var consumedTime = (lasttimestamp != default ? timestamp.Ticks - lasttimestamp.Ticks : 0) / 10;
                    var item = new CelogData(timestamp, (CelogId)header.ID, header.CPU, new ArraySegment<byte>(_Buffer, pos, header.Length), currentThread, result, consumedTime);
                    timeline.Add(item);
                    lasttimestamp = timestamp;
                }

                if (result is CelogThreadSwitch switchEvent)
                {
                    currentThread = switchEvent.ThreadHandle == CelogThreadCreate.CpuInIdle.ThreadHandle ?
                                    CelogThreadCreate.CpuInIdle :
                                    threads.LatestEvent(switchEvent.ThreadHandle);
                }

                progress.Dispatch(pos, progressCallback);
            }

            //events = events.SkipWhile(i => (CelogId.ProcessCreate, CelogId.ThreadCreate, CelogId.ExtraProcessInfo).Contains(i.Id)).ToList(); //TODO
            return new Celog(threads, processes, events, mutexes, semaphores, timeline, warnings, defaultQuantum);
        }

        static bool TryProcessLogMarker(ReadOnlySpan<byte> buffer,
                                     ref uint freq,
                                     ref uint defaultQuantum,
                                     out uint numCPUs,
                                     out bool isV3)
        {
            numCPUs = 1;
            isV3 = false;
            if (buffer.TryReadUInt32(out var value))
            {
                if (new CEL_HEADER(value).ID == CELID_LOG_MARKER)
                {
                    var slice = buffer.Slice(4);
                    ProcessLogMarker(slice, ref freq, ref defaultQuantum, out numCPUs, out isV3);
                    return true;
                }
            }
            defaultQuantum = 100;
            freq = 1;
            return false;
        }

        static object? ProcessLogMarker(ReadOnlySpan<byte> slice,
                                     ref uint freq,
                                     ref uint defaultQuantum,
                                     out uint numCPUs,
                                     out bool isV3)
        {
            numCPUs = 1;
            isV3 = false;
            if (slice.TryRead<CEL_LOG_MARKER>(out var marker))
            {
                if (marker.dwVersion == 3 && slice.TryRead<CEL_LOG_MARKER_V3>(out var marker3))
                {
                    freq = marker3.dwFrequency;
                    defaultQuantum = marker3.dwDefaultQuantum;
                    numCPUs = marker3.dwNumCPUs;
                    isV3 = true;
                }
                else
                {
                    freq = marker.dwFrequency;
                    defaultQuantum = marker.dwDefaultQuantum;
                }
            }

            return null;
        }

        private object? ProcessLogData(TimeSpan timestamp,
                                       ushort id,
                                       ReadOnlySpan<byte> buffer,
                                       List<CelogProcessCreate> processes,
                                       List<CelogThreadCreate> threads,
                                       List<CelogEventCreate> events,
                                       List<CelogSemCreate> semaphores,
                                       List<CelogMutexCreate> mutex,
                                       List<CelogModuleLoad> modules,
                                       out CelogWarningInfo? warning,
                                       ref uint freq,
                                       ref uint defaultQuantum)
        {
            warning = null;
            return id switch
            {
                CELID_CS_INIT => new CelogCritSecInit(buffer),
                CELID_CS_DELETE => new CelogCritSecDelete(buffer),
                CELID_CS_ENTER => new CelogCritSecEnter(buffer, threads),
                CELID_CS_LEAVE => new CelogCritSecLeave(buffer, threads),
                CELID_EVENT_CREATE => new CelogEventCreate(buffer, timestamp).Chain(item => { events.Add(item); }),
                CELID_EVENT_SET => new CelogEventSet(buffer, events),
                CELID_EVENT_RESET => new CelogEventReset(buffer, events),
                CELID_EVENT_PULSE => new CelogEventPulse(buffer, events),
                CELID_EVENT_DELETE => new CelogEventDelete(buffer, timestamp, events),
                CELID_WAIT_MULTI => new CelogWaitMulti(buffer),
                CELID_SLEEP => new CelogSleep(buffer),
                CELID_SEM_CREATE => new CelogSemCreate(buffer, timestamp).Chain(item => semaphores.Add(item)),
                CELID_SEM_RELEASE => new CelogSemRelease(buffer, semaphores),
                CELID_SEM_DELETE => new CelogSemDelete(buffer, timestamp, semaphores),
                CELID_HEAP_CREATE => new CelogHeapCreate(buffer, timestamp),
                CELID_HEAP_ALLOC => new CelogHeapAlloc(buffer),
                CELID_HEAP_REALLOC => new CelogHeapRealloc(buffer),
                CELID_HEAP_FREE => new CelogHeapFree(buffer),
                CELID_HEAP_DESTROY => new CelogHeapDestroy(buffer),
                CELID_VIRTUAL_ALLOC_EX => new CelogVirtualAllocEx(buffer),
                CELID_VIRTUAL_COPY_EX => new CelogVirtualCopyEx(buffer),
                CELID_VIRTUAL_FREE_EX => new CelogVirtualFreeEx(buffer),
                CELID_SYSTEM_PAGE_IN => new CelogSystemPageIn(buffer, processes),
                CELID_SYSTEM_PAGE_OUT => null,
                CELID_MAPFILE_CREATE_EX => new CelogMapfileCreateEx(buffer),
                CELID_MAPFILE_DESTROY => new CelogMapfileDestroy(buffer),
                CELID_MAPFILE_VIEW_OPEN_EX => new CelogMapfileViewOpenEx(buffer),
                CELID_MAPFILE_VIEW_CLOSE => new CelogMapfileViewClose(buffer),
                CELID_MAPFILE_VIEW_FLUSH => new CelogMapfileViewFlush(buffer),
                CELID_CACHE_ALLOCVIEW => null,
                CELID_CACHE_FREEVIEW => null,
                CELID_CACHE_FLUSHVIEW => null,
                CELID_THREAD_SWITCH => CelogThreadSwitch.Create(buffer),
                CELID_THREAD_MIGRATE => new CelogThreadMigrate(buffer),
                CELID_THREAD_CREATE => new CelogThreadCreate(buffer, timestamp, processes).Chain(item => threads.Add(item)),
                CELID_THREAD_TERMINATE => new CelogThreadTerminate(buffer, timestamp, threads),
                CELID_THREAD_DELETE => new CelogThreadDelete(buffer, timestamp, threads),
                CELID_PROCESS_CREATE => new CelogProcessCreate(buffer, timestamp).Chain(item => processes.Add(item)),
                CELID_PROCESS_TERMINATE => new CelogProcessTerminate(buffer, timestamp, processes),
                CELID_PROCESS_DELETE => new CelogProcessDelete(buffer, timestamp, processes),
                CELID_THREAD_SUSPEND => new CelogThreadSuspend(buffer),
                CELID_THREAD_RESUME => new CelogThreadResume(buffer),
                CELID_THREAD_QUANTUMEXPIRE => null,
                CELID_EXTRA_PROCESS_INFO => new CelogExtraProcessInfo(buffer, processes, ref warning),
                CELID_SYSTEM_INVERT => new CelogSystemInvert(buffer),
                CELID_THREAD_PRIORITY => new CelogThreadPriority(buffer, timestamp, threads),
                CELID_THREAD_QUANTUM => new CelogThreadQuantum(buffer),
                CELID_MODULE_LOAD => new CelogModuleLoad(buffer, timestamp, processes).Chain(item => modules.Add(item)),
                CELID_MODULE_FREE => new CelogModuleFree(buffer, timestamp, processes),
                CELID_MODULE_FAILED_LOAD => new CelogModuleFailedLoad(buffer),
                CELID_EXTRA_MODULE_INFO => new CelogExtraModuleInfo(buffer, modules),
                CELID_MODULE_REFERENCES => new CelogModuleReferences(buffer, modules, processes),
                CELID_MUTEX_CREATE => new CelogMutexCreate(buffer, timestamp).Chain(item => mutex.Add(item)),
                CELID_MUTEX_RELEASE => new CelogMutexRelease(buffer, mutex),
                CELID_MUTEX_DELETE => new CelogMutexDelete(buffer, timestamp, mutex),
                CELID_RAW_LONG => MemoryMarshal.Cast<byte, int>(buffer).ToArray(),
                CELID_RAW_ULONG => MemoryMarshal.Cast<byte, uint>(buffer).ToArray(),
                CELID_RAW_SHORT => MemoryMarshal.Cast<byte, short>(buffer).ToArray(),
                CELID_RAW_USHORT => MemoryMarshal.Cast<byte, ushort>(buffer).ToArray(),
                CELID_RAW_WCHAR => MemoryMarshal.Cast<byte, char>(buffer).ToNullTerminateString(),
                CELID_RAW_CHAR => Encoding.GetString(buffer.Slice(buffer.GetAnsiStringLength())),
                CELID_RAW_UCHAR => buffer.ToArray(),
                CELID_RAW_FLOAT => MemoryMarshal.Cast<byte, float>(buffer).ToArray(),
                CELID_RAW_DOUBLE => MemoryMarshal.Cast<byte, double>(buffer).ToArray(),
                CELID_SYSTEM_TLB => new CelogSystemTlb(buffer),
                CELID_INTERRUPTS => new CelogInterrupts(buffer),
                CELID_KCALL_ENTER => MemoryMarshal.Cast<byte, int>(buffer)[0],// single int
                CELID_KCALL_LEAVE => MemoryMarshal.Cast<byte, int>(buffer)[0],// single int
                CELID_FLAGGED => null,
                CELID_CALLSTACK => null,
                CELID_CALLSTACK_EX => MemoryMarshal.Cast<byte, uint>(buffer).ToArray(),
                CELID_CEPERF => throw new NotImplementedException(),
                CELID_TIMER_START => MemoryMarshal.Cast<byte, char>(buffer).ToString(),
                CELID_TIMER_STOP => MemoryMarshal.Cast<byte, char>(buffer).ToString(),
                CELID_LOWMEM_SIGNALLED => new CelogLowmemSignalled(buffer),
                CELID_MEMTRACK_DETACHP => new CelogMemtrackDetachp(buffer),
                CELID_MEMTRACK_BASELINE => new CelogMemtrackBaseline(buffer),
                CELID_BOOT_TIME => new CelogBootTime(buffer),
                CELID_GDI => new CelogGdiInfo(buffer),
                CELID_RDP => new CelogRdpInfo(buffer),
                CELID_PROFILER_START => null,
                CELID_PROFILER_STOP => null,
                CELID_PROFILER_MARK => null,
                CELID_PROFILER_NAME => null,
                CELID_MONTECARLO_HIT => null,
                CELID_OEMPROFILER_HIT => null,
                CELID_APP_PROFILER_HIT => null,
                CELID_CAP_OVERHEAD => null,
                CELID_CALLCAP_ENTER => null,
                CELID_CALLCAP_EXIT => null,
                CELID_FASTCAP_ENTER => null,
                CELID_FASTCAP_EXIT => null,
                CELID_DEBUG_MSG => new CelogDebugMsg(buffer),
                CELID_DEBUG_TRAP => new CelogDebugTrap(buffer),
                CELID_EVENTLOG_CHANNEL_MSG => new CelogEventlogChannelMsg(buffer),
                CELID_EVENTLOG_WPP_MSG => new CelogEventlogWppMsg(buffer),
                CELID_DATA_LOSS => new CelogDataLoss(buffer),
                CELID_SYNC_END => null,
                CELID_LOG_MARKER => ProcessLogMarker(buffer, ref freq, ref defaultQuantum, out var _, out var _),
                _ when (CELID_GWES <= id && id <= CELID_GWES_MAX) => buffer.ToArray(),
                _ when (_Handlers.TryGetValue(id, out var handler)) => handler(id, buffer, new CelogEventHandlerContext(threads)),
                _ => ProcessUnknownLogId(id, buffer, out warning),
            };
        }
        static byte[] ProcessUnknownLogId(ushort id, ReadOnlySpan<byte> buffer, out CelogWarningInfo? warning)
        {
            warning = (new CelogWarningInfo($"Not Supported Celog ID:{id}"));
            return buffer.ToArray();
        }

    }
}
