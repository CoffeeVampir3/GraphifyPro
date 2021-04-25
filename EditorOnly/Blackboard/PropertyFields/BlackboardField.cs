using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
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

            var propertyName = t.FriendlyName() + " " + UnityEngine.Random.Range(0, 99);
            var pv = new PropertyValue(newItem, propertyName);
            var fieldKey = t.Name + Guid.NewGuid();
            binder.boundDitionary.Add(fieldKey, pv);
            return new BlackboardField(fieldKey, t,
                pv, binder, updateViewAction, svParent);
        }

        public static BlackboardField LoadField(string fieldKey, Type t, PropertyValue someObject,
            PropertyFieldBinder binder, Action updateViewAction, VisualElement svParent)
        {
            return new(fieldKey, t,
                someObject, binder, updateViewAction, svParent);
        }

        private BlackboardField(string fieldKey, Type t, PropertyValue someObject, 
            PropertyFieldBinder binder, Action updateViewAction, VisualElement svParent)
        {
            if (fieldKey == null)
            {
                fieldKey = t.Name + Guid.NewGuid();
                binder.boundDitionary.Add(fieldKey, someObject);
            }
            var field = BlackboardPropertyFieldFactory.Create(t, someObject, binder, fieldKey);
            var fv = new Foldout {text = t.FriendlyName()};
            var textField = new TextField();
            var deleteBtn = new Button {text = "-"};
            textField.SetValueWithoutNotify(someObject.lookupKey);

            textField.userData = field;
            deleteBtn.userData = field;
            textField.RegisterValueChangedCallback(e =>
            {
                if (string.IsNullOrEmpty(e.newValue) || field.userData is not string lookupKey)
                    return;
                binder.boundDitionary[lookupKey].lookupKey = e.newValue;
                binder.UpdateSerializedModel();
            });

            deleteBtn.clicked += () =>
            {
                if (!(textField.userData is BindableElement relatedField) ||
                    relatedField.userData is not string relatedFieldKey)
                {
                    return;
                }

                binder.boundDitionary.Remove(relatedFieldKey);
                updateViewAction.Invoke();
            };

            var m = fv.Q<VisualElement>("unity-checkmark");
            deleteBtn.ClearClassList();
            deleteBtn.style.position = new StyleEnum<Position>(Position.Absolute);
            deleteBtn.AddToClassList("--delBtn");
            
            m.parent.Add(deleteBtn);
            fv.Add(textField);
            fv.Add(field);

            RegisterCallback<GeometryChangedEvent>(_ =>
            {
                fv.style.width = svParent.resolvedStyle.width;
                deleteBtn.style.left = (svParent.resolvedStyle.width)-27;
            });

            svParent.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                fv.style.width = svParent.resolvedStyle.width;
                deleteBtn.style.left = (svParent.resolvedStyle.width)-27;
            });

            field.AddToClassList("dataBlackboard-item");
            field.AddToClassList("dataBlackboard-field");
            field.AddToClassList("dataBlackboard-valuefield");
            fv.AddToClassList("dataBlackboard-item");
            fv.AddToClassList("dataBlackboard-foldout");
            textField.AddToClassList("dataBlackboard-item");
            textField.AddToClassList("dataBlackboard-field");
            textField.AddToClassList("dataBlackboard-textfield");
            AddToClassList("blackboardField");

            Add(fv);
        }
    }
}