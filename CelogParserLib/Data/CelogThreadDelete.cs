using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogThreadDelete : ICelogInfo
    {
        internal CelogThreadDelete(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogThreadCreate> threads)
        {
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_DELETE>();
            ThreadHandle = data.hThread;

            if (threads.LatestEvent(data.hThread) is { } t)
            {
                t.DeletedAt = timestamp;
                Origin = t;
            }
        }

        public CeHandle ThreadHandle { get; }
        public ICelogThreadInfo? Origin { get; }

        public override string ToString()
         => $"Handle={ThreadHandle}";
    }
}
