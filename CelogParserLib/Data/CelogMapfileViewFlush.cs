using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogMapfileViewFlush : ICelogInfo
    {
        internal CelogMapfileViewFlush(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MAPFILE_VIEW_FLUSH>();
            ProcessHandle = data.hProcess;
            BaseAddress = data.lpBaseAddress;
            Len = data.dwLen;
            NumPages = data.dwNumPages;
            FlushType = (CelogMapFlushType)(data.wFlushFlags & CEL_MAPFLUSH_TYPEMASK);
            FlushFlags = (CelogMapFlushFlags)(data.wFlushFlags & CEL_MAPFLUSH_FLAGMASK);
        }

        public CeHandle ProcessHandle { get; }
        public CePtr BaseAddress { get; }
        public uint Len { get; }
        public uint NumPages { get; }
        public CelogMapFlushType FlushType { get; }
        public CelogMapFlushFlags FlushFlags { get; }

    }
}
