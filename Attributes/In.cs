﻿using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Graphify
{
    public class In : PortDefinition
    {
        public In(PortCapacity capacity = PortCapacity.Single, 
            Orientation orientation = Orientation.Horizontal, 
            PortModelOptions options = PortModelOptions.NoEmbeddedConstant)
        {
            Construct(capacity, orientation, options);
        }
    }
}