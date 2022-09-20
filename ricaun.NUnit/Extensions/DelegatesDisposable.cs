
using System;
using System.Diagnostics;

namespace ricaun.NUnit.Extensions
{
    /// <summary>
    /// DelegatesDisposable
    /// </summary>
    internal class DelegatesDisposable : IDisposable
    {
        private Delegate[] Delegates;
        private readonly Action<Delegate> add;
        private readonly Action<Delegate> remove;
        private readonly Func<Delegate[]> get;

        /// <summary>
        /// DelegatesDisposable
        /// </summary>
        /// <param name="add"></param>
        /// <param name="remove"></param>
        /// <param name="get"></param>
        public DelegatesDisposable(Action<Delegate> add, Action<Delegate> remove, Func<Delegate[]> get)
        {
            this.add = add;
            this.remove = remove;
            this.get = get;
            Delegates = get();

            Initialize();
        }

        private void Initialize()
        {
            //Debug.WriteLine($"DelegatesDisposable Initialize: {Delegates.Length}");
            foreach (var d in Delegates)
            {
                remove(d);
            }
        }

        private void RemoveAll()
        {
            var delegates = get();
            //Debug.WriteLine($"DelegatesDisposable Remove: {delegates.Length}");
            foreach (var d in delegates)
            {
                remove(d);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            RemoveAll();
            //Debug.WriteLine($"DelegatesDisposable Dispose: {Delegates.Length}");
            foreach (var d in Delegates)
            {
                add(d);
            }
        }
    }
}