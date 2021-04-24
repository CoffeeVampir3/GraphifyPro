using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire.Binding
{
    public static class WrapperFactory
    {
        //No need to clear, consistent for every value type those don't change.
        private static readonly Dictionary<Type, Type> typeToWrapperType = new();
        public static AntiAllocationWrapper CreateValueTypeWrapper(object value)
        {
            var valueType = value.GetType();
            if (!typeToWrapperType.TryGetValue(valueType, out var wrapperType))
            {
                wrapperType = typeof(AntiAllocationWrapper<>).MakeGenericType(valueType);
                typeToWrapperType.Add(valueType, wrapperType);
            }

            if (Activator.CreateInstance(wrapperType) is not AntiAllocationWrapper wrappedValue)
            {
                Debug.LogError("System was unable to create a valid allocation wrapper for type: " + valueType + 
                               " this is a critical bug, please report this.");
                return null;
            }
            wrappedValue.SetValue(value);
            return wrappedValue;
        }

        internal static bool ShouldBeWrapped(object value)
        {
            return value != null && value.GetType().IsValueType;
        }
    }
}