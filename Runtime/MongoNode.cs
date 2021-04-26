using System;

namespace Vampire.Runtime
{
    [Serializable]
    [GraphifyNode("wow!")]
    public class MongoNode : RuntimeNode
    {
        [In]
        public ValuePort<int> mongoPort = new();
    }
}