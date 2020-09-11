using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogMutexDelete : ICelogInfo
    {
        internal CelogMutexDelete(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogMutexCreate> mutexes)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MUTEX_DELETE>();
            MutexHandle = data.hMutex;
            if (mutexes.LatestEvent(MutexHandle) is { } m)
            {
                Origin = m;
                m.DeletedAt = timestamp;
            }
        }

        public CeHandle MutexHandle { get; }
        public ICelogMutexInfo? Origin { get; }
        
        public override string ToString()
            => $"Handle={MutexHandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ MutexHandle };
    }
}
