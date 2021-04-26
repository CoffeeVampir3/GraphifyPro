using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public class SerializedBlackboard
    {
        [OdinSerialize, NonSerialized] 
        private object[] serializedObjects;
        [OdinSerialize, NonSerialized] 
        private string[] serializedKeys;
        
        public void Serialize(Dictionary<string, object> data)
        {
            var objects = new List<object>(100);
            var objectKeys = new List<string>(100);
            var hashedKeys = new HashSet<string>();

            foreach (var item in data)
            {
                if (hashedKeys.Contains(item.Key))
                {
                    Debug.LogWarning("Attempted to serialize multiple of the same key named: " + item.Key);
                    continue;
                }
                hashedKeys.Add(item.Key);
                objects.Add(item.Value);
                objectKeys.Add(item.Key);
            }
            
            serializedObjects = objects.ToArray();
            serializedKeys = objectKeys.ToArray();
        }

        public Dictionary<string, object> Deserialize()
        {
            Dictionary<string, object> data = new();
            if (serializedObjects.Length == 0)
                return data;
            
            for (var i = 0; i < serializedObjects?.Length; i++)
            {
                var item = serializedObjects[i];
                var key = serializedKeys[i];
                data.Add(key, item);
            }

            return data;
        }
    }
}