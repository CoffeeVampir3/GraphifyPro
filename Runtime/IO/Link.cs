using System;

namespace Vampire.Graphify.Runtime
{
    [Serializable]
    public struct Link
    {
        public short toNodeIndex;
        public short toPortIndex;
        public short fromPortIndex;
        public short dynamicPortId;
    }
}