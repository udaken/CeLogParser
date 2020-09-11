using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public interface ICelogEventInfo : ICelogKernelObjectInfo
    {
        bool Manual { get; }
        bool InitialState { get; }
    }
    public sealed class CelogEventCreate : ICelogEventInfo
    {
        internal CelogEventCreate(ReadOnlySpan<byte> buffer, TimeSpan timestamp)
        {
            ref readonly var data = ref buffer.AsRef<CEL_EVENT_CREATE>();
            EventHandle = data.hEvent;
            Manual = data.fManual;
            InitialState = data.fInitialState;
            Create = data.fCreate;
            Name = buffer.Length != Unsafe.SizeOf<CEL_EVENT_CREATE>() ? data.GetName(buffer.Length).ToNullTerminateString() : "<anonymous>";
            CreatedAt = timestamp;
        }

        public CeHandle EventHandle { get; }
        CeHandle ICelogKernelObjectInfo.Handle => EventHandle;

        public bool Manual { get; }
        public bool InitialState { get; }
        public bool Create { get; }
        public string Name { get; }
        public TimeSpan CreatedAt { get; }
        public TimeSpan? DeletedAt { get; internal set; }
        public override string ToString()
            => $"Handle={EventHandle}, Manual={Manual}, InitialState={InitialState}, Create={Create}, {Name}";

        public bool IsPsudoObject => false;
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ EventHandle };
    }
}
