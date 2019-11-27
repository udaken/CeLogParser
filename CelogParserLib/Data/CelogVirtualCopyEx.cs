using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogVirtualCopyEx : ICelogInfo
    {
        internal CelogVirtualCopyEx(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_VIRTUAL_COPY_EX>();
            DestProcessHandle = data.hDestProc;
            Dest = new CePtr(data.dwDest);
            SrcProcessHandle = data.hSrcProc;
            Source = new CePtr(data.dwSource);
            Size = data.dwSize;
            Protect = (CeProtectFlags)data.dwProtect;
        }

        public CeHandle DestProcessHandle { get; }
        public CePtr Dest { get; }
        public CeHandle SrcProcessHandle { get; }
        public CePtr Source { get; }
        public uint Size { get; }
        public CeProtectFlags Protect { get; }

        public override string ToString()
            => $"DestProcessHandle={DestProcessHandle}, Dest={Dest}, SrcProcessHandle={SrcProcessHandle}, Source={Source}, Size={Size}, Protect={Protect}";
    }
}
