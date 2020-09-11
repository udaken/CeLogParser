using CelogParserLib.ARM;
using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogDebugTrap : ICelogInfo
    {
        internal CelogDebugTrap(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_DEBUG_TRAP>();
            Flags = data.wFlags;
            Context = data.context;
            ExceptionRecord = data.er;
            StackTraceOffset = data.cbStackTraceOffset;
            StackTrace = data.GetStackTrace(buffer.Length).ToArray();
        }

        public ushort Flags { get; }
        public ushort StackTraceOffset { get; }
        public CONTEXT Context { get; private set; }
        internal EXCEPTION_RECORD ExceptionRecord { get; private set; }

        public IReadOnlyList<uint> StackTrace { get; }
        public override string ToString()
            => $"DebugTrap";

        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
