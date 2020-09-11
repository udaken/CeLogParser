using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogEventlogChannelMsg : ICelogInfo
    {
        internal unsafe CelogEventlogChannelMsg(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_EVENTLOG_CHANNEL_MSG>();
            ProviderID = data.ProviderID;
            CPUType = data.CPUType;
            EventDescriptor = data.EventDescriptor;
            cbUserData = data.cbUserData;
            eUserDataCount = data.eUserDataCount;
            SequenceNumber = data.SequenceNumber;
            AlignmentPadding = data.AlignmentPadding;

            fixed (CEL_EVENTLOG_CHANNEL_MSG* p = &data)
            {
                UserData = new byte[4] {
                    p->UserData[0],
                    p->UserData[1],
                    p->UserData[2],
                    p->UserData[3],
                };
            }
        }

        public Guid ProviderID { get; }
        public uint CPUType { get; }
        internal EVENT_DESCRIPTOR EventDescriptor { get; }
        public uint cbUserData { get; }
        public uint eUserDataCount { get; }
        public uint SequenceNumber { get; }
        public uint AlignmentPadding { get; }
        public IReadOnlyList<byte> UserData { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
