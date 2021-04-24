using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public class PropertyFieldBinder
    {
        public Dictionary<string, object> boundDitionary;
        private BlackboardPropertyRetainer retainer;

        public void TestingOnly()
        {
            if (boundDitionary != null)
                return;
            var assets = AssetDatabase.FindAssets("t:" + nameof(BlackboardPropertyRetainer));
            var assetGuid = assets.FirstOrDefault();
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            retainer = AssetDatabase.LoadAssetAtPath<BlackboardPropertyRetainer>(assetPath);
            if (retainer == null) return;
            boundDitionary = retainer.Deserialize();
        }

        public static string GenerateUniqueFieldIdentifier(object obj)
        {
            string typeName = obj.GetType().Name;
            string guid = Guid.NewGuid().ToString();
            return typeName + guid;
        }

        private void UpdateSerializedModel()
        {
            retainer.Serialize(boundDitionary);
        }

        public void BindingResolver<ArgType, FieldType>(FieldType field, object obj, string key)
        where FieldType : BaseField<ArgType>
        {
            TestingOnly();
            field.userData = key;
            
            field.RegisterValueChangedCallback(e =>
            {
                string userDataLookupKey = field.userData as string;
                if (string.IsNullOrEmpty(userDataLookupKey))
                    return;
                boundDitionary[userDataLookupKey] = e.newValue;
                UpdateSerializedModel();
            });
            
            if (boundDitionary.TryGetValue(key, out var keyedValue))
            {
                if (keyedValue is ArgType fieldTypedArg)
                {
                    field.SetValueWithoutNotify(fieldTypedArg);
                    return;
                }
            }

            boundDitionary.Add(key, obj);
            UpdateSerializedModel();
            if (obj == null) return;
            field.SetValueWithoutNotify((ArgType) obj);
        }
    }
}