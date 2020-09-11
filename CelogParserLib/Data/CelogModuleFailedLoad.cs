using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogModuleFailedLoad : ICelogInfo
    {
        internal unsafe CelogModuleFailedLoad(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MODULE_FAILED_LOAD>();
            ProcessId = data.dwProcessId;
            Error = data.dwError;
            Flags = data.dwFlags;
            fixed (CEL_MODULE_FAILED_LOAD* p = &data)
            {
                Name = new string(p->szName);
            }
        }

        public uint ProcessId { get; }
        public uint Error { get; }
        public uint Flags { get; }
        public string Name { get; }

        public override string ToString()
            => $"ModuleFailedLoad {Name}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
