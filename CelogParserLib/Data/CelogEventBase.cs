using System.Collections.Generic;

namespace CelogParserLib.Data
{
    public abstract class CelogEventBase : ICelogInfo
    {
        internal CelogEventBase(CeHandle hEvent, List<CelogEventCreate> events)
        {
            EventHandle = hEvent;

            if (events.LatestEvent(EventHandle) is { } t)
            {
                Origin = t;
            }
        }

        public CeHandle EventHandle { get; }
        
        public ICelogEventInfo? Origin { get; }

        public override string ToString()
            => $"Handle={EventHandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ EventHandle };
    }
}
