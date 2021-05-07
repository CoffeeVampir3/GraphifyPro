using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class ExposedProperty
    {
        [OdinSerialize, ValueDropdown("GetGeneratedPropertyTypes")]
        protected Type propertyClass;

        private IEnumerable GetGeneratedPropertyTypes() {
            var types = TypeCache.GetTypesDerivedFrom<RuntimeProperties>();
            ValueDropdownList<Type> typeList = new();
            foreach (var item in types)
            {
                typeList.Add(item.Name, item);
            }
            
            return typeList;
        }

        public abstract void Validate();
    }
    
    [Serializable]
    public class ExposedProperty<T> : ExposedProperty
    {
        //TODO:: Avoid runtime reflection by caching getter/setter names instead.
        //TODO:: Odin Dependency
        [SerializeField, ValueDropdown("GetPropertiesFromGeneratedType")]
        private string propertyName;

        private IEnumerable GetPropertiesFromGeneratedType()
        {
            if (propertyClass == null)
                return null;
            var gets = propertyClass.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            return (from m in gets where m.PropertyType == typeof(T) select m.Name).ToList();
        }

        public override void Validate()
        {
            var prop = propertyClass.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (prop != null) return;
            Debug.LogError("Couldn't find property which had been named " + propertyName + " was it deleted?");
        }

        private Type FindGeneratedClassTypeIfExists(string generatedClassName) {
            var types = TypeCache.GetTypesDerivedFrom<RuntimeProperties>();
            return types.FirstOrDefault(item => item.Name == generatedClassName);
        }

        private bool cached = false;
        private Func<T> cachedGetter;
        private Action<T> cachedSetter;
        private static readonly Type getFuncPattern = typeof(Func<T>);
        private static readonly Type setFuncPattern = typeof(Action<T>);
        public void Cache()
        {
            if (cached)
                return;
            cached = true;
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
            targetEvaluator.Testing_SetCurrentGraphContext();
            Cache();
            return cachedGetter.Invoke();
        }

        public void Set(GraphEvaluator targetEvaluator, T newValue)
        {
            targetEvaluator.Testing_SetCurrentGraphContext();
            Cache();
            cachedSetter.Invoke(newValue);
        }
    }
}