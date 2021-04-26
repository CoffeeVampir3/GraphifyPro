﻿using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine;
using Vampire.Runtime;

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