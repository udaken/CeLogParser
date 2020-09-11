using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogHeapRealloc : ICelogInfo
    {
        internal CelogHeapRealloc(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_HEAP_REALLOC>();
            Handle = data.hHeap;
            Flags = (HeapFlags)data.dwFlags;
            Bytes = data.dwBytes;
            MemOld = new CePtr(data.lpMemOld);
            Mem = new CePtr(data.lpMem);
            Tid = data.dwTID;
            Pid = data.dwPID;
            CallerPID = data.dwCallerPID;
            StackTrace = data.GetStackTrace(buffer.Length).ToList(i => new CePtr(i));
        }

        public CeHandle Handle { get; }
        public HeapFlags Flags { get; }
        public uint Bytes { get; }
        public CePtr MemOld { get; }
        public CePtr Mem { get; }
        public uint Tid { get; }
        public uint Pid { get; }
        public uint CallerPID { get; }
        public IReadOnlyList<CePtr> StackTrace { get; }
        public override string ToString()
            => $"Handle={Handle}, Flags={Flags}, Bytes={Bytes}, MemOld={MemOld}, Mem={Mem}, Tid={Tid:X}, Pid={Pid:X}, CallerPID={CallerPID:X}, StackTrace={string.Join(",", StackTrace)}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}