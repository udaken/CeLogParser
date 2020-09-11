using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogCritSecLeave : ICelogInfo
    {
        internal CelogCritSecLeave(ReadOnlySpan<byte> buffer, System.Collections.Generic.List<CelogThreadCreate> threads)
        {
            ref readonly var data = ref buffer.AsRef<CEL_CRITSEC_LEAVE>();
            Handle = data.hCS;
            OwnerThreadHandle = data.hOwnerThread;

            OwnerThread = threads.LatestEvent(OwnerThreadHandle);
        }

        public CeHandle Handle { get; }
        public CeHandle OwnerThreadHandle { get; }
        public ICelogThreadInfo? OwnerThread { get; }
        public override string ToString()
            => $"CriticalSection Thread:{OwnerThreadHandle} Leave on {Handle} ";
        public IReadOnlyList<CeHandle> ContainsHadles => new[] { Handle };
    }
}
