using System;

namespace Vampire.Runtime
{
    public abstract class PortDefinition : Attribute
    {
        public PortCapacity capacity;
        public Orientation orientation;
        public bool showBackingValue;

        protected void Construct(PortCapacity capacity, 
            Orientation orientation, bool showBackingValue)
        {
            this.capacity = capacity;
            this.orientation = orientation;
            this.showBackingValue = showBackingValue;
        }
    }
}