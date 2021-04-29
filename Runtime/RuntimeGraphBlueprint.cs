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
        public SerializedBlackboard serializedBlackboard = new();
        //
        public RuntimeGraph CreateRuntimeGraph()
        {
            return new(this);
        }
    } 
}