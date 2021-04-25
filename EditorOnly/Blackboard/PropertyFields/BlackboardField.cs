using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public class BlackboardField : VisualElement
    {
        public static BlackboardField CreateNew(Type t, PropertyFieldBinder binder, 
            Action updateViewAction, VisualElement svParent)
        {
            object newItem;
            if (t == typeof(string))
            {
                newItem = "";
            }
            else if (t.IsEnum)
            {
                Debug.LogError("Enums not currently supported for blackboard fields!");
                return null;
            }
            else
            {
                newItem = Activator.CreateInstance(t);
            }
            var pv = new PropertyValue(newItem, t.Name + Guid.NewGuid());
            var fieldKey = t.Name + Guid.NewGuid();
            binder.boundDitionary.Add(fieldKey, pv);
            return new BlackboardField(fieldKey, t,
                pv, binder, updateViewAction, svParent);
        }

        public static BlackboardField LoadField(string fieldKey, Type t, PropertyValue someObject,
            PropertyFieldBinder binder, Action updateViewAction, VisualElement svParent)
        {
            return new BlackboardField(fieldKey, t,
                someObject, binder, updateViewAction, svParent);
        }
        
        protected BlackboardField(string fieldKey, Type t, PropertyValue someObject, 
            PropertyFieldBinder binder, Action updateViewAction, VisualElement svParent)
        {
            if (fieldKey == null)
            {
                fieldKey = t.Name + Guid.NewGuid();
                binder.boundDitionary.Add(fieldKey, someObject);
            }
            var field = BlackboardPropertyFieldFactory.Create(t, someObject, binder, fieldKey);

            var labelName = ObjectNames.NicifyVariableName(t.Name);
            var fv = new Foldout {text = labelName};
            var textField = new TextField();
            var deleteBtn = new Button {text = "-"};
            textField.SetValueWithoutNotify(someObject.lookupKey);

            textField.userData = field;
            deleteBtn.userData = field;
            textField.RegisterValueChangedCallback(e =>
            {
                var lookupKey = field.userData as string;
                if (string.IsNullOrEmpty(e.newValue))
                    return;
                binder.boundDitionary[lookupKey].lookupKey = e.newValue;
            });

            deleteBtn.clicked += () =>
            {
                if (!(textField.userData is BindableElement relatedField))
                {
                    return;
                }
                
                updateViewAction.Invoke();
            };

            var m = fv.Q<VisualElement>("unity-checkmark");
            deleteBtn.ClearClassList();
            deleteBtn.style.position = new StyleEnum<Position>(Position.Absolute);
            deleteBtn.AddToClassList("--delBtn");
            
            m.parent.style.flexShrink = 1;
            m.parent.style.flexGrow = 1;
            m.parent.Add(deleteBtn);
            fv.Add(textField);
            fv.Add(field);
            fv.style.maxWidth = 265;
            
            RegisterCallback<GeometryChangedEvent>(e =>
            {
                fv.style.width = svParent.resolvedStyle.width;
                deleteBtn.style.left = (svParent.resolvedStyle.width)-30;
            });

            svParent.RegisterCallback<GeometryChangedEvent>(e =>
            {
                fv.style.width = svParent.resolvedStyle.width;
                deleteBtn.style.left = (svParent.resolvedStyle.width)-30;
            });

            field.AddToClassList("dataBlackboard-item");
            field.AddToClassList("dataBlackboard-field");
            field.AddToClassList("dataBlackboard-valuefield");
            fv.AddToClassList("dataBlackboard-item");
            fv.AddToClassList("dataBlackboard-foldout");
            textField.AddToClassList("dataBlackboard-item");
            textField.AddToClassList("dataBlackboard-field");
            textField.AddToClassList("dataBlackboard-textfield");

            Add(fv);
        }
    }
}