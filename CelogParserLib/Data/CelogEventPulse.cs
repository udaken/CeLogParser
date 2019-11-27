using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogEventPulse : CelogEventBase
    {
        internal CelogEventPulse(ReadOnlySpan<byte> buffer, List<CelogEventCreate> events)
            : base(buffer.AsRef<CEL_EVENT_PULSE>().hEvent, events)
        {
        }
    }
}
