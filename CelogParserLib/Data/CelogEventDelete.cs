using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogEventDelete : ICelogInfo
    {
        internal CelogEventDelete(ReadOnlySpan<byte> buffer, TimeSpan timestamp, List<CelogEventCreate> events)
        {
            ref readonly var data = ref buffer.AsRef<CEL_EVENT_DELETE>();
            EventHandle = data.hEvent;

            if (events.LatestEvent(EventHandle) is { } e)
            {
                Origin = e;
                e.DeletedAt = timestamp;
            }
        }

        public CeHandle EventHandle { get; }
        public ICelogEventInfo? Origin { get; }

        public override string ToString()
            => $"Handle={EventHandle}";
    }
}
