using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogHeapDestroy : ICelogInfo
    {
        internal CelogHeapDestroy(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_HEAP_DESTROY>();
            Handle = data.hHeap;
            Tid = data.dwTID;
            Pid = data.dwPID;
        }

        public CeHandle Handle { get; }
        public uint Tid { get; }
        public uint Pid { get; }

        public override string ToString()
            => $"Handle={Handle}, Tid={Tid:X}, Pid={Pid:X}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}