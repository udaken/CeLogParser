using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogHeapAlloc : ICelogInfo
    {
        internal CelogHeapAlloc(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_HEAP_ALLOC>();
            Handle = data.hHeap;
            Flags = (HeapFlags)data.dwFlags;
            Bytes = data.dwBytes;
            Mem = new CePtr(data.lpMem);
            Tid = data.dwTID;
            Pid = data.dwPID;
            CallerPID = data.dwCallerPID;
            StackTrace = data.GetStackTrace(buffer.Length).ToArray();
        }

        public CeHandle Handle { get; }
        public HeapFlags Flags { get; }
        public uint Bytes { get; }
        public CePtr Mem { get; }
        public uint Tid { get; }
        public uint Pid { get; }
        public uint CallerPID { get; }
        public IReadOnlyList<uint> StackTrace { get; }

        public override string ToString()
            => $"Handle={Handle}, Bytes={Bytes}, Mem={Mem} Tid={Tid:X}, Pid={Pid:X}, CallerPID={CallerPID:X}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
    [Flags]
    public enum HeapFlags : uint
    {
        NoSerialize = 0x00000001,
        Growable = 0x00000002,
        GenerateExceptions = 0x00000004,
        ZeroMemory = 0x00000008,
        ReallocInPlaceOnly = 0x00000010,
        TailCheckingEnabled = 0x00000020,
        FreeCheckingEnabled = 0x00000040,
        DisableCoalesceOnFree = 0x00000080,
        SharedReadonly = 0x00001000,
        CreateAlign16 = 0x00010000,
        CreateEnableTracing = 0x00020000,
        CreateEnableExecute = 0x00040000,

    }
}