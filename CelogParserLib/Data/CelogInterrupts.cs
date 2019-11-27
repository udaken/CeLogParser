using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogInterrupts : ICelogInfo
    {
        internal CelogInterrupts(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_INTERRUPTS>();
            Discarded = data.dwDiscarded;

            IntData = data.GetIntData(buffer.Length).ToList((in CEL_INT_DATA item)
                => (item.dwTimeStamp, item.wSysIntr, item.wNestingLevel));
        }

        public uint Discarded { get; }
        public IReadOnlyList<(uint TimeStamp, ushort SysInter, ushort NestingLevel)> IntData { get; }
        public override string ToString()
            => $"Discarded={Discarded}, IntData={string.Join(",", IntData)}";
    }
}