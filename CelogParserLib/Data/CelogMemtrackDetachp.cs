using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogMemtrackDetachp : ICelogInfo
    {
        internal CelogMemtrackDetachp(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MEMTRACK_DETACHP>();
            ProcessHandle = data._.hProcess;
        }
        public CeHandle ProcessHandle { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;

    }
}