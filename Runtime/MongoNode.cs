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
            foreach (var item in graph.localBlackboard)
            {
                Debug.Log(item.Key + " " + item.Value);
            }

            foreach (var link in mongoPort.Links)
            {
            }

            return null;
        }
    }
}