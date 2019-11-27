using System;

namespace CelogParserLib.Functional
{
    static class DeferExtention
    {
        public static void Defer(Action finallyAction, Action statements)
        {
            if (finallyAction is null)
            {
                throw new ArgumentNullException(nameof(finallyAction));
            }

            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            try
            {
                statements();
            }
            finally
            {
                finallyAction();
            }
        }
        public static void Defer<T>(Action<T> finallyAction, T arg, Action statements)
        {
            if (finallyAction is null)
            {
                throw new ArgumentNullException(nameof(finallyAction));
            }

            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            try
            {
                statements();
            }
            finally
            {
                finallyAction(arg);
            }
        }
        public static void Defer<T1, T2>(Action<T1, T2> finallyAction, T1 arg1, T2 arg2, Action statements)
        {
            if (finallyAction is null)
            {
                throw new ArgumentNullException(nameof(finallyAction));
            }

            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            try
            {
                statements();
            }
            finally
            {
                finallyAction(arg1, arg2);
            }
        }
        public static void Defer<T1, T2, T3>(Action<T1, T2, T3> finallyAction, T1 arg1, T2 arg2, T3 arg3, Action statements)
        {
            if (finallyAction is null)
            {
                throw new ArgumentNullException(nameof(finallyAction));
            }

            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            try
            {
                statements();
            }
            finally
            {
                finallyAction(arg1, arg2, arg3);
            }
        }
    }
}
