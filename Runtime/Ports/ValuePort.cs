using System;
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
        
        public T LocalValue
        {
            get
            {
                if (portId < 0)
                    return default;
                var val = CurrentEvaluation.currentGraph.values[portId];
                return val switch
                {
                    AntiAllocationWrapper<T> allocWrap => allocWrap.item,
                    T typedVal => typedVal,
                    _ => default
                };
            }
            set
            {
                if (portId < 0)
                    return;
                var val = CurrentEvaluation.currentGraph.values[portId];
                if (val is AntiAllocationWrapper<T> allocWrap)
                {
                    allocWrap.SetValue(value);
                    return;
                }
                CurrentEvaluation.currentGraph.values[portId] = value;
            }
        }
        
        public bool TryGetValue(Link link, out T val)
        {
            var item = CurrentEvaluation.currentGraph.values[link.toPortIndex];
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
        
        public bool TryGetValueAs<SomeType>(Link link, out SomeType val)
        {
            var item = CurrentEvaluation.currentGraph.values[link.toPortIndex];
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