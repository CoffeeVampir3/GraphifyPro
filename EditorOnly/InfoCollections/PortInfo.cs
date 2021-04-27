using System;
using UnityEngine;
using Vampire.Runtime;
using PortCapacity = Vampire.Runtime.PortCapacity;

namespace Vampire.Graphify.EditorOnly
{
    [Serializable]
    public class PortInfo
    {
        [SerializeField] 
        public PortCapacity portCapacity;

        public PortInfo(PortDefinition definition)
        {
            this.portCapacity = definition.capacity;
        }
    }
}