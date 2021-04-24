using System;
using UnityEngine;
using UnityEngine.UIElements;
using Vampire.Binding;

namespace Vampire.Graphify.EditorOnly
{
    public class CustomBlackboard : VisualElement
    {
        private readonly ScrollView scrollView = new();
        public CustomBlackboard()
        {
            style.width = 200;
            style.height = 200;
            style.left = 0;
            style.height = new StyleLength(Length.Percent(100));
            style.position = new StyleEnum<Position>(Position.Absolute);
            name = "Woawblackboard";
            style.backgroundColor = Color.red;

            PropertyFieldBinder pfbinder = new();
            pfbinder.TestingOnly();

            if (pfbinder.boundDitionary.Count == 0)
            {
                foreach (var n in PropertyFieldFactory.GetDrawableTypes())
                {
                    if (n.IsEnum || n == typeof(string))
                        continue;

                    var thing = Activator.CreateInstance(n);
                    scrollView.Add(PropertyFieldFactory.Create(n, thing, pfbinder));
                }
            }
            else
            {
                foreach (var m in pfbinder.boundDitionary)
                {
                    scrollView.Add(PropertyFieldFactory.Create(m.Value.GetType(), m.Value, pfbinder, m.Key));
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