using System;
using System.Runtime.CompilerServices;

namespace CelogParserLib
{
    [System.Serializable]
    public readonly struct CePtr : IEquatable<CePtr>, IComparable<CePtr>
    {
        private readonly uint Value;

        public static readonly CePtr Zero = default;

        public bool IsZero => this == Zero;

        internal CePtr(uint value)
            => Value = value;

        public override bool Equals(object obj)
            => obj is CePtr value ? Equals(value) : false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(CePtr other)
            => Value == other.Value;

        public override int GetHashCode()
            => Value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(uint left, CePtr right)
            => new CePtr(left).Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(uint left, CePtr right)
            => !(left == right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CePtr left, uint right)
            => left.Equals(new CePtr(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CePtr left, uint right)
            => !(left == right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(CePtr left, CePtr right)
            => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(CePtr left, CePtr right)
            => !(left == right);

        public override string ToString()
            => Value.ToString("X8");

        public int CompareTo(CePtr other)
            => Value.CompareTo(other.Value);

        public static bool operator <(CePtr left, CePtr right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(CePtr left, CePtr right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(CePtr left, CePtr right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(CePtr left, CePtr right)
            => left.CompareTo(right) >= 0;

        public static CePtr operator +(CePtr p, uint offset)
            => new CePtr(p.Value + offset);
            
        public static uint operator -(CePtr left, CePtr right)
            => left.Value - right.Value;
    }
}
