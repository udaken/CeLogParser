using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogMapfileViewClose : ICelogInfo
    {
        internal CelogMapfileViewClose(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MAPFILE_VIEW_CLOSE>();
            ProcessHandle = data.hProcess;
            BaseAddress = data.lpBaseAddress;

        }

        public CeHandle ProcessHandle { get; }
        public CePtr BaseAddress { get; }
    }
}
