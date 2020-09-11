using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogEventlogWppMsg : ICelogInfo
    {
        internal CelogEventlogWppMsg(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_EVENTLOG_WPP_MSG>();
            MissedEvents = data.cMissedEvents;
            KernelTime = data.KernelTime;
            UserTime = data.UserTime;
        }

        public uint MissedEvents { get; }
        public FILETIME KernelTime { get; }
        public FILETIME UserTime { get; }
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
