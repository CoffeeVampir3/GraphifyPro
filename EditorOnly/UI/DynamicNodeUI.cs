﻿using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.UIElements;

namespace Vampire.Graphify.EditorOnly
{
    public class DynamicNodeUI : CollapsibleInOutNode
    {
        private readonly GraphifyNodeModel _nodeModel;
        public DynamicNodeUI(GraphifyNodeModel nodeModel)
        {
            _nodeModel = nodeModel;
        }

        protected override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            //Hopefully temporary solution until we can figure out a more elegant UI solution.
            foreach (var item in _nodeModel.DynamicPortList)
            {
                evt.menu.AppendAction(item.fieldName + "/Add Port", _ =>
                {
                    CommandDispatcher.Dispatch(new ResizeDynamicPortCommand(
                        item, 1, _nodeModel));
                });
                evt.menu.AppendAction(item.fieldName + "/Remove Port", _ =>
                {
                    CommandDispatcher.Dispatch(new ResizeDynamicPortCommand(
                        item, -1, _nodeModel));
                });
            }
        }
    }
}