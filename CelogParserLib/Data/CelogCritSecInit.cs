using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogCritSecInit : ICelogInfo
    {
        internal CelogCritSecInit(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_CRITSEC_INIT>();
            Handle = data.hCS;
        }

        public CeHandle Handle { get; }

        public override string ToString()
            => $"CriticalSection Init on {Handle}";
    }
}
