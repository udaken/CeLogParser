using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogModuleFree : ICelogInfo
    {
        internal CelogModuleFree(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogProcessCreate> process)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MODULE_FREE>();
            ModuleHandle = data.hModule;
            ProcessHandle = data.hProcess;

            if (process.LatestEvent(data.hProcess) is { } p)
            {
                Process = p;
            }
        }

        public CeHandle ModuleHandle { get; }
        public CeHandle ProcessHandle { get; }
        public ICelogProcessInfo? Process { get; }

        public override string ToString()
            => $"Handle={ModuleHandle}, Process={ProcessHandle}";
    }
}
