using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogSemRelease : ICelogInfo
    {
        public CelogSemRelease(ReadOnlySpan<byte> buffer, List<CelogSemCreate> semaphores)
        {
            ref readonly var data = ref buffer.AsRef<CEL_SEM_RELEASE>();
            SemaphoreHandle = data.hSem;
            ReleaseCount = data.dwReleaseCount;
            PreviousCount = data.dwPreviousCount;

            Origin = semaphores.LatestEvent(SemaphoreHandle);
        }

        public CeHandle SemaphoreHandle { get; }
        public uint ReleaseCount { get; }
        public uint PreviousCount { get; }
        public ICelogSemInfo? Origin { get; }

        public override string ToString()
         => $"Handle={SemaphoreHandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ SemaphoreHandle };
    }
}
