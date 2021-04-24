using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public class BlackboardField : VisualElement
    {
        public BlackboardField(string fieldKey, Type t, 
            object someObject, PropertyFieldBinder binder, Action updateViewAction, VisualElement svParent)
        {
            var field = PropertyFieldFactory.Create(t, someObject, binder, fieldKey);

            var labelName = ObjectNames.NicifyVariableName(t.Name);
            var fv = new Foldout {text = labelName};
            var textField = new TextField();
            var deleteBtn = new Button {text = "-"};
            textField.SetValueWithoutNotify(fieldKey);

            textField.userData = field;
            deleteBtn.userData = field;
            textField.RegisterValueChangedCallback(e =>
            {

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