using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public interface ICelogMutexInfo : ICelogKernelObjectInfo
    {

    }
    public sealed class CelogMutexCreate : ICelogMutexInfo
    {
        internal CelogMutexCreate(ReadOnlySpan<byte> buffer, TimeSpan createdAt)
        {
            ref readonly var data = ref buffer.AsRef<CEL_MUTEX_CREATE>();
            MutexHandle = data.hMutex;
            Name = buffer.Length != Unsafe.SizeOf<CEL_MUTEX_CREATE>() ? data.GetName(buffer.Length).ToNullTerminateString() : "<anonymous>";
            CreatedAt = createdAt;
        }

        public CeHandle MutexHandle { get; }
        CeHandle ICelogKernelObjectInfo.Handle => MutexHandle;
        public string Name { get; }
        public override string ToString()
            => $"Handle={MutexHandle}, {Name}";

        public bool IsPsudoObject => false;

        public TimeSpan CreatedAt { get; }
        public TimeSpan? DeletedAt { get; internal set; }
        public IReadOnlyList<CeHandle> ContainsHadles => new []{ MutexHandle };

    }
}
