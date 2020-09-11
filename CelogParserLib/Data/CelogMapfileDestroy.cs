using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogMapfileDestroy : ICelogInfo
    {
        internal CelogMapfileDestroy(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MAPFILE_DESTROY>();
            Handle = data.hMap;
        }

        public CeHandle Handle { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ Handle };
    }
}
