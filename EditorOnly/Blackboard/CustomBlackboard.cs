using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Vampire.Binding
{
    public class CustomBlackboard : VisualElement
    {
        private readonly ScrollView scrollView = new();
        private Button collapseButton;
        private ToolbarMenu createPropMenu;
        private readonly VisualElement topContainer = new();
        private readonly PropertyFieldBinder binder;
        
        public CustomBlackboard(BlackboardPropertyRetainer propRetainer)
        {
            name = "customBlackboard";
            binder = new PropertyFieldBinder(propRetainer);

            Add(CreateTopMenu());
            RebuildFromBinder();

            Add(scrollView);
            RegisterCallback<GeometryChangedEvent>(OnGeoChange);
        }

        private Button CreateCollapseButton()
        {
            collapseButton = new Button {text = "<<"};
            collapseButton.AddToClassList("collapserButton");

            collapseButton.clicked += Collapse;
            return collapseButton;
        }

        private void Collapse()
        {
            collapseButton.clicked -= Collapse;
            collapseButton.clicked += Popout;
            scrollView.AddToClassList("blackboard-collapsed");
            createPropMenu.AddToClassList("blackboard-collapsed");
            collapseButton.text = " >>";
            topContainer.style.maxWidth = collapseButton.resolvedStyle.width;
        }

        private void Popout()
        {
            collapseButton.clicked -= Popout;        
            collapseButton.clicked += Collapse;
            scrollView.RemoveFromClassList("blackboard-collapsed");
            createPropMenu.RemoveFromClassList("blackboard-collapsed");
            topContainer.style.maxWidth = new StyleLength(StyleKeyword.Initial);
            collapseButton.text = "<<";
        }

        private VisualElement CreateTopMenu()
        {
            var newItemToolbar = new Toolbar();
            createPropMenu = new ToolbarMenu {text = "New Property: "};
            var drawTypes = BlackboardPropertyFieldFactory.GetDrawableTypes();
            foreach (var item in drawTypes)
            {
                if (item == typeof(Enum))
                    continue;
                createPropMenu.menu.AppendAction(item.Name, _ =>
                {
                    scrollView.Add(BlackboardField.CreateNew(item, binder, RebuildFromBinder, scrollView));
                    binder.UpdateSerializedModel();
                });
            }

            var collapseBlackboardButton = CreateCollapseButton();
            newItemToolbar.Add(collapseBlackboardButton);
            newItemToolbar.Add(createPropMenu);
            newItemToolbar.AddToClassList("blackboardToolbar");
            
            topContainer.Add(newItemToolbar);
            topContainer.AddToClassList("blackboardToolContainer");

            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            
            return topContainer;
        }

        private void OnGeoChange(GeometryChangedEvent geo)
        {
            scrollView.style.minHeight = resolvedStyle.height - (topContainer.resolvedStyle.height+15);
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