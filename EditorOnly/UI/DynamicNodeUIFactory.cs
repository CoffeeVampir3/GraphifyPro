using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Graphify.EditorOnly
{
    [GraphElementsExtensionMethodsCache]
    static class DynamicNodeUIFactory
    {
        public static IModelUI CreateNode(this ElementBuilder elementBuilder, CommandDispatcher store, GraphifyNodeModel nodeModel)
        {
            IModelUI ui = new DynamicNodeUI(nodeModel);
            ui.SetupBuildAndUpdate(nodeModel, store, elementBuilder.GraphView, elementBuilder.Context);
            return ui;
        }
    }
}