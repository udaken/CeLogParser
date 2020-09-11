using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogLowmemSignalled : ICelogInfo
    {
        internal CelogLowmemSignalled(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_LOWMEM_DATA>();
            PageFreeCount = data.pageFreeCount;
            Need = data.cpNeed;
            LowThreshold = data.cpLowThreshold;
            CriticalThreshold = data.cpCriticalThreshold;
            LowBlockSize = data.cpLowBlockSize;
            CriticalBlockSize = data.cpCriticalBlockSize;
        }

        public int PageFreeCount { get; }
        public int Need { get; }
        public int LowThreshold { get; }
        public int CriticalThreshold { get; }
        public int LowBlockSize { get; }
        public int CriticalBlockSize { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}