using System;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.NUnit.Extensions
{
    /// <summary>
    /// TaskSTA
    /// </summary>
    internal static class TaskSTA
    {
        /// <summary>
        /// CurrentThread ApartmentState is <see cref="ApartmentState.STA"/>
        /// </summary>
        /// <returns></returns>
        private static bool CurrentThreadIsSTA()
        {
            // System.Diagnostics.Debug.WriteLine($"CurrentThread: {System.Threading.Thread.CurrentThread.GetApartmentState()}");
            return System.Threading.Thread.CurrentThread.GetApartmentState() == ApartmentState.STA;
        }

        /// <summary>
        /// RunSafe if CurrentThread is <see cref="ApartmentState.STA"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T RunSafe<T>(Func<T> func)
        {
            if (CurrentThreadIsSTA())
            {
                return Run(func).GetAwaiter().GetResult();
            }
            return func();
        }

        /// <summary>
        /// RunSafe if CurrentThread is <see cref="ApartmentState.STA"/>
        /// </summary>
        /// <param name="action"></param>
        public static void RunSafe(Action action)
        {
            if (CurrentThreadIsSTA())
            {
                Run(action).GetAwaiter().GetResult();
                return;
            }
            action();
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Task<T> Run<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            Thread thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
#if NET
if (OperatingSystem.IsWindows())
#endif
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task Run(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            Thread thread = new Thread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
#if NET
if (OperatingSystem.IsWindows())
#endif
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }
    }
}
