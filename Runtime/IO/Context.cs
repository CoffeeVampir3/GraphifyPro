using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vampire.Runtime
{
    public class Context
    {
        public RuntimeGraph currentGraph;
        public PropertyDictionary Properties => currentGraph.properties;
        private readonly Stack<RuntimeNode> contextStack = new();
        
        public Context(RuntimeGraph virtGraph)
        {
            this.currentGraph = virtGraph;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return contextStack.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(RuntimeNode node)
        {
            contextStack.Push(node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RuntimeNode Pop()
        {
            var m = contextStack.Pop();
            return m;
        }
    }
}