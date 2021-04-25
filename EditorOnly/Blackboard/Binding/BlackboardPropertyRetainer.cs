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
        [OdinSerialize, NonSerialized] 
        private object[] serializedObjects;
        [OdinSerialize, NonSerialized] 
        private string[] serializedTitles;
        [OdinSerialize, NonSerialized] 
        private string[] serializedKeys;

        [Button]
        public void Clear()
        {
            serializedObjects = null;
            serializedTitles = null;
            serializedKeys = null;
        }

        public void Serialize(Dictionary<string, PropertyValue> data)
        {
            var objects = new List<object>(100);
            var objectKeys = new List<string>(100);
            var titles = new List<string>(100);
            var hashedKeys = new HashSet<string>();
            var hashedTitles = new HashSet<string>();

            foreach (var item in data)
            {
                if (hashedKeys.Contains(item.Key))
                {
                    Debug.LogWarning("Attempted to serialize multiple of the same key named: " + item.Key);
                    continue;
                }

                if (hashedTitles.Contains(item.Value.lookupKey))
                {
                    Debug.LogError("Attempted to serialize multiple objects with the same name: " + 
                                   item.Value.lookupKey + " the item with value: " + item.Value.initialValue + 
                                   " has been skipped! Choose a unique name for this item or it will not be saved!");
                    continue;
                }
                hashedKeys.Add(item.Key);
                hashedTitles.Add(item.Value.lookupKey);
                switch (item.Value)
                {
                    default:
                        objects.Add(item.Value.initialValue);
                        objectKeys.Add(item.Key);
                        titles.Add(item.Value.lookupKey);
                        continue;
                }
            }
            
            EditorUtility.SetDirty(this);

            serializedObjects = objects.ToArray();
            serializedKeys = objectKeys.ToArray();
            serializedTitles = titles.ToArray();

            EditorUtility.SetDirty(this);
        }

        public Dictionary<string, PropertyValue> Deserialize()
        {
            Dictionary<string, PropertyValue> data = new();
            for (var i = 0; i < (serializedObjects?.Length ?? -1); i++)
            {
                var item = serializedObjects[i];
                var itemTitle = serializedTitles[i];
                var key = serializedKeys[i];
                data.Add(key, new PropertyValue(item, itemTitle));
            }

            return data;
        }
    }
}