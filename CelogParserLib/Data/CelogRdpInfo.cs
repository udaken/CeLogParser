using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogRdpInfo : ICelogInfo
    {
        internal CelogRdpInfo(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_RDP_INFO>();
            OrderType = data.bOrderType;
            Order = data.bOrder;
            TimeSpent = data.dwTimeSpent;
        }
        public byte OrderType { get; }
        public byte Order { get; }
        public uint TimeSpent { get; }

        public override string ToString()
         => $"RDP, ({nameof(OrderType)}:{OrderType}, {nameof(Order)}:{Order}, {nameof(TimeSpent)}:{TimeSpent})";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}