using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogVirtualFreeEx : ICelogInfo
    {
        internal CelogVirtualFreeEx(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_VIRTUAL_FREE_EX>();
            ProcessHandle = data.hProcess;
            Address = data.dwAddress;
            Size = data.dwSize;
            Type = (CeAllocType)data.dwType;
        }

        public CeHandle ProcessHandle { get; }
        public uint Result { get; }
        public uint Address { get; }
        public uint Size { get; }
        public CeAllocType Type { get; }

        public override string ToString()
            => $"Handle={ProcessHandle}, Result={Result}, Address={Address}, Size={Size}, Type={Type}";
    }
}
