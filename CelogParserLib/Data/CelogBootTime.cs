using System;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogBootTime : ICelogInfo
    {
        internal CelogBootTime(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_BOOT_TIME>();
            Action = data.dwAction;
            Name = data.GetName(buffer.Length).ToNullTerminateString();
        }
        public CelogBootTimeAction Action { get; }
        public string Name { get; }

        public override string ToString()
         => $"Action={Action}, {Name}";
    }
}