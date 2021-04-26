using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Vampire.Runtime
{
    [CreateAssetMenu]
    public class RuntimeGraphBlueprint : SerializedScriptableObject
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