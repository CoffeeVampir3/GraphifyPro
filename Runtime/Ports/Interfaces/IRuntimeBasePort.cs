using System.Collections.Generic;

namespace Vampire.Runtime
{
    public interface IRuntimeBasePort
    {
        public IReadOnlyList<Link> Links { get; }
        public short PortId { get; set; }

        public void Editor_Reset();

        internal void AddLink(Link link);
        internal void OrderLinks();
    }
}