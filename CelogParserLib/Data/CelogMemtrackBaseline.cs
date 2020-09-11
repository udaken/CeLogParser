using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogMemtrackBaseline : ICelogInfo
    {
        internal CelogMemtrackBaseline(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MEMTRACK_BASELINE>();
        }
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;

    }
}