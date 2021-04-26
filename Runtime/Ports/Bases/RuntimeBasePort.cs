using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class RuntimeBasePort : IRuntimeBasePort
    {
        [SerializeField, HideInInspector]
        protected internal List<Link> links = new();
        public IReadOnlyList<Link> Links => links;
        void IRuntimeBasePort.AddLink(Link link) => links.Add(link);
        void IRuntimeBasePort.ClearLinks() => links.Clear();
        void IRuntimeBasePort.OrderLinks() => 
            links = links.OrderBy(link => link.dynamicPortId).ToList();
    }
}