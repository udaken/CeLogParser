using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogEventSet : CelogEventBase
    {
        internal CelogEventSet(ReadOnlySpan<byte> buffer, List<CelogEventCreate> events)
            : base(buffer.AsRef<CEL_EVENT_SET>().hEvent, events)
        {
        }
    }
}
