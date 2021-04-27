using System;

namespace Vampire.Runtime
{
    public abstract class PortDefinition : Attribute
    {
        public PortCapacity capacity;
        public Orientation orientation;

        protected void Construct(PortCapacity capacity, 
            Orientation orientation)
        {
            this.capacity = capacity;
            this.orientation = orientation;
        }
    }
}