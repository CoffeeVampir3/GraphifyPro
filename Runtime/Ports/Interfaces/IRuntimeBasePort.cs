using System.Collections.Generic;

namespace Vampire.Runtime
{
    public interface IRuntimeBasePort
    {
        public IReadOnlyList<Link> Links { get; }
        public short PortId { get; set; }

        internal void AddLink(Link link);
        internal void ClearLinks();
        internal void OrderLinks();
    }
}