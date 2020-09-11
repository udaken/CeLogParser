using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogSystemInvert : ICelogInfo
    {
        internal CelogSystemInvert(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_SYSTEM_INVERT>();
            this.Threadhandle = data.hThread;
            this.Priority = data.nPriority;
        }

        public CeHandle Threadhandle { get; }
        public int Priority { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ Threadhandle };
    }
}
