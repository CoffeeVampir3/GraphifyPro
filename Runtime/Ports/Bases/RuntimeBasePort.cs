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
        [SerializeField, HideInInspector] 
        protected internal short portId = -1;
        public IReadOnlyList<Link> Links => links;

        public RuntimeNode FirstNode()
            => links.Count <= 0 ? null : links[0].Node;
        public Link FirstLink()
            => links.Count <= 0 ? Link.Invalid : links[0];
        
        short IRuntimeBasePort.PortId { get => portId; set => portId = value; }
        void IRuntimeBasePort.AddLink(Link link) => links.Add(link);
        void IRuntimeBasePort.OrderLinks() => 
            links = links.OrderBy(link => link.dynamicPortId).ToList();
        void IRuntimeBasePort.Editor_Reset()
        {
            portId = -1;
            links.Clear();
        }
    }
}