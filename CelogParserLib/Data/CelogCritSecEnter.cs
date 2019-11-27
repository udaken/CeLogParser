using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogCritSecEnter : ICelogInfo
    {
        internal CelogCritSecEnter(ReadOnlySpan<byte> buffer, System.Collections.Generic.List<CelogThreadCreate> kObjects)
        {
            ref readonly var data = ref buffer.AsRef<CEL_CRITSEC_ENTER>();
            Handle = data.hCS;
            OwnerThreadHandle = data.hOwnerThread;

            OwnerThread = kObjects.LatestEvent(OwnerThreadHandle);
        }

        public CeHandle Handle { get; }
        public CeHandle OwnerThreadHandle { get; }
        public ICelogThreadInfo? OwnerThread { get; }

        public override string ToString()
            => $"CriticalSection Thread:{OwnerThreadHandle} Enter on {Handle} ";
    }
}
