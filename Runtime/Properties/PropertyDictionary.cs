using System;
using System.Collections.Generic;
using Sirenix.Serialization;

namespace Vampire.Runtime
{
    /// <summary>
    /// A dictionary of graph properties.
    /// </summary>
    [Serializable]
    public class PropertyDictionary : IUnsafePropertyDictionary
    {
        [OdinSerialize] 
        internal readonly Dictionary<string, object> properties;
        public IReadOnlyDictionary<string, object> Properties => properties;

        public PropertyDictionary()
        {
            properties = new();
        }

        /// <summary>
        /// Sets or creates a property with the given key/value.
        /// </summary>
        public void Set<T>(string key, T value)
        {
            if (value != null && value.GetType().IsValueType)
            {
                if (!properties.TryGetValue(key, out var item))
                {
                    var allocWrapper = new AntiAllocationWrapper<T> {item = value};
                    properties.Add(key, allocWrapper);
                    return;
                }
                if (item is AntiAllocationWrapper<T> allocWrap)
                {
                    allocWrap.item = value;
                    return;
                }
            }

            if (properties.ContainsKey(key))
            {
                properties[key] = value;
                return;
            }

            properties.Add(key, value);
        }

        /// <summary>
        /// Tries to get the value of the property as the given type.
        /// </summary>
        public bool TryGetValueAs<T>(string key, out T value)
        {
            if(!properties.TryGetValue(key, out var inst))
            {
                value = default;
                return false;
            }
            
            switch (inst)
            {
                case ValueKeyedProperty {valueKey: < 0}:
                    value = default;
                    return false;
                case ValueKeyedProperty vk:
                {
                    var item = RuntimeGraph.current.values[vk.valueKey];
                    switch (item)
                    {
                        case AntiAllocationWrapper<T> allocWrap:
                            value = allocWrap.item;
                            return true;
                        case T typedValueKeyItem:
                            value = typedValueKeyItem;
                            return true;
                        default:
                            value = default;
                            return false;
                    }
                }
                case AntiAllocationWrapper<T> allocWrap:
                    value = allocWrap.item;
                    return true;
                case T typedInst:
                    value = typedInst;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }
        
        internal PropertyDictionary(Dictionary<string, object> propertyDict)
        {
            properties = propertyDict;
        }
        
        private static void DeepCopyValueWrappers(ref PropertyDictionary targetDict)
        {
            foreach (var item in targetDict.properties)
            {
                if (item.Value is AntiAllocationWrapper wrapper)
                {
                    targetDict.properties[item.Key] = wrapper.CloneWrapper();
                }
            }
        }
        
        #region Internals
        
        internal PropertyDictionary Copy()
        {
            PropertyDictionary newDict = new PropertyDictionary(new Dictionary<string, object>(properties));
            DeepCopyValueWrappers(ref newDict);
            return newDict;
        }

        internal void Add(string key, object value,
            IReadOnlyDictionary<string, short> valueKeyedItems)
        {
            if (valueKeyedItems.TryGetValue(key, out var valueKey))
            {
                var vk = new ValueKeyedProperty(valueKey);
                properties.Add(key, vk);
                return; 
            }
            if (value != null && value.GetType().IsValueType)
            {
                properties[key] = AntiAllocationWrapper.CreateValueTypeWrapper(value);
                return;
            }
            properties.Add(key, value);
        }
        
        #endregion

        #region Unsafe Implementation

        //Optimized unchecked operations used by the property code gen.

        /// <summary>
        /// Used for generated code only, this does not handle all cases and will fail
        /// for non-generated code.
        /// </summary>
        T IUnsafePropertyDictionary.Unsafe_GetDirect<T>(string key)
            => properties[key] switch
            {
                AntiAllocationWrapper<T> allocWrap => allocWrap.item,
                T typedInst => typedInst,
                _ => default
            };
        
        /// <summary>
        /// Used for generated code only, this does not handle all cases and will fail
        /// for non-generated code.
        /// </summary>
        void IUnsafePropertyDictionary.Unsafe_SetDirect(string key, object o)
        => properties[key] = o;

        /// <summary>
        /// Used for generated code only, this does not handle all cases and will fail
        /// for non-generated code.
        /// </summary>
        void IUnsafePropertyDictionary.Unsafe_SetWrapped<T>(string key, T wrappedItem)
        {
            if (!properties.TryGetValue(key, out var item)) return;
            if (item is AntiAllocationWrapper<T> allocWrap)
                allocWrap.item = wrappedItem;
        }

        #endregion
    }
}