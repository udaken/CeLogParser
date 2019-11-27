using System;
using System.Collections.Generic;
using System.Text;

namespace CelogParserLib.Functional
{
    internal static class Extension
    {
        public static Action<T1> DiscardReturnValue<T1, TResult>(Func<T1, TResult> func)
            => arg1 => func(arg1);
        public static Action<T1, T2> DiscardReturnValue<T1, T2, TResult>(Func<T1, T2, TResult> func)
            => (arg1, arg2) => func(arg1, arg2);
        public static Action<T1, T2, T3> DiscardReturnValue<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
            => (arg1, arg2, arg3) => func(arg1, arg2, arg3);
        public static Action<T1, T2, T3, T4> DiscardReturnValue<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func)
            => (arg1, arg2, arg3, arg4) => func(arg1, arg2, arg3, arg4);
        public static Action<T1, T2, T3, T4, T5> DiscardReturnValue<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func)
            => (arg1, arg2, arg3, arg4, arg5) => func(arg1, arg2, arg3, arg4, arg5);
        public static Action<T1, T2, T3, T4, T5, T6> DiscardReturnValue<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func)
            => (arg1, arg2, arg3, arg4, arg5, arg6) => func(arg1, arg2, arg3, arg4, arg5, arg6);
        public static Action<T1, T2, T3, T4, T5, T6, T7> DiscardReturnValue<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
            => (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        public static TResult Invoke<T, TResult>(this T self, Func<T, TResult> func)
            => func(self);

        public static T1 Chain<T1, T2>(this T1 self, T2 arg, Action<T1, T2> action)
        {
            action(self, arg);
            return self;
        }
        public static T Chain<T>(this T self, RefAction<T> action)
        {
            action(ref self);
            return self;
        }
        public static T Chain<T>(this T self, Action<T> action)
        {
            action(self);
            return self;
        }
        public static T Chain<T>(this T self, Action action)
        {
            action();
            return self;
        }
        public static T? ToNullable<T>(this T s) where T : struct => s;

#if false
        private static bool ContainsInternal<TTuple, T>(TTuple tuple, T value)
             where TTuple : System.Runtime.CompilerServices.ITuple
        {
            var eq = EqualityComparer<T>.Default;
            for (var i = 0; i < tuple.Length; i++)
            {
                var item = tuple[i];
                if (eq.Equals(value, (T)item))
                    return true;
            }
            return false;
        }
#endif
        private static bool Contains<T>(this T tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
            => (equalityComparer ?? EqualityComparer<T>.Default).Equals(tuple, value);

        public static bool Contains<T>(this ValueTuple<T> tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
            => (tuple.Item1).Contains(value, equalityComparer);

        public static bool Contains<T>(this ValueTuple<T, T> tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
        {
            var (_, item1) = tuple;
            return (tuple.Item1).Contains(value, equalityComparer) || (item1).Contains(value, equalityComparer);
        }
        public static bool Contains<T>(this ValueTuple<T, T, T> tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
        {
            var (_, item1, item2) = tuple;
            return (tuple.Item1).Contains(value, equalityComparer) || (item1, item2).Contains(value, equalityComparer);
        }
        public static bool Contains<T>(this ValueTuple<T, T, T, T> tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
        {
            var (_, item1, item2, item3) = tuple;
            return (tuple.Item1).Contains(value, equalityComparer) || (item1, item2, item3).Contains(value, equalityComparer);
        }
        public static bool Contains<T>(this ValueTuple<T, T, T, T, T> tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
        {
            var (_, item1, item2, item3, item4) = tuple;
            return (tuple.Item1).Contains(value, equalityComparer) || (item1, item2, item3, item4).Contains(value, equalityComparer);
        }
        public static bool Contains<T>(this ValueTuple<T, T, T, T, T, T> tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
        {
            var (_, item1, item2, item3, item4, item5) = tuple;
            return (tuple.Item1).Contains(value, equalityComparer) || (item1, item2, item3, item4, item5).Contains(value, equalityComparer);
        }
        public static bool Contains<T>(this ValueTuple<T, T, T, T, T, T, T> tuple, T value, IEqualityComparer<T>? equalityComparer = null)
             where T : struct
        {
            var (_, item1, item2, item3, item4, item5, item6) = tuple;
            return (tuple.Item1).Contains(value, equalityComparer) || (item1, item2, item3, item4, item5, item6).Contains(value, equalityComparer);
        }

        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T> tuple)
             where T : struct
        {
            var item1 = tuple.Item1;
            yield return item1;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T> tuple)
             where T : struct
        {
            var (item1, item2) = tuple;
            yield return item1;
            yield return item2;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T> tuple)
             where T : struct
        {
            var (item1, item2, item3) = tuple;
            yield return item1;
            yield return item2;
            yield return item3;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T> tuple)
             where T : struct
        {
            var (item1, item2, item3, item4) = tuple;
            yield return item1;
            yield return item2;
            yield return item3;
            yield return item4;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T, T> tuple)
             where T : struct
        {
            var (item1, item2, item3, item4, item5) = tuple;
            yield return item1;
            yield return item2;
            yield return item3;
            yield return item4;
            yield return item5;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T, T, T> tuple)
             where T : struct
        {
            var (item1, item2, item3, item4, item5, item6) = tuple;
            yield return item1;
            yield return item2;
            yield return item3;
            yield return item4;
            yield return item5;
            yield return item6;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T, T, T, T> tuple)
             where T : struct
        {
            var (item1, item2, item3, item4, item5, item6, item7) = tuple;
            yield return item1;
            yield return item2;
            yield return item3;
            yield return item4;
            yield return item5;
            yield return item6;
            yield return item7;
        }
        // ------------------------
        public static TResult Call<TResult>(this Func<TResult> func, ValueTuple arglist)
            => func();
        public static TResult Call<T1, TResult>(this Func<T1, TResult> func, ValueTuple<T1> arglist)
            => func(arglist.Item1);
        public static TResult Call<T1, T2, TResult>(this Func<T1, T2, TResult> func, ValueTuple<T1, T2> arglist)
            => func(arglist.Item1, arglist.Item2);
        public static TResult Call<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, ValueTuple<T1, T2, T3> arglist)
            => func(arglist.Item1, arglist.Item2, arglist.Item3);
        public static TResult Call<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, ValueTuple<T1, T2, T3, T4> arglist)
            => func(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4);
        public static TResult Call<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, ValueTuple<T1, T2, T3, T4, T5> arglist)
            => func(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5);
        public static TResult Call<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, ValueTuple<T1, T2, T3, T4, T5, T6> arglist)
            => func(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5, arglist.Item6);
        public static TResult Call<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, ValueTuple<T1, T2, T3, T4, T5, T6, T7> arglist)
            => func(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5, arglist.Item6, arglist.Item7);
        // ------------------------
        public static void Call(this Action action, ValueTuple arglist)
            => action();
        public static void Call<T1>(this Action<T1> action, ValueTuple<T1> arglist)
            => action(arglist.Item1);
        public static void Call<T1, T2>(this Action<T1, T2> action, ValueTuple<T1, T2> arglist)
            => action(arglist.Item1, arglist.Item2);
        public static void Call<T1, T2, T3>(this Action<T1, T2, T3> action, ValueTuple<T1, T2, T3> arglist)
            => action(arglist.Item1, arglist.Item2, arglist.Item3);
        public static void Call<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, ValueTuple<T1, T2, T3, T4> arglist)
            => action(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4);
        public static void Call<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, ValueTuple<T1, T2, T3, T4, T5> arglist)
            => action(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5);
        public static void Call<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, ValueTuple<T1, T2, T3, T4, T5, T6> arglist)
            => action(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5, arglist.Item6);
        public static void Call<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, ValueTuple<T1, T2, T3, T4, T5, T6, T7> arglist)
            => action(arglist.Item1, arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5, arglist.Item6, arglist.Item7);
        // ------------------------
        public static ValueTuple Shift(this ValueTuple arglist)
            => ValueTuple.Create();
        public static ValueTuple Shift<T1>(this ValueTuple<T1> arglist)
            => ValueTuple.Create();
        public static ValueTuple<T2> Shift<T1, T2>(this ValueTuple<T1, T2> arglist)
            => ValueTuple.Create(arglist.Item2);
        public static ValueTuple<T2, T3> Shift<T1, T2, T3>(this ValueTuple<T1, T2, T3> arglist)
            => ValueTuple.Create(arglist.Item2, arglist.Item3);
        public static ValueTuple<T2, T3, T4> Shift<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> arglist)
            => ValueTuple.Create(arglist.Item2, arglist.Item3, arglist.Item4);
        public static ValueTuple<T2, T3, T4, T5> Shift<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> arglist)
            => ValueTuple.Create(arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5);
        public static ValueTuple<T2, T3, T4, T5, T6> Shift<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> arglist)
            => ValueTuple.Create(arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5, arglist.Item6);
        public static ValueTuple<T2, T3, T4, T5, T6, T7> Shift<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> arglist)
            => ValueTuple.Create(arglist.Item2, arglist.Item3, arglist.Item4, arglist.Item5, arglist.Item6, arglist.Item7);
    }
}
