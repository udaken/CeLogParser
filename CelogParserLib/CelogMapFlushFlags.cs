using System;
using static CelogParserLib.Interop;

namespace CelogParserLib
{
    [Flags]
    public enum CelogMapFlushFlags : ushort
    {
        None = 0,
        Begin = CEL_MAPFLUSH_BEGIN,
        Fulldiscard = CEL_MAPFLUSH_FULLDISCARD,
        Nowriteout = CEL_MAPFLUSH_NOWRITEOUT,
    }
}
