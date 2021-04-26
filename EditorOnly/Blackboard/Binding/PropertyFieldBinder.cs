using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public class PropertyFieldBinder
    {
        public readonly Dictionary<string, PropertyValue> boundDictionary;
        private readonly BlackboardPropertyRetainer retainer;

        public PropertyFieldBinder(BlackboardPropertyRetainer retainer)
        {
            this.retainer = retainer;
            boundDictionary = retainer.Deserialize();
        }

        public static string GenerateUniqueFieldIdentifier(object obj)
        {
            string typeName = obj.GetType().Name;
            string guid = Guid.NewGuid().ToString();
            return typeName + guid;
        }

        public void UpdateSerializedModel()
        {
            retainer.Serialize(boundDictionary);
        }

        public void BindingResolver<ArgType, FieldType>(FieldType field, object obj, string key)
        where FieldType : BaseField<ArgType>
        {
            field.userData = key;
            
            field.RegisterValueChangedCallback(e =>
            {
                string userDataLookupKey = field.userData as string;
                if (string.IsNullOrEmpty(userDataLookupKey))
                    return;
                boundDictionary[userDataLookupKey].initialValue = e.newValue;
                UpdateSerializedModel();
            });
            
            if (boundDictionary.TryGetValue(key, out var keyedValue))
            {
                if (keyedValue.initialValue is ArgType fieldTypedArg)
                {
                    field.SetValueWithoutNotify(fieldTypedArg);
                    return;
                }
            }
            
            UpdateSerializedModel();
            field.SetValueWithoutNotify((ArgType) obj);
        }
    }
}