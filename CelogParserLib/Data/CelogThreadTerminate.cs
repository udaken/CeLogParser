using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogThreadTerminate : ICelogInfo
    {
        internal CelogThreadTerminate(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogThreadCreate> threads)
        {
                        ref readonly var data = ref buffer.AsRef<CEL_THREAD_TERMINATE>();
            ThreadHandle = data.hThread;

            if (threads.LatestEvent(data.hThread) is { } t)
            {
                t.TerminatedAt = timestamp;
                Origin = t;
            }
            else
            {
                // TODO
            }
        }

        public CeHandle ThreadHandle { get; }

        public ICelogThreadInfo? Origin { get; }

        public override string ToString()
         => $"Handle={ThreadHandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ ThreadHandle };

    }
}
