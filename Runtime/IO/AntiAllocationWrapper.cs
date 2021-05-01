using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class AntiAllocationWrapper
    {
        internal abstract void SetValue(object o);

        internal abstract AntiAllocationWrapper CloneWrapper();
        
        //No need to clear, since these types are consistent. We can keep these cached 4ever
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
    }
    
    [Serializable]
    public class AntiAllocationWrapper<T> : AntiAllocationWrapper
    {
        [SerializeField]
        public T item;

        internal override void SetValue(object o)
        {
            if(o is T valItem)
            {
                item = valItem;
            }
            else
            {
                Debug.LogError("Attempted to set the value of an allocation wrapper to type " + o.GetType() +
                               " which is not this wrappers type!");
            }
        }

        internal override AntiAllocationWrapper CloneWrapper()
        {
            var clonedWrapper = new AntiAllocationWrapper<T>();
            clonedWrapper.SetValue(item);
            return clonedWrapper;
        }
    }
}