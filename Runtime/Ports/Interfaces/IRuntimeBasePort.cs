using System.Collections.Generic;

namespace Vampire.Runtime
{
    public interface IRuntimeBasePort : IHasNodeConnections
    {
        public IReadOnlyList<Link> Links { get; }

        internal void AddLink(Link link);
        internal void ClearLinks();
        internal void OrderLinks();
    }
}