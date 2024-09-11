using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// AttributeService
    /// </summary>
    internal class AttributeService
    {
        #region Attribute

        /// <summary>
        /// AnyAttributeName
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public bool AnyAttributeName(ICustomAttributeProvider customAttributeProvider, params Type[] types)
        {
            var names = types.Select(e => e.Name);
            return customAttributeProvider
                .GetCustomAttributes(true)
                .Any(e => names.Contains(e.GetType().Name));
        }

        /// <summary>
        /// AnyAttributeName
        /// </summary>
        /// <typeparam name="TCustomAttributeType"></typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public bool AnyAttributeName<TCustomAttributeType>(ICustomAttributeProvider customAttributeProvider) where TCustomAttributeType : Attribute
        {
            return customAttributeProvider
                .GetCustomAttributes(true)
                .Any(e => e.GetType().Name.Equals(typeof(TCustomAttributeType).Name));
        }

        /// <summary>
        /// AnyAttribute
        /// </summary>
        /// <typeparam name="TCustomAttributeType"></typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public bool AnyAttribute<TCustomAttributeType>(ICustomAttributeProvider customAttributeProvider) where TCustomAttributeType : Attribute
        {
            return GetAttributes<TCustomAttributeType>(customAttributeProvider).Any();
        }

        /// <summary>
        /// HasAttribute
        /// </summary>
        /// <typeparam name="TCustomAttributeType"></typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool HasAttribute<TCustomAttributeType>(ICustomAttributeProvider customAttributeProvider, Func<TCustomAttributeType, bool> func) where TCustomAttributeType : Attribute
        {
            var attribute = GetAttributes<TCustomAttributeType>(customAttributeProvider)
                .FirstOrDefault();

            if (attribute is null)
                return false;

            return func.Invoke(attribute);
        }

        /// <summary>
        /// HasAttributeName
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public bool HasAttributeName(ICustomAttributeProvider customAttributeProvider, Type attributeType)
        {
            return customAttributeProvider.GetCustomAttributes(true)
                .Any(e => e.GetType().Name.Equals(attributeType.Name));
        }

        /// <summary>
        /// HasAttribute
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public bool HasAttribute(ICustomAttributeProvider customAttributeProvider, Type attributeType)
        {
            return customAttributeProvider.GetCustomAttributes(attributeType, true).Any();
        }

        /// <summary>
        /// GetAttributes
        /// </summary>
        /// <typeparam name="TCustomAttributeType"></typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public IEnumerable<TCustomAttributeType> GetAttributes<TCustomAttributeType>(ICustomAttributeProvider customAttributeProvider) where TCustomAttributeType : Attribute
        {
            return customAttributeProvider
                .GetCustomAttributes(typeof(TCustomAttributeType), true)
                .OfType<TCustomAttributeType>();
        }

        /// <summary>
        /// TryGetAttributes
        /// </summary>
        /// <typeparam name="TCustomAttributeType"></typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="customAttributeTypes"></param>
        /// <returns></returns>
        public bool TryGetAttributes<TCustomAttributeType>(ICustomAttributeProvider customAttributeProvider,
            out IEnumerable<TCustomAttributeType> customAttributeTypes)
            where TCustomAttributeType : Attribute
        {
            customAttributeTypes = GetAttributes<TCustomAttributeType>(customAttributeProvider);
            return customAttributeTypes.Any();
        }

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <typeparam name="TCustomAttributeType"></typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public TCustomAttributeType GetAttribute<TCustomAttributeType>(ICustomAttributeProvider customAttributeProvider) where TCustomAttributeType : Attribute
        {
            return customAttributeProvider
                .GetCustomAttributes(typeof(TCustomAttributeType), true)
                .OfType<TCustomAttributeType>()
                .FirstOrDefault();
        }

        /// <summary>
        /// TryGetAttribute
        /// </summary>
        /// <typeparam name="TCustomAttributeType"></typeparam>
        /// <param name="customAttributeProvider"></param>
        /// <param name="customAttributeType"></param>
        /// <returns></returns>
        public bool TryGetAttribute<TCustomAttributeType>(
                ICustomAttributeProvider customAttributeProvider,
                out TCustomAttributeType customAttributeType
            )
            where TCustomAttributeType : Attribute
        {
            customAttributeType = GetAttribute<TCustomAttributeType>(customAttributeProvider);
            return customAttributeType is not null;
        }

        #endregion
    }
}
