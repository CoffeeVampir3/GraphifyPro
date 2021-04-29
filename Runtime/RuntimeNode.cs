using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("GraphifyPro.Editor")]
namespace Vampire.Runtime
{
    [Serializable]
    public abstract class RuntimeNode
    {
        [SerializeField, HideInInspector] 
        internal short nodeId = -1;

        public abstract RuntimeNode Evaluate(Context ctx);
    }
}