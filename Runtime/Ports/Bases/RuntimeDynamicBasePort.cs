using System;
using System.Collections.Generic;

namespace Vampire.Graphify.Runtime
{
    [Serializable]
    public abstract class RuntimeDynamicBasePort : RuntimeBasePort, IRuntimeDynamicBasePort
    {
        private Dictionary<int, List<Link>> cachedIndexedLinks = new();
        private static List<Link> emptyLinkList = new(0);
        public List<Link> this[int index]  {
            get
            {
                if (index > links.Count)
                    return emptyLinkList;
                if (cachedIndexedLinks.TryGetValue(index, out var cachedLinks))
                {
                    return cachedLinks;
                }
                var indexedLinks = new List<Link>(8);
                foreach (var item in links)
                {
                    if (item.dynamicPortId == index)
                    {
                        indexedLinks.Add(item);
                    }
                }
                if (indexedLinks.Count == 0)
                {
                    cachedIndexedLinks.Add(index, emptyLinkList);
                    return emptyLinkList;
                }
                
                cachedIndexedLinks.Add(index, indexedLinks);
                return indexedLinks;
            }
        }
    }
}