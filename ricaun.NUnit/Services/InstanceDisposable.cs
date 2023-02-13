using System;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// InstanceDisposable
    /// </summary>
    internal class InstanceDisposable : ActivatorService, IDisposable
    {
        private readonly object instance;

        /// <summary>
        /// InstanceDisposable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="possibleParams"></param>
        public InstanceDisposable(Type type, params object[] possibleParams)
        {
            this.instance = CreateInstance(type, possibleParams);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="method"></param>
        /// <param name="possibleParams"></param>
        /// <returns></returns>
        public object Invoke(MethodInfo method, params object[] possibleParams)
        {
            return Invoke(instance, method, possibleParams);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            TryForceDispose(this.instance);
        }
    }
}
