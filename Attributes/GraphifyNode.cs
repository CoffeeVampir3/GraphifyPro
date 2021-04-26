using System;

namespace Vampire.Runtime
{
    [Serializable]
    public class GraphifyNode : Attribute
    {
        public readonly string path;

        public GraphifyNode(string searchPath)
        {
            path = searchPath;
        }
    }
}