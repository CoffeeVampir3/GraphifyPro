using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public class CustomBlackboard : VisualElement
    {
        private readonly ScrollView scrollView = new();
        private readonly PropertyFieldBinder binder = new();
        public CustomBlackboard()
        {
            name = "customBlackboard";

            binder.TestingOnly();
            if (binder.boundDitionary.Count == 0)
            {
                foreach (var n in BlackboardPropertyFieldFactory.GetDrawableTypes())
                {
                    scrollView.Add(BlackboardField.CreateNew(n, binder, RebuildFromBinder, scrollView));
                }
            }
            else
            {
                RebuildFromBinder();
            }

            Add(scrollView);
            RegisterCallback<GeometryChangedEvent>(OnGeoChange);
        }
        
        private void OnGeoChange(GeometryChangedEvent geo)
        {
            scrollView.style.minHeight = resolvedStyle.height;
        }

        private void RebuildFromBinder()
        {
            binder.UpdateSerializedModel();
            scrollView.Clear();
            foreach (var m in binder.boundDitionary)
            {
                scrollView.Add(BlackboardField.LoadField(m.Key,
                    m.Value.initialValue.GetType(), m.Value,
                    binder, RebuildFromBinder, scrollView));
            }
        }
    }
}