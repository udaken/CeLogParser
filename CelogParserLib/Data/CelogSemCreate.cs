using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public interface ICelogSemInfo : ICelogKernelObjectInfo
    {
        uint InitialCount { get; }
        uint MaxCount { get; }
    }
    public sealed class CelogSemCreate : ICelogSemInfo
    {
        internal CelogSemCreate(ReadOnlySpan<byte> buffer, TimeSpan timestamp)
        {
            ref readonly var data = ref buffer.AsRef<CEL_SEM_CREATE>();
            SemaphoreHandle = data.hSem;
            MaxCount = data.dwMaxCount;
            InitialCount = data.dwInitCount;
            Name = buffer.Length != Unsafe.SizeOf<CEL_SEM_CREATE>() ? data.GetName(buffer.Length).ToNullTerminateString() : "<anonymous>";
            CreatedAt = timestamp;
        }

        public CeHandle SemaphoreHandle { get; }
        CeHandle ICelogKernelObjectInfo.Handle => SemaphoreHandle;

        public uint InitialCount { get; }

        public uint MaxCount { get; }
        public string Name { get; }
        public TimeSpan? DeleteAt { get; internal set; }

        public override string ToString()
         => $"Handle={SemaphoreHandle}, MaxCount={MaxCount}, InitialCount={InitialCount}, {Name}";

        public bool IsPsudoObject => false;

        public TimeSpan CreatedAt { get; }
        public TimeSpan? DeletedAt { get; internal set; }
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ SemaphoreHandle };
    }
}
