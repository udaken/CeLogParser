using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogProcessTerminate : ICelogInfo
    {
        internal CelogProcessTerminate(ReadOnlySpan<byte> buffer, TimeSpan timestamp, System.Collections.Generic.List<CelogProcessCreate> processes)
        {
            ref readonly var data = ref buffer.AsRef<CEL_PROCESS_TERMINATE>();
            ProcessHandle = data.hProcess;

            if (processes.LatestEvent(ProcessHandle) is { } p)
            {
                Origin = p;
                p.TerminatedAt = timestamp;
            }
        }

        public CeHandle ProcessHandle { get; }
        public ICelogProcessInfo? Origin { get; }

        public override string ToString()
            => $"Handle={ProcessHandle}";
    }
}
