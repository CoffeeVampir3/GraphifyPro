using System;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public struct Link
    {
        [SerializeField]
        internal short toNodeIndex;
        [SerializeField]
        internal short toPortIndex;
        [SerializeField]
        internal short fromPortIndex;
        [SerializeField]
        internal short dynamicPortId;
        public RuntimeNode Node => toNodeIndex < 0 ? null : RuntimeGraph.current.nodes[toNodeIndex];
        
        public static readonly Link Invalid = new()
        {
            toNodeIndex = -1,
            toPortIndex = -1,
            fromPortIndex = -1,
            dynamicPortId = 0
        };
    }
}