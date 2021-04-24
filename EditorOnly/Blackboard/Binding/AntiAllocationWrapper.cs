using System;
using UnityEngine;

namespace Vampire.Binding
{
    [Serializable]
    public abstract class AntiAllocationWrapper
    {
        internal abstract void SetValue(object o);
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
    }
}