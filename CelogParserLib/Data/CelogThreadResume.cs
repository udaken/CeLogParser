using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogThreadResume : ICelogInfo
    {
        internal CelogThreadResume(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_RESUME>();
            ThreadHandle = data.hThread;
        }

        public CeHandle ThreadHandle { get; }
        
        public override string ToString()
         => $"Handle={ThreadHandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
