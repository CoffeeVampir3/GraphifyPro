using System;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Graphify
{
    public abstract class PortDefinition : Attribute
    {
        public PortType portType;
        public PortCapacity capacity;
        public PortModelOptions options;
        public Orientation orientation;

        protected void Construct(PortCapacity capacity, 
            Orientation orientation, 
            PortModelOptions options)
        {
            this.portType = PortType.Data;
            this.capacity = capacity;
            this.orientation = orientation;
            this.options = options;
        }
    }
}