using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// ActivatorService
    /// </summary>
    internal class ActivatorService : TestAttributeService
    {
        #region CreateInstance / Dispose

        /// <summary>
        /// CreateInstance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="possibleParams"></param>
        /// <returns></returns>
        public object CreateInstance(Type type, params object[] possibleParams)
        {
            var constructor = type.GetConstructors().FirstOrDefault();
            return constructor.Invoke(GetMethodOrderParameters(constructor, possibleParams));
        }

        /// <summary>
        /// TryForceDispose
        /// </summary>
        /// <param name="instance"></param>
        public void TryForceDispose(object instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        #endregion

        #region Invoke

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="possibleParams"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException"></exception>
        public object Invoke(object obj, MethodInfo method, params object[] possibleParams)
        {
            if (method is null)
                return null;

            var methodParams = GetMethodOrderParameters(method, possibleParams);

            if (IsReturnTypeEqualsTask(method))
            {
                var taskInvoke = method.Invoke(obj, methodParams);

                if (taskInvoke is Task task)
                {
                    return InvokeTask(task);
                }

                throw new TaskCanceledException("Task method not supported!");
            }

            return method.Invoke(obj, methodParams);
        }

        /// <summary>
        /// Invoke Task and Return 'Result'
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private object InvokeTask(Task task)
        {
            CancellationTokenSource source = new CancellationTokenSource(TestEngineFilter.CancellationTokenTimeOut);
            var cancellationToken = source.Token;
            using (cancellationToken.Register(() => { }))
            {
                try
                {
                    Task.Run(async () => { await task.ConfigureAwait(false); }, cancellationToken).GetAwaiter().GetResult();
                    var resultProperty = task.GetType().GetProperty("Result");
                    return resultProperty?.GetValue(task);
                }
                catch (Exception) when (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Next line will never be reached because cancellation will always have been requested in this catch block.
                    // But it's required to satisfy compiler.
                    throw new InvalidOperationException();
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Is <paramref name="method"/> Task or Task<>
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool IsReturnTypeEqualsTask(MethodInfo method)
        {
            var returnType = method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return true;
            }

            return returnType == typeof(Task);
        }
        #endregion

        #region Order Parameters
        /// <summary>
        /// GetMethodOrderParameters
        /// </summary>
        /// <param name="methodBase"></param>
        /// <param name="possibleParams"></param>
        /// <returns></returns>
        private object[] GetMethodOrderParameters(MethodBase methodBase, params object[] possibleParams)
        {
            var result = new List<object>();
            var possibleParamsTemp = possibleParams.ToList();

            foreach (ParameterInfo parameter in methodBase.GetParameters())
            {
                object o = possibleParamsTemp.FirstOrDefault(e => IsParameterTypeSimilar(parameter, e));
                possibleParamsTemp.Remove(o);
                result.Add(o);
            }

            return result.ToArray();
        }

        private bool IsParameterTypeSimilar(ParameterInfo parameter, object parameterValue)
        {
            if (parameter.ParameterType == typeof(object))
                return true;

            return parameterValue.GetType().Equals(parameter.ParameterType);
        }

        #endregion
    }
}
