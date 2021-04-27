using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vampire.Runtime
{
    [Serializable]
    public abstract class RuntimeBasePort : IRuntimeBasePort
    {
        [SerializeField]
        protected internal List<Link> links = new();
        [SerializeField] 
        protected internal short portId = -1;

        public virtual void Editor_Reset()
        {
            portId = -1;
            links.Clear();
        }
        
        public IReadOnlyList<Link> Links => links;
        public short PortId { get => portId; set => portId = value; }
        void IRuntimeBasePort.AddLink(Link link) => links.Add(link);
        void IRuntimeBasePort.OrderLinks() => 
            links = links.OrderBy(link => link.dynamicPortId).ToList();
    }
}