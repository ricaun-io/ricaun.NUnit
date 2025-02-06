using ricaun.NUnit.Extensions;
using System;

namespace ricaun.NUnit.Services
{
    internal class TypeInstance
    {
        public TypeInstance(Type type, object[] parameters = null)
        {
            Type = type;
            Parameters = parameters ?? new object[0];
        }

        public Type Type { get; }
        public object[] Parameters { get; }
        public string FullName => ToString();

        // implicit 
        public static implicit operator Type(TypeInstance typeInstance)
        {
            return typeInstance.Type;
        }

        override public string ToString()
        {
            return $"{Type.FullName}{Parameters.ToArgumentName()}";
        }
    }
}
