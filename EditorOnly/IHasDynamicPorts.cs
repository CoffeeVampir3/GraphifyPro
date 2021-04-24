using System.Collections.Generic;

namespace Vampire.Graphify.EditorOnly
{
    public interface IHasDynamicPorts
    {
        IReadOnlyList<DynamicPortInfo> DynamicPortList { get; }

        void ResizeDynamicPort(DynamicPortInfo targetPortInfo, int by);
    }
}