using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Graphify.EditorOnly
{
    public class GraphifyView : GraphView
    {
        public GraphifyView(GraphViewEditorWindow window, 
            CommandDispatcher commandDispatcher) : base(window, commandDispatcher) { }
    }
}