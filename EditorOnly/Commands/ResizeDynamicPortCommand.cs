using System.Linq;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Graphify.EditorOnly
{
    class ResizeDynamicPortCommand : ModelCommand<GraphifyNodeModel>
    {
        const string k_UndoStringSingular = "Add Port";

        private readonly DynamicPortInfo _targetPortInfo;
        private readonly int resizeBy;

        public ResizeDynamicPortCommand(DynamicPortInfo targetPortInfo, int resizeBy,
            params GraphifyNodeModel[] nodes)
            : base(k_UndoStringSingular, k_UndoStringSingular, nodes)
        {
            this.resizeBy = resizeBy;
            this._targetPortInfo = targetPortInfo;
        }

        public static void DefaultHandler(GraphToolState state, ResizeDynamicPortCommand command)
        {
            if (!command.Models.Any())
                return;

            state.PushUndo(command);

            using var updater = state.GraphViewState.UpdateScope;
            foreach (var nodeModel in command.Models)
            {
                if(nodeModel is IHasDynamicPorts modelWithDynamics)
                    modelWithDynamics.ResizeDynamicPort(command._targetPortInfo, command.resizeBy);
            }
            
            updater.MarkChanged(command.Models);
            //Hey it works! =D
            //For some reason the deleted edge don't actually get update without this.
            updater.ForceCompleteUpdate();
        }
    }
}