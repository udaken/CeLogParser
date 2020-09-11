using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogThreadPriority : ICelogInfo
    {
        internal CelogThreadPriority(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogThreadCreate> threads)
        {
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_PRIORITY>();
            ThreadHandle = data.hThread;
            Priority = data.nPriority;
            if (threads.LatestEvent(data.hThread) is { } t)
            {
                t._PrioritiyTerms.Add((timestamp, Priority));
            }
        }

        public CeHandle ThreadHandle { get; }
        public int Priority { get; }

        public override string ToString()
         => $"Handle={ThreadHandle}, Priority={Priority}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
