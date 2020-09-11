using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public interface ICelogProcessInfo : ICelogKernelObjectInfo
    {
        IReadOnlyDictionary<CeHandle, ICelogModuleInfo> Modules { get; }
        CePtr VMBase { get; }
        CelogExtraProcessInfo? ExtraInfo { get; }
    }
    public sealed class CelogProcessCreate : ICelogProcessInfo
    {
        internal static readonly ICelogProcessInfo UnknownProcess = new CelogProcessCreate();
        private CelogProcessCreate()
        {
            ProcessHandle = default;
            Name = "Unknown Process";
            IsPsudoObject = true;
        }

        internal CelogProcessCreate(ReadOnlySpan<byte> buffer, TimeSpan timestamp)
        {
            ref readonly var data = ref buffer.AsRef<CEL_PROCESS_CREATE>();
            ProcessHandle = data.hProcess;
            Name = data.GetName(buffer.Length).ToNullTerminateString();
            VMBase = new CePtr(data.dwVMBase);
            ExtraInfo = default;
            CreatedAt = timestamp;
        }

        public TimeSpan CreatedAt { get; }
        public TimeSpan? DeletedAt { get; internal set; }
        public TimeSpan? TerminatedAt { get; internal set; }
        public CeHandle ProcessHandle { get; }
        CeHandle ICelogKernelObjectInfo.Handle => ProcessHandle;
        public string Name { get; }
        public CePtr VMBase { get; }
        public CelogExtraProcessInfo? ExtraInfo { get; internal set; }

        public override string ToString()
            => $"Handle={ProcessHandle}, VMBase={VMBase}, {Name}";

        public IReadOnlyDictionary<CeHandle, ICelogModuleInfo> Modules => ModulesInternal;
        internal Dictionary<CeHandle, ICelogModuleInfo> ModulesInternal = new Dictionary<CeHandle, ICelogModuleInfo>();

        public bool IsPsudoObject { get; } = false;
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ ProcessHandle };
    }
}
