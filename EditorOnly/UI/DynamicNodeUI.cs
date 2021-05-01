using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.UIElements;

namespace Vampire.Graphify.EditorOnly
{
    public class DynamicNodeUI : CollapsibleInOutNode
    {
        private readonly GraphifyNodeModel nodeModel;
        public DynamicNodeUI(GraphifyNodeModel nodeModel)
        {
            this.nodeModel = nodeModel;
        }

        protected override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            //Hopefully temporary solution until we can figure out a more elegant UI solution.
            foreach (var item in nodeModel.DynamicPortList)
            {
                evt.menu.AppendAction(item.fieldName + "/Add Port", _ =>
                {
                    CommandDispatcher.Dispatch(new ResizeDynamicPortCommand(
                        item, 1, nodeModel));
                });
                evt.menu.AppendAction(item.fieldName + "/Remove Port", _ =>
                {
                    CommandDispatcher.Dispatch(new ResizeDynamicPortCommand(
                        item, -1, nodeModel));
                });
            }
        }
    }
}
