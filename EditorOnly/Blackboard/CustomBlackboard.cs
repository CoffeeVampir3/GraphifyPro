using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public class CustomBlackboard : VisualElement
    {
        private readonly ScrollView scrollView = new();
        public CustomBlackboard()
        {
            style.width = 275;
            style.height = 200;
            style.left = 0;
            style.height = new StyleLength(Length.Percent(100));
            style.position = new StyleEnum<Position>(Position.Absolute);
            name = "Woawblackboard";
            style.backgroundColor = new Color(.22f, .22f, .22f, 1);

            PropertyFieldBinder pfbinder = new();
            pfbinder.TestingOnly();

            if (pfbinder.boundDitionary.Count == 0)
            {
                foreach (var n in PropertyFieldFactory.GetDrawableTypes())
                {
                    if (n.IsEnum || n == typeof(string))
                        continue;

                    var thing = Activator.CreateInstance(n);
                    scrollView.Add(new BlackboardField(null, n, thing, pfbinder, null, scrollView));
                }
            }
            else
            {
                foreach (var m in pfbinder.boundDitionary)
                {
                    scrollView.Add(new BlackboardField(m.Key, m.Value.GetType(), m.Value, pfbinder, null, scrollView));
                } 
            }

            Add(scrollView);
            RegisterCallback<GeometryChangedEvent>(OnGeoChange);
        }
        
        private void OnGeoChange(GeometryChangedEvent geo)
        {
            scrollView.style.minHeight = resolvedStyle.height;
        }
    }
}