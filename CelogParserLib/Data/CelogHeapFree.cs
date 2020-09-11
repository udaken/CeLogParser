using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogHeapFree : ICelogInfo
    {
        internal CelogHeapFree(ReadOnlySpan<byte> buffer)
        {
                        ref readonly var data = ref buffer.AsRef<CEL_HEAP_FREE>();
            Handle = data.hHeap;
            Flags = (HeapFlags)data.dwFlags;
            Mem = new CePtr(data.lpMem);
            Tid = data.dwTID;
            Pid = data.dwPID;
            CallerPID = data.dwCallerPID;
            StackTrace = data.GetStackTrace(buffer.Length).ToArray();
        }

        public CeHandle Handle { get; }
        public HeapFlags Flags { get; }
        public CePtr Mem { get; }
        public uint Tid { get; }
        public uint Pid { get; }
        public uint CallerPID { get; }
        public IReadOnlyList<uint> StackTrace { get; }
        public override string ToString()
            => $"Handle={Handle}, Mem={Mem}, Tid={Tid:X}, Pid={Pid:X}, CallerPID={CallerPID:X}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}