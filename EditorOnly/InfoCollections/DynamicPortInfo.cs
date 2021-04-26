using System;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using Vampire.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    [Serializable]
    public class DynamicPortInfo
    {
        [SerializeField] 
        public string fieldName;
        [SerializeField]
        public int currentSize = 0;
        public int minSize;
        public int maxSize;
        public OrderedPorts ports = new();

        public DynamicPortInfo(string fieldName, 
            DynamicRange dynDef = null)
        {
            this.fieldName = fieldName;
            if (dynDef != null)
            {
                minSize = dynDef.min;
                maxSize = dynDef.max;
            }
            else
            {
                minSize = 0;
                maxSize = byte.MaxValue;
            }
            if(minSize > maxSize)
                Debug.LogError("Resizable port defined with great min size than max size!");
            if (minSize > 0 || minSize > currentSize)
                currentSize = minSize;
        }
    }
}