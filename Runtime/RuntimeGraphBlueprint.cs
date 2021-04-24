using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Vampire.Graphify.Runtime
{
    [CreateAssetMenu]
    public class RuntimeGraphBlueprint : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize]
        public RuntimeNode[] nodes;
        [NonSerialized, OdinSerialize] 
        public object[] initializationValues;
        //
        public RuntimeGraph CreateRuntimeGraph()
        {
            return new(this);
        }
    } 
}