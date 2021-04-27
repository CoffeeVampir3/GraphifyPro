using System;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class AntiAllocationWrapper
    {
        internal abstract void SetValue(object o);

        internal abstract AntiAllocationWrapper CloneWrapper();
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
        }

        internal override AntiAllocationWrapper CloneWrapper()
        {
            var clonedWrapper = new AntiAllocationWrapper<T>();
            clonedWrapper.SetValue(item);
            return clonedWrapper;
        }
    }
}