using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogMapfileViewOpenEx : ICelogInfo
    {
        internal CelogMapfileViewOpenEx(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MAPFILE_VIEW_OPEN_EX>();
            Handle = data.hMap;
            ProcessHandle = data.hProcess;
            DesiredAccess = data.dwDesiredAccess;
            FileOffset = ((ulong)data.dwFileOffsetHigh << 32) | data.dwFileOffsetLow;
            Len = data.dwLen;
            BaseAddress = data.lpBaseAddress;
        }

        public CeHandle Handle { get; }
        public CeHandle ProcessHandle { get; }
        public uint DesiredAccess { get; }
        public ulong FileOffset { get; }
        public uint Len { get; }
        public CePtr BaseAddress { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
