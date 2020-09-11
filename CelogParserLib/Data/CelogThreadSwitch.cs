using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogThreadSwitch : ICelogInfo
    {
        private CelogThreadSwitch(CeHandle hThread)
        {
            ThreadHandle = hThread;
        }
        static readonly CelogThreadSwitch SwitchToIdle = new CelogThreadSwitch(default);
        internal static CelogThreadSwitch Create(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_SWITCH>();
            if(data.hThread == default)
            {
                return SwitchToIdle;
            }
            return new CelogThreadSwitch(data.hThread);
        }
        public CeHandle ThreadHandle { get; }

        public override string ToString()
            => $"Handle={ThreadHandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
