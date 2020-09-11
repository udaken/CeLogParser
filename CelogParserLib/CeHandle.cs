using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CelogParserLib
{
    [System.Serializable]
    public readonly struct CeHandle : IEquatable<CeHandle>
    {
        internal static readonly IReadOnlyList<CeHandle> EmptyList  = Array.Empty<CeHandle>();

        readonly uint Value;

        public static CeHandle InvalidHandleValue
            => new CeHandle(0xFFFFFFFF);

        public bool IsInvalid
            => this == InvalidHandleValue;

        internal CeHandle(uint value)
            => Value = value;

        public override bool Equals(object obj)
            => obj is CeHandle value ? Equals(value) : false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(CeHandle other)
            => Value == other.Value;

        public override int GetHashCode()
            => Value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CeHandle left, CeHandle right)
            => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CeHandle left, CeHandle right)
            => !(left == right);

        public override string ToString()
            => $"0x{Value:X8}";
    }
}
