using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogEventReset : CelogEventBase
    {
        internal CelogEventReset(ReadOnlySpan<byte> buffer, List<CelogEventCreate> events)
            : base(buffer.AsRef<CEL_EVENT_RESET>().hEvent, events)
        {
        }
    }
}
