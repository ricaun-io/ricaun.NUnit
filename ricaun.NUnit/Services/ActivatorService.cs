using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

            if (method.ReturnType == typeof(Task))
            {
                throw new TaskCanceledException("Task method not supported!");

                //var taskInvoke = (Task)method.Invoke(obj, methodParams);
                //if (!taskInvoke.Wait(2000))
                //{
                //    Console.WriteLine("Task 2000ms Timeout");
                //};

                //var task = Task.Run(async () =>
                //{
                //    await taskInvoke;
                //});
                //task.GetAwaiter().GetResult();

                //return;
            }

            return method.Invoke(obj, methodParams);
        }

        /// <summary>
        /// InvokeAsync
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="possibleParams"></param>
        /// <returns></returns>
        public async Task InvokeAsync(object obj, MethodInfo method, params object[] possibleParams)
        {
            if (method is null)
                return;

            var methodParams = GetMethodOrderParameters(method, possibleParams);

            if (method.ReturnType == typeof(Task))
            {
                await (Task)method.Invoke(obj, methodParams);
                return;
            }

            method.Invoke(obj, methodParams);
        }

        /// <summary>
        /// InvokeAsync
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="possibleParams"></param>
        /// <returns></returns>
        public async Task InvokeAsync(object obj, string methodName, params object[] possibleParams)
        {
            var method = obj?.GetType().GetMethod(methodName);
            await InvokeAsync(obj, method, possibleParams);
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
                object o = possibleParamsTemp.FirstOrDefault(e => e.GetType().Equals(parameter.ParameterType));
                possibleParamsTemp.Remove(o);
                result.Add(o);
            }

            return result.ToArray();
        }
        #endregion
    }
}
