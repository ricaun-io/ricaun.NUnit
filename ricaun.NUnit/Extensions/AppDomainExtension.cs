using System;
using System.Reflection;

namespace ricaun.NUnit.Extensions
{
    internal static class AppDomainExtension
    {
        /// <summary>
        /// Get private <see cref="AppDomain.AssemblyResolve"/> EventHandler
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public static ResolveEventHandler GetResolveEventHandler(this AppDomain appDomain)
        {
            var fieldName = "_AssemblyResolve";

            var fieldInfo = appDomain.GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            return fieldInfo?.GetValue(appDomain) as ResolveEventHandler;
        }

        /// <summary>
        /// Create new <see cref="DelegatesDisposable"/> for <see cref="AppDomain.AssemblyResolve"/>
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public static DelegatesDisposable GetAssemblyResolveDisposable(this AppDomain appDomain)
        {
            var resolveEventHandler = appDomain.GetResolveEventHandler();
            return new DelegatesDisposable(
                value => appDomain.AssemblyResolve += (ResolveEventHandler)value,
                value => appDomain.AssemblyResolve -= (ResolveEventHandler)value,
                () =>
                {
                    return appDomain.GetResolveEventHandler()?.GetInvocationList() ?? new Delegate[] { };
                }
                );
        }
    }
}