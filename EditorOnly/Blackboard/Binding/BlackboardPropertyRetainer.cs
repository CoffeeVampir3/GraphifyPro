using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Vampire.Binding
{
    [CreateAssetMenu(menuName = "Blackboard")]
    public class BlackboardPropertyRetainer : SerializedScriptableObject
    {
        [OdinSerialize] 
        private object[] serializedObjects;
        [OdinSerialize, NonSerialized] 
        private string[] serializedKeys;
        [OdinSerialize, NonSerialized] 
        private UnityEngine.Object[] serializedUnityObjects;
        [OdinSerialize, NonSerialized] 
        private string[] serializedUnityObjectKeys;

        public void Serialize(Dictionary<string, object> data)
        {
            var objects = new List<object>(16);
            var objectKeys = new List<string>(16);
            var unityObjects = new List<UnityEngine.Object>(16);
            var unityObjectKeys = new List<string>(16);
            var hashedKeys = new HashSet<string>();

            foreach (var item in data)
            {
                if (hashedKeys.Contains(item.Key))
                {
                    Debug.LogWarning("Attempted to serialize multiple of the same key named: " + item.Key);
                    continue;
                }
                hashedKeys.Add(item.Key);
                switch (item.Value)
                {
                    case UnityEngine.Object uObj:
                        unityObjects.Add(uObj);
                        unityObjectKeys.Add(item.Key);
                        break;
                    default:
                        objects.Add(item.Value);
                        objectKeys.Add(item.Key);
                        continue;
                }
            }
            
            EditorUtility.SetDirty(this);

            serializedObjects = objects.ToArray();
            serializedKeys = objectKeys.ToArray();
            
            serializedUnityObjects = unityObjects.ToArray();
            serializedUnityObjectKeys = unityObjectKeys.ToArray();

            EditorUtility.SetDirty(this);
        }

        public Dictionary<string, object> Deserialize()
        {
            Dictionary<string, object> data = new();
            for (var i = 0; i < (serializedObjects?.Length ?? -1); i++)
            {
                var item = serializedObjects[i];
                var key = serializedKeys[i];
                data.Add(key, item);
            }
            for (var i = 0; i < (serializedUnityObjects?.Length ?? -1); i++)
            {
                var item = serializedUnityObjects[i];
                var key = serializedUnityObjectKeys[i];
                data.Add(key, item);
            }

            return data;
        }
    }
}