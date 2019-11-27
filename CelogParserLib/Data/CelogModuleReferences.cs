using System;
using System.Linq;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
using System.Diagnostics;

namespace CelogParserLib.Data
{
    public sealed class CelogModuleReferences : ICelogInfo
    {
        internal CelogModuleReferences(ReadOnlySpan<byte> buffer, List<CelogModuleLoad> modules, List<CelogProcessCreate> processes)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MODULE_REFERENCES>();
            ModuleHandle = data.hModule;
            ProcessRefCounts = data.GetRefs(buffer.Length).ToList(item => (item.hProcess, item.dwRefCount));

            System.Diagnostics.Debug.Assert(ProcessRefCounts.Count > 0);

            if (modules.LastOrNull(m => m.ModuleHandle == ModuleHandle) is { } m)
            {
                if (m.ExtraInfo == null)
                {
                    return;
                }
                m.ExtraInfo.References = this;

                if (m.ExtraInfo.ModuleInfo != null)
                {
                    Debug.Assert(m.ExtraInfo.ModuleInfo.ProcessHandle.IsInvalid);
                    
                    foreach (var (hProcess, _) in ProcessRefCounts)
                    {
                        if (processes.LatestEvent(hProcess) is { } p)
                        {
                            p.ModulesInternal[ModuleHandle] = m.ExtraInfo.ModuleInfo;
                        }
                    }
                }
            }

        }

        public CeHandle ModuleHandle { get; }

        public IReadOnlyList<(CeHandle ProcessHandle, uint RefCount)> ProcessRefCounts { get; }

        public override string ToString()
            => $"Handle={ModuleHandle}, {string.Join(",", ProcessRefCounts)}";

    }
}
