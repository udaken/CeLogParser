using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogSystemTlb : ICelogInfo
    {
        internal CelogSystemTlb(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_SYSTEM_TLB>();
            this.Count = data.dwCount;
        }

        public uint Count { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}