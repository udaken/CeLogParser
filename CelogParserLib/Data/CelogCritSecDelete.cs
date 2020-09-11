using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogCritSecDelete : ICelogInfo
    {
        internal CelogCritSecDelete(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_CRITSEC_DELETE>();
            Handle = data.hCS;
        }

        public CeHandle Handle { get; }
        public override string ToString()
            => $"CriticalSection Delete on {Handle}";
        public IReadOnlyList<CeHandle> ContainsHadles => new[] { Handle };
    }
}