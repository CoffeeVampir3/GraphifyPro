using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Vampire.Runtime
{
    public abstract class RuntimeGraphBlueprint : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize]
        public RuntimeNode[] nodes;
        [NonSerialized, OdinSerialize] 
        public object[] initializationValues;
        [NonSerialized, OdinSerialize] 
        public string[] initializationNames;
        [NonSerialized, OdinSerialize] 
        public PropertyDictionary localProperties;
        //
        public RuntimeGraph CreateRuntimeGraph()
        {
            return new(this);
        }
    } 
}