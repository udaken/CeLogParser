using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogThreadSuspend : ICelogInfo
    {
        internal CelogThreadSuspend(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_SUSPEND>();
            ThreadHandle = data.hThread;
        }

        public CeHandle ThreadHandle { get; }

        public override string ToString()
         => $"Handle={ThreadHandle}";

    }
}
