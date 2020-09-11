using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogSemDelete : ICelogInfo
    {
        public CelogSemDelete(ReadOnlySpan<byte> buffer, TimeSpan timestamp, System.Collections.Generic.List<CelogSemCreate> semaphores)
        {
            ref readonly var data = ref buffer.AsRef<CEL_SEM_DELETE>();
            SemaphoreHandle = data.hSem;

            if (semaphores.LatestEvent(SemaphoreHandle) is { } s)
            {
                s.DeleteAt = timestamp;
                Origin = s;
            }
        }

        public CeHandle SemaphoreHandle { get; }
        public ICelogSemInfo? Origin { get; }

        public override string ToString()
         => $"Handle={SemaphoreHandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ SemaphoreHandle };
    }
}
