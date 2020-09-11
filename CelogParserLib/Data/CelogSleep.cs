using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogSleep : ICelogInfo
    {
        internal CelogSleep(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_SLEEP>();
            this.Timeout = data.dwTimeout;
        }

        public uint Timeout { get; }
        public override string ToString()
            => $"Timeout={Timeout}ms";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}