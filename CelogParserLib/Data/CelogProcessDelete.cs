using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogProcessDelete : ICelogInfo
    {
        internal CelogProcessDelete(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogProcessCreate> processes)
        {
            ref readonly var data = ref buffer.AsRef<CEL_PROCESS_DELETE>();
            ProcessHandle = data.hProcess;
            if (processes.LatestEvent(ProcessHandle) is { } p)
            {
                Origin = p;
                p.DeletedAt = timestamp;
            }
        }

        public CeHandle ProcessHandle { get; }
        public ICelogProcessInfo? Origin { get; }

        public override string ToString()
            => $"Handle={ProcessHandle}";
    }
}
