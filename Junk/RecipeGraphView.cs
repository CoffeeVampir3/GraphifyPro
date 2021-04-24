using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Graphify.EditorOnly
{
    public class RecipeGraphView : GraphView
    {
        public RecipeGraphView(GraphViewEditorWindow window, 
            CommandDispatcher commandDispatcher) : base(window, commandDispatcher) { }
    }
}