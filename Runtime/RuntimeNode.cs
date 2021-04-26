using System;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class RuntimeNode
    {
        [SerializeField, HideInInspector] 
        internal short nodeId = -1;

        public abstract RuntimeNode Evaluate(RuntimeGraph graph);
    }
}