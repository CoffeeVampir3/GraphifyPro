using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class ExposedProperty
    {
    }

    [Serializable]
    public class ExposedProperty<PropertyClassType, T> : ExposedProperty
        where PropertyClassType : RuntimeProperties
    {
        [SerializeField, ValueDropdown("GetPropertiesFromGeneratedType")]
        private string propertyName;
        
        protected bool cached = false;
        protected Func<T> cachedGetter;
        protected Action<T> cachedSetter;
        protected static readonly Type getFuncPattern = typeof(Func<T>);
        protected static readonly Type setFuncPattern = typeof(Action<T>);

        private IEnumerable GetPropertiesFromGeneratedType()
        {
            var propertyClass = typeof(PropertyClassType);

            var gets = propertyClass.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            return (from m in gets where m.PropertyType == typeof(T) select m.Name).ToList();
        }

        protected void Cache()
        {
            if (cached)
                return;
            cached = true;
            var propertyClass = typeof(PropertyClassType);
            var prop = propertyClass.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (prop == null)
            {
                Debug.LogError("Couldn't find property which had been named " + propertyName + " was it deleted?");
                return;
            }
            
            cachedGetter = (Func<T>)Delegate.CreateDelegate(getFuncPattern, propertyClass, prop.GetMethod.Name);
            cachedSetter = (Action<T>)Delegate.CreateDelegate(setFuncPattern, propertyClass, prop.SetMethod.Name);
            
            if (cachedGetter == null)
            {
                Debug.LogError("Failed to build getter for graphify property " + prop.Name);
            }
            if (cachedSetter == null)
            {
                Debug.LogError("Failed to build setter for graphify property " + prop.Name);
            }
        }

        public T Get(GraphEvaluator targetEvaluator)
        {
            targetEvaluator.SetGraphContext();
            Cache();
            return cachedGetter.Invoke();
        }

        public void Set(GraphEvaluator targetEvaluator, T newValue)
        {
            targetEvaluator.SetGraphContext();
            Cache();
            cachedSetter.Invoke(newValue);
        }
    }
}