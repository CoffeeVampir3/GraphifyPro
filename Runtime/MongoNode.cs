using System;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    [GraphifyNode("wow!")]
    public class MongoNode : RuntimeNode
    {
        [SerializeField]
        [Out]
        public DynamicValuePort<int> mongoPort = new();
        [SerializeField]
        [In]
        public ValuePort<int> mongoIn = new();

        public override RuntimeNode Evaluate(RuntimeGraph graph)
        {

            RuntimeNode someLink = null;
            foreach (var link in mongoPort.Links)
            {
                someLink = link.Node;
                if (!mongoPort.TryGetValue(link, out var val)) continue;
                Debug.Log(val);
            }
            mongoIn.LocalValue += 1;

            return someLink;
        }
    }
}