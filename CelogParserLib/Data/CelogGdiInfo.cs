using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogGdiInfo : ICelogInfo
    {
        internal CelogGdiInfo(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_GDI_INFO>();
            Operation = data.dwGDIOp;
            EntryTime = data.dwEntryTime;
            Context = data.dwContext;
            Context2 = data.dwContext2;
            Context3 = data.dwContext3;
            Context4 = data.dwContext4;
        }
        public CelogGdiOp Operation { get; }
        public uint EntryTime { get; }
        public uint Context { get; }
        public uint Context2 { get; }
        public uint Context3 { get; }
        public uint Context4 { get; }

        public override string ToString()
         => $"GDI, {Operation}";
    }
}