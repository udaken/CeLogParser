using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogMutexRelease : ICelogInfo
    {
        internal CelogMutexRelease(ReadOnlySpan<byte> buffer, List<CelogMutexCreate> mutexes)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MUTEX_RELEASE>();
            MutexHandle = data.hMutex;
            if (mutexes.LatestEvent(MutexHandle) is { } m)
            {
                Origin = m;
            }
        }

        public CeHandle MutexHandle { get; }
        public ICelogMutexInfo? Origin { get; }

        public override string ToString()
            => $"Handle={MutexHandle}";
    }
}
