using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public interface ICelogModuleInfo : ICelogInfo
    {
        CeHandle ModuleHandle { get; }
        CeHandle ProcessHandle { get; }
        ICelogProcessInfo? Process { get; }
        CePtr Base { get; }
        string Name { get; }
        CelogExtraModuleInfo? ExtraInfo { get; }

        uint VMLen {get;}
    }
    public sealed class CelogModuleLoad : ICelogModuleInfo, IEquatable<CelogModuleLoad>
    {
        internal CelogModuleLoad(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogProcessCreate> process)
        {
            LoadedTime = timestamp;
            ref readonly var data = ref buffer.AsRef<CEL_MODULE_LOAD>();
            ModuleHandle = data.hModule;
            ProcessHandle = data.hProcess;
            Base = new CePtr(data.dwBase);
            Name = data.GetName(buffer.Length).ToNullTerminateString();

            if (process.LatestEvent(data.hProcess) is { } p)
            {
                Process = p;
            }
        }
        public TimeSpan LoadedTime { get; }
        public TimeSpan? FreedTime { get; internal set; }
        public CeHandle ModuleHandle { get; }
        public CeHandle ProcessHandle { get; }
        public ICelogProcessInfo? Process { get; }
        public CePtr Base { get; }
        public string Name { get; }

        public CelogExtraModuleInfo? ExtraInfo { get; internal set; }

        public bool Equals(CelogModuleLoad other) => ModuleHandle == other.ModuleHandle && ProcessHandle == other.ProcessHandle && Base == other.Base;

        public override string ToString()
            => $"Process={ProcessHandle}, Module={ModuleHandle}, Base={Base}, {Name}";

        public uint VMLen => ExtraInfo?.VMLen ?? 0;

    }
}
