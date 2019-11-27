using CelogParserLib.Data;
using System;
using System.Collections.Generic;

namespace CelogParserLib
{
    public struct CelogEventHandlerContext
    {
        internal CelogEventHandlerContext(IReadOnlyList<ICelogKernelObjectInfo> kObjects)
        {
            KernelObjects = kObjects;
        }
        public readonly IReadOnlyList<ICelogKernelObjectInfo> KernelObjects { get; }
    }
    public delegate object? CelogEventHandler(ushort id, ReadOnlySpan<byte> buffer, in CelogEventHandlerContext context);
}
