using System;
using Vampire.Graphify;
using Vampire.Graphify.Runtime;

namespace GraphifyPro.Runtime
{
    [Serializable]
    [GraphifyNode("wow!")]
    public class MongoNode : RuntimeNode
    {
        [In]
        public ValuePort<int> mongoPort = new();
    }
}