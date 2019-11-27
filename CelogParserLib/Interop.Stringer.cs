using System;
namespace CelogParserLib
{
    static unsafe partial class Interop
    {
        public partial struct CEL_HEADER_V3
        {
            public override string ToString()
            {
                return $"HEADER_V3(Length={Length},CPU={CPU},ID={ID},fTimeStamp={fTimeStamp},fFileTime={fFileTime})";
            }
        }

    }
}
