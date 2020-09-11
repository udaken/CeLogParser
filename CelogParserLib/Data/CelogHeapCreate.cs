using System;
using System.Collections.Generic;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;

namespace CelogParserLib.Data
{
    public sealed class CelogHeapCreate : ICelogInfo
    {
        internal CelogHeapCreate(ReadOnlySpan<byte> buffer, TimeSpan timestamp)
        {
            ref readonly var data = ref buffer.AsRef<CEL_HEAP_CREATE>();
            Handle = data.hHeap;
            Options = (HeapFlags)data.dwOptions;
            InitSize = data.dwInitSize;
            MaxSize = data.dwMaxSize;
            Tid = data.dwTID;
            Pid = data.dwPID;

            CreateAt = timestamp;
        }

        public TimeSpan CreateAt { get; }
        public HeapFlags Options { get; }
        public uint InitSize { get; }
        public uint MaxSize { get; }
        public CeHandle Handle { get; }
        public uint Tid { get; }
        public uint Pid { get; }

        public TimeSpan? DestroyAt { get; internal set; }
        public override string ToString()
            => $"Handle={Handle}, Options={Options}, InitSize={InitSize}, MaxSize={MaxSize}, Tid={Tid:X}, Pid={Pid:X}";
        public IReadOnlyList<CeHandle> ContainsHadles => CeHandle.EmptyList;
    }
}
