using System;
using System.Collections.Generic;
using System.Linq;
using UnsafeAndSpanExtension;
using static CelogParserLib.Interop;
namespace CelogParserLib.Data
{
    public sealed class CelogWaitMulti : ICelogInfo
    {
        class CeHandleArrayComparer : IEqualityComparer<WeakReference<CeHandle[]>>
        {
            public bool Equals(WeakReference<CeHandle[]> x, WeakReference<CeHandle[]> y)
                => x.TryGetTarget(out var xTarget) && y.TryGetTarget(out var yTarget) && xTarget.SequenceEqual(yTarget);

            public int GetHashCode(WeakReference<CeHandle[]> obj)
                => obj.TryGetTarget(out var target) ? GetHashCode(target) : 0;

            internal static int GetHashCode(ReadOnlySpan<CeHandle> obj)
            {
                var h = 0;
                for (var i = 0; i < obj.Length; i++)
                {
                    h ^= (obj[i].GetHashCode() ^ ~i);
                }
                return h;
            }
            internal static bool TryGetCache(HashSet<WeakReference<CeHandle[]>> cache, ReadOnlySpan<CeHandle> obj, out CeHandle[] v)
            {
                foreach (var cacheEntry in cache)
                {
                    if (cacheEntry.TryGetTarget(out var c) && obj.SequenceEqual(c))
                    {
                        v = c;
                        return true;
                    }
                }
                v = Array.Empty<CeHandle>();
                return false;
            }
        }
        private static readonly HashSet<WeakReference<CeHandle[]>> _Cache = new HashSet<WeakReference<CeHandle[]>>(new CeHandleArrayComparer());
        internal CelogWaitMulti(ReadOnlySpan<byte> buffer)
        {
            ref readonly var data = ref buffer.AsRef<CEL_WAIT_MULTI>();
            Timeout = data.dwTimeout;
            WaitAll = data.fWaitAll;
            var span = data.GetHandles(buffer.Length);
#if false
            if (span.Length > 4)
            {
                if (CeHandleArrayComparer.TryGetCache(_Cache, span, out var cachedValue))
                {
                    WaitHandles = cachedValue;
                    return;
                }
            }
#endif
            var array = span.ToArray();
#if false
            if(array.Length > 4)
            {
                _Cache.Add(new WeakReference<CeHandle[]>(array));
            }
#endif
            WaitHandles = array;
        }

        public uint Timeout { get; }

        public bool WaitAll { get; }

        public IReadOnlyList<CeHandle> WaitHandles { get; }

        public bool IsSingle => WaitHandles.Count == 1;

        public override string ToString()
        {
            return $"Wait For {(IsSingle ? "Single Object" : (WaitAll ? "All Objects" : "Any Object"))} [{string.Join(",", WaitHandles)}] Timeout={(Timeout == 0xFFFFFFFF ? "INFINITE" : Timeout.ToString())}";
        }
    }
}
