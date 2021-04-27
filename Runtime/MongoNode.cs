using System;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    [GraphifyNode("wow!")]
    public class MongoNode : RuntimeNode
    {
        [Out(PortCapacity.Multi), SerializeField]
        public DynamicValuePort<int> mongoPort = new();
        [In, SerializeField]
        public ValuePort<int> mongoIn = new();
        [Out, SerializeField] 
        public ValuePort<GameObject> someGoPortOut = new();
        [In, SerializeField]
        public ValuePort<GameObject> someGoPortIn = new();

        public override RuntimeNode Evaluate(Context ctx)
        {
            RuntimeNode someLink = null;
            foreach (var link in mongoPort.Links)
            {
                someLink = link.Node;
                //if (!mongoPort.TryGetValue(link, out var val)) continue;
            }
            mongoIn.LocalValue += 1;
            foreach (var link in someGoPortOut.Links)
            {
                if (!someGoPortOut.TryGetValue(link, out var someGo) || someGo == null)
                    continue;
                Debug.Log(someGo.name);
            }

            foreach (var link in mongoPort.links)
            {
                if (!mongoPort.TryGetValue(link, out var mewers)) continue;
                Debug.Log(mewers);
            }

            someGoPortIn.LocalValue = null;

            return someLink;
        }
    }
}