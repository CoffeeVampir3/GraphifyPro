using System;
using UnityEngine;
using Vampire.Graphify;
using Vampire.Graphify.Runtime;

namespace GraphifyPro.Runtime
{
    [Serializable]
    [GraphifyNode("Schlongos/wowBongo!")]
    public class BongoNode : RuntimeNode
    {
        [SerializeField]
        public int mySchlongo;
    }
}