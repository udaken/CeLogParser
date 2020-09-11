using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogExtraModuleInfo : ICelogInfo
    {
        internal CelogExtraModuleInfo(ReadOnlySpan<byte> buffer, List<CelogModuleLoad> modules)
        {
            ref readonly var info = ref buffer.AsRef<CEL_EXTRA_MODULE_INFO>();
            ModuleHandle = info.hModule;
            VMLen = info.dwVMLen;
            ModuleFlags = info.dwModuleFlags;
            OID = info.dwOID;
            FullPath = info.GetFullPath(buffer.Length).ToNullTerminateString();

            if (modules.LastOrNull(m => m.ModuleHandle == ModuleHandle) is { } m)
            {
                m.ExtraInfo = this;
                ModuleInfo = m;
            }
        }

        public CeHandle ModuleHandle { get; }
        public uint VMLen { get; }
        public CelogModuleFlag ModuleFlags { get; }
        public uint OID { get; }
        public string FullPath { get; }

        public ICelogModuleInfo? ModuleInfo { get; internal set; }
        public CelogModuleReferences? References { get; internal set; }

        public override string ToString()
            => $"Handle={ModuleHandle}, Flag={ModuleFlags}, VMLen={VMLen}, {FullPath}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
