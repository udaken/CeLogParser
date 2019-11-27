using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogDebugMsg : ICelogInfo
    {
        internal CelogDebugMsg(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_DEBUG_MSG>();

            Pid = data.pid;
            Tid = data.tid;
            Message = data.GetMessage(buffer.Length).ToNullTerminateString();
        }

        public uint Pid { get; }
        public uint Tid { get; }
        public string Message { get; }

        public override string ToString() => $"PID:{Pid:X} TID:{Tid:X} {Message}";

    }
}
