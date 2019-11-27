using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogDataLoss : ICelogInfo
    {
        internal CelogDataLoss(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_DATA_LOSS>();
            Bytes = data.dwBytes;
        }

        public uint Bytes{get;}

        public override string ToString()
            => $"{Bytes} bytes lost.";
    }
}