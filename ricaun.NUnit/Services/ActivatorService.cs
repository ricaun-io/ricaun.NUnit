using ricaun.NUnit.Extensions;
using System;
using System.Collections.Generic;
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
            var constructor = GetConstructorFitParameters(type, possibleParams);
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
                return TaskSTA.Run(() =>
                {
                    var taskInvoke = method.Invoke(obj, methodParams);
                    if (taskInvoke is Task task)
                    {
                        return InvokeTask(task);
                    }
                    throw new TaskCanceledException("Task method not supported!");

                }).GetAwaiter().GetResult();
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
                    task.Wait(cancellationToken);

                    var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));
                    var resultValue = resultProperty?.GetValue(task);

                    if (resultValue.GetType().Name.Equals("VoidTaskResult"))
                        return null;

                    return resultValue;
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
        /// Is <paramref name="method"/> Task or Task[]
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
        private static ConstructorInfo GetConstructorFitParameters(Type type, object[] possibleParams)
        {
            var constructorBase = type.GetConstructors().FirstOrDefault();
            var validConstructors = type.GetConstructors()
                .Where(e => IsConstructorValidParameters(e, possibleParams))
                .OrderByDescending(e => ConstructorFitParameters(e, possibleParams));

            var constructor = validConstructors.FirstOrDefault() ?? constructorBase;

            return constructor;
        }

        private static bool IsConstructorValidParameters(ConstructorInfo constructorInfo, object[] possibleParams)
        {
            var parameters = constructorInfo.GetParameters();
            if (parameters.Length == possibleParams.Length)
            {
                return parameters.All(e => IsParameterTypeSimilar(e, possibleParams[e.Position]));
            }
            return false;
        }

        private static int ConstructorFitParameters(ConstructorInfo constructorInfo, object[] possibleParams)
        {
            var value = 0;
            var parameters = constructorInfo.GetParameters();
            if (parameters.Length == possibleParams.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (possibleParams[i].GetType() == parameters[i].ParameterType)
                        value += 1;
                }
                return value;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// GetMethodOrderParameters
        /// </summary>
        /// <param name="methodBase"></param>
        /// <param name="possibleParams"></param>
        /// <returns></returns>
        private static object[] GetMethodOrderParameters(MethodBase methodBase, params object[] possibleParams)
        {
            var result = new List<object>();
            var possibleParamsTemp = possibleParams.ToList();

            foreach (ParameterInfo parameter in methodBase.GetParameters())
            {
                object o = possibleParamsTemp.FirstOrDefault(e => IsParameterTypeSimilar(parameter, e));
                possibleParamsTemp.Remove(o);

                // Try to Convert to the parameter type
                TryChangeType(o, parameter.ParameterType, out o);

                result.Add(o);
            }

            return result.ToArray();
        }

        private static bool IsParameterTypeSimilar(ParameterInfo parameter, object parameterValue)
        {
            var parameterType = parameter.ParameterType;
            if (parameterType == typeof(object))
                return true;

            if (parameterValue is null)
            {
                // Check if the parameter type is nullable
                bool isNullable = !parameterType.IsValueType || Nullable.GetUnderlyingType(parameterType) != null;
                return isNullable;
            }

            var valueType = parameterValue.GetType();
            bool canAssign = parameterType.IsAssignableFrom(valueType);

            System.Diagnostics.Debug.WriteLine($"{canAssign}:\t {valueType} >> {parameterValue.GetType()}");

            if (canAssign)
            {
                return true;
            }

            return TryChangeType(parameterValue, parameterType, out _);
        }

        private static bool TryChangeType(object value, Type type, out object result)
        {
            try
            {
                result = Convert.ChangeType(value, type);
                return true;
            }
            catch { }
            result = value;
            return false;
        }

        #endregion
    }
}
