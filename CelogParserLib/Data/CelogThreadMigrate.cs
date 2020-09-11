using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogThreadMigrate : ICelogInfo
    {
        internal CelogThreadMigrate(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_MIGRATE>();
            Processhandle = data.hProcess;
        }

        public CeHandle Processhandle { get; }
        
        public override string ToString()
         => $"Handle={Processhandle}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}