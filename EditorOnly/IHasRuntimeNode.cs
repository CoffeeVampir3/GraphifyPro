using UnityEditor.GraphToolsFoundation.Overdrive;
using Vampire.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    public interface IHasRuntimeNode : IInputOutputPortsNodeModel, IHasDynamicPorts
    {
        RuntimeNode RuntimeNode { get; }
        short RuntimeNodeId { get; }
    }
}