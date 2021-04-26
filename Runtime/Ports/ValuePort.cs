﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class ValuePort : RuntimeBasePort
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetValueDirect(Link link, RuntimeGraph graph) 
            => graph.values[link.fromPortIndex];
    }

    [Serializable]
    public class ValuePort<T> : ValuePort, IPortWithValue<T>
    {
        [SerializeField] 
        protected T portValue;
        object IPortWithValue.GetInitValue() => portValue;
        
        public bool TryGetValue(Link link, RuntimeGraph graph, out T val)
        {
            var item = graph.values[link.toPortIndex];
            switch (item)
            {
                case AntiAllocationWrapper<T> allocWrap:
                    val = allocWrap.item;
                    return true;
                case T typedItem:
                    val = typedItem;
                    return true;
                default:
                    val = default;
                    return false;
            }
        }
        
        public bool TryGetValueAs<SomeType>(Link link, RuntimeGraph graph, out SomeType val)
        {
            var item = graph.values[link.toPortIndex];
            switch (item)
            {
                case AntiAllocationWrapper<SomeType> allocWrap:
                    val = allocWrap.item;
                    return true;
                case SomeType typedItem:
                    val = typedItem;
                    return true;
                default:
                    val = default;
                    return false;
            }
        }
    }
}