using System;
using System.Linq;
using System.Collections.Generic;

namespace CelogParserLib
{
    static class RangeExtention
    {
        #if false
        public static bool Contains(this Range range, int value)
        {
            var start = range.Start.IsFromEnd ? (int.MaxValue - range.Start.Value) : range.Start.Value;
            var end = range.End.IsFromEnd ? (int.MaxValue - range.End.Value) : range.End.Value;
            return start <= value && value < end;
        }
        public static IEnumerable<int> AsEnumerable(this Range range)
        {
            var start = range.Start.IsFromEnd ? (int.MaxValue - range.Start.Value) : range.Start.Value;
            var end = range.End.IsFromEnd ? (int.MaxValue - range.End.Value) : range.End.Value;
            return Enumerable.Range(start, end - start);
        }
        #endif
    }
}
