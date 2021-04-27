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

        public RuntimeNode Node => CurrentEvaluation.currentGraph.nodes[toNodeIndex];
    }
}