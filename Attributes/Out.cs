using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Runtime
{
    public class Out : PortDefinition
    {
        public Out(PortCapacity capacity = PortCapacity.Single,
            Orientation orientation = Orientation.Horizontal, 
            PortModelOptions options = PortModelOptions.NoEmbeddedConstant)
        {
            Construct(capacity, orientation, options);
        }
    }
}