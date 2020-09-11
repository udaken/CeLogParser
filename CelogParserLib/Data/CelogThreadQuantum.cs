using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogThreadQuantum : ICelogInfo
    {
        internal CelogThreadQuantum(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_THREAD_QUANTUM>();
            this.ThreadHandle = data.hThread;
            this.Quantum = data.dwQuantum;
        }

        public CeHandle ThreadHandle { get; }
        public uint Quantum { get; }
        
        public override string ToString()
         => $"Handle={ThreadHandle}, Quantum={Quantum}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
