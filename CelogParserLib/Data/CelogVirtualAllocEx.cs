using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogVirtualAllocEx : ICelogInfo
    {
        internal CelogVirtualAllocEx(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_VIRTUAL_ALLOC_EX>();
            ProcessHandle = data.hProcess;
            Result = data.dwResult;
            Address = new CePtr(data.dwAddress);
            Size = data.dwSize;
            Type = (CeAllocType)data.dwType;
            Protect = (CeProtectFlags)data.dwProtect;
        }

        public CeHandle ProcessHandle { get; }
        public uint Result { get; }
        public CePtr Address { get; }
        public uint Size { get; }
        public CeAllocType Type { get; }
        public CeProtectFlags Protect { get; }

        public override string ToString()
            => $"Handle={ProcessHandle}, Result={Result}, Address={Address}, Size={Size}, Type={Type}, Protect={Protect}";
    }
    [Flags]
    public enum CeAllocType : uint
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Free = 0x10000,
        Private = 0x20000,
        Mapped = 0x40000,
        Reset = 0x80000,
        TopDown = 0x100000,
        AutoCommit = 0x200000,
        FourMbPages = 0x80000000,

    }
    [Flags]
    public enum CeProtectFlags : uint
    {
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        Guard = 0x100,
        NoCache = 0x200,
        Physical = 0x400,
        WriteCombine = 0x400,
    }
}
