using CelogParserLib.Data;
using CelogParserLib.Functional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace CelogParserLib
{
    internal delegate void RefAction<T>(ref T arg);
    internal delegate TResult RefFunc<T, TResult>(ref T arg);
    internal delegate void RefReadOnlyAction<T>(in T arg);
    internal delegate TResult RefReadOnlyFunc<T, TResult>(in T arg);

    internal static class Extension
    {
        public static System.Runtime.InteropServices.ComTypes.FILETIME ReadFileTime(this BinaryReader reader)
        {
            var lo = reader.ReadInt32();
            var hi = reader.ReadInt32();
            return new System.Runtime.InteropServices.ComTypes.FILETIME() { dwLowDateTime = lo, dwHighDateTime = hi };
        }
        public static bool TryConvertToInt32(this long value, out int result)
        {
            if (value >= int.MaxValue || value <= int.MinValue)
            {
                result = default;
                return false;
            }
            {
                result = (int)value;
                return true;
            }
        }
        public static bool TryConvertToUInt32(this long value, out uint result)
        {
            if (value >= uint.MaxValue)
            {
                result = default;
                return false;
            }
            {
                result = (uint)value;
                return true;
            }
        }

        public static bool TryConvertToInt16(this int value, out short result)
        {
            if (value >= short.MaxValue || value <= short.MinValue)
            {
                result = default;
                return false;
            }
            else
            {
                result = (short)value;
                return true;
            }
        }

        public static TSource? LastOrNullable<TSource>(this IEnumerable<TSource> source) where TSource : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is IReadOnlyList<TSource> list)
            {
                int count = list.Count;
                if (count > 0)
                {
                    return list[count - 1];
                }
            }
            else
            {
                using var enumerator = source.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    TSource current;
                    do
                    {
                        current = enumerator.Current;
                    }
                    while (enumerator.MoveNext());
                    return current;
                }
            }
            return null;
        }

        public static TSource? LastOrNullable<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : struct
            => LastOrNullable(source, (in TSource arg) => predicate(arg));
        public static TSource? LastOrNullable<TSource>(this IEnumerable<TSource> source, RefReadOnlyFunc<TSource, bool> predicate) where TSource : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (source is IReadOnlyList<TSource> list)
            {
                int count = list.Count;
                for (var i = count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if (predicate(item))
                    {
                        return item;
                    }
                }
                return null;
            }
            else
            {
                TSource? result = null;
                foreach (var item in source)
                {
                    if (predicate(item))
                    {
                        result = item;
                    }
                }
                return result;
            }
        }

        public static TSource? LastOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (source is IReadOnlyList<TSource> list)
            {
                int count = list.Count;
                for (var i = count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if (predicate(item))
                    {
                        return item;
                    }
                }
                return null;
            }
            else
            {
                TSource? result = null;
                foreach (var item in source)
                {
                    if (predicate(item))
                    {
                        result = item;
                    }
                }
                return result;
            }
        }

        public static List<TResult> ToList<TSource, TResult>(this ReadOnlySpan<TSource> span, Func<TSource, TResult> act)
            => ToList(span, (in TSource item) => act(item));
        public static List<TResult> ToList<TSource, TResult>(this ReadOnlySpan<TSource> span, RefReadOnlyFunc<TSource, TResult> act)
        {
            var result = new List<TResult>(span.Length);

            for (var i = 0; i < span.Length; i++)
            {
                result.Add(act(in span[i]));
            }
            return result;
        }
        public static TResult[] ToArray<TSource, TResult>(this ReadOnlySpan<TSource> span, RefReadOnlyFunc<TSource, TResult> act)
        {
            var result = new TResult[span.Length];

            for (var i = 0; i < span.Length; i++)
            {
                result[i] = act(in span[i]);
            }
            return result;
        }

        public static List<TResult> ToList<TSource, TResult>(this Span<TSource> span, Func<TSource, TResult> act)
            => ToList((ReadOnlySpan<TSource>)span, (in TSource item) => act(item));

        public static List<TResult> ToList<TSource, TResult>(this Span<TSource> span, RefReadOnlyFunc<TSource, TResult> act)
            => ToList((ReadOnlySpan<TSource>)span, act);

        public static T? LatestEvent<T>(this IEnumerable<T> kobjects, CeHandle handle) where T : class, ICelogKernelObjectInfo
            => handle.IsInvalid ? null : kobjects.LastOrNull(info => info.Handle == handle);


    }
}
