using System;
using System.Collections.Generic;

namespace CelogParserLib.Data
{
    public interface ICelogInfo
    {
        static readonly IReadOnlyList<CeHandle> Empty  = Array.Empty<CeHandle>();
        IReadOnlyList<CeHandle> ContainsHadles { get; }
    }

}
