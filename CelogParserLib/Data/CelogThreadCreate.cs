using System;
using System.Linq;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public interface ICelogThreadInfo : ICelogKernelObjectInfo
    {
        CePtr StartAddr { get; }
        int Priority { get; }
        ICelogProcessInfo? Process { get; }

        string FriendlyName { get; }
        IReadOnlyList<(TimeSpan, int)> PrioritiyTerms { get; }

        string GetFriendlyName(TimeSpan time);
    }

    public sealed class CelogThreadCreate : ICelogThreadInfo
    {
        public static CelogThreadCreate CpuInIdle { get; } = new CelogThreadCreate();
        private CelogThreadCreate(string name)
        {
            _Name = name;
            LazyGetName = new Lazy<string>(() =>
            {
                if (!string.IsNullOrEmpty(_Name))
                {
                    return _Name;
                }

                if (Process != null)
                {
                    foreach (var m in Process.Modules.Values)
                    {
                        if (m.Base <= StartAddr && StartAddr < (m.Base + m.VMLen))
                        {
                            return $"{m.Name}+0x{StartAddr - m.Base:X}";
                        }
                    }
                }
                return "UNKNOWN THREAD";
            });
        }
        private CelogThreadCreate()
            : this("CPU in Idle")
        {
            ThreadHandle = default;
            ProcessHandle = CelogProcessCreate.UnknownProcess.Handle;
            Process = CelogProcessCreate.UnknownProcess;
            ModuleHandle = CeHandle.InvalidHandleValue;
            IsPsudoObject = true;
            Priority = -1;
        }

        internal CelogThreadCreate(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogProcessCreate> processes)
            : this("")
        {
            CreatedAt = timestamp;
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_CREATE>();
            ThreadHandle = data.hThread;
            ProcessHandle = data.hProcess;
            Process = default;
            ModuleHandle = data.hModule;
            StartAddr = new CePtr(data.dwStartAddr);
            Priority = data.nPriority;
            _PrioritiyTerms.Add((timestamp, Priority));
            _Name = data.GetName(buffer.Length).ToNullTerminateString();

            Process = processes.LatestEvent(ProcessHandle);
        }

        public TimeSpan CreatedAt { get; }
        public TimeSpan? DeletedAt { get; internal set; }
        public TimeSpan? TerminatedAt { get; internal set; }
        public CeHandle ThreadHandle { get; }
        CeHandle ICelogKernelObjectInfo.Handle => ThreadHandle;
        public CeHandle ProcessHandle { get; }
        public CeHandle ModuleHandle { get; }
        public CePtr StartAddr { get; }
        public int Priority { get; }
        public string Name => LazyGetName.Value;
        public ICelogProcessInfo? Process { get; }

        readonly Lazy<string> LazyGetName;
        private readonly string _Name;

        internal readonly List<(TimeSpan Start, int Priority)> _PrioritiyTerms = new List<(TimeSpan, int)>(1);
        IReadOnlyList<(TimeSpan, int)> ICelogThreadInfo.PrioritiyTerms => _PrioritiyTerms;

        public string FriendlyName => IsPsudoObject ? _Name : $"{LazyGetName.Value}(Priority={Priority})";
        public string GetFriendlyName(TimeSpan time)
            => $"{LazyGetName.Value}(Priority={_PrioritiyTerms.LastOrNullable(term => term.Start <= time)?.Priority ?? Priority})";

        public override string ToString()
            => $"Handle={ThreadHandle}, Process={ProcessHandle}, Module={ModuleHandle}, StartAddr={StartAddr}, Priority={Priority}, {Name}";

        public bool IsPsudoObject { get; } = false;
    }
}
