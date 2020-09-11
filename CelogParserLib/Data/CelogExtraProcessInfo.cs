using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    class DefaultModule : ICelogModuleInfo
    {
        readonly CelogExtraProcessInfo _Info;
        public DefaultModule(CelogExtraProcessInfo info, ICelogProcessInfo p)
        {
            Process = p;
            _Info = info;
        }
        public CeHandle ModuleHandle => CeHandle.InvalidHandleValue;

        public CeHandle ProcessHandle => _Info.ProcessHandle;

        public ICelogProcessInfo? Process { get; }

        public CePtr Base => _Info.CodeBase;

        public string Name => Process?.Name ?? "";

        public CelogExtraModuleInfo? ExtraInfo => null;
        public uint VMLen => _Info.VMLen;
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;

    }
    public class CelogExtraProcessInfo : ICelogInfo
    {
        internal CelogExtraProcessInfo(ReadOnlySpan<byte> buffer, List<CelogProcessCreate> processes, ref CelogWarningInfo? warning)
        {
            ref readonly var data = ref buffer.AsRef<CEL_EXTRA_PROCESS_INFO>();
            ProcessHandle = data.hProcess;
            CodeBase = new CePtr(data.dwCodeBase);
            VMLen = data.dwVMLen;
            OID = data.dwOID;
            FullPath = data.GetFullPath(buffer.Length).ToNullTerminateString();

            var p = processes.LatestEvent(ProcessHandle);
            if (p != null)
            {
                if (p.ExtraInfo != null)
                    throw new Exception();
                else
                {
                    p.ExtraInfo = this;
                    p.ModulesInternal.Add(CeHandle.InvalidHandleValue, new DefaultModule(this, p));
                }
            }
            else
                warning = (new CelogWarningInfo($"Process:{ProcessHandle} Not Found."));
        }
        public CeHandle ProcessHandle { get; }
        public CePtr CodeBase { get; }
        public uint VMLen { get; }
        public uint OID { get; }
        public string FullPath { get; }

        public override string ToString()
            => $"Process={ProcessHandle}, CodeBase={CodeBase}, VMLen={VMLen}, {FullPath}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
