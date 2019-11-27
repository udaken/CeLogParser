using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogSystemPageIn : ICelogInfo
    {
        internal CelogSystemPageIn(ReadOnlySpan<byte> buffer, System.Collections.Generic.List<CelogProcessCreate> kObjects)
        {
            ref readonly var data = ref buffer.AsRef<CEL_SYSTEM_PAGE_IN>();
            Address = data.dwAddress;
            ReadWrite = data.fReadWrite;
            EndPageIn = data.fEndPageIn;
            Success = data.fSuccess;
            ProcessHandle = data.hProcess;

            Process = kObjects.LatestEvent(ProcessHandle);
        }

        public uint Address { get; }
        public bool ReadWrite { get; }
        public bool EndPageIn { get; }
        public bool Success { get; }
        public CeHandle ProcessHandle { get; }
        public ICelogProcessInfo? Process { get; }
    }
}
