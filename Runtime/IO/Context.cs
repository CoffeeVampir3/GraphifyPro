using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vampire.Runtime
{
    public class Context
    {
        public readonly RuntimeGraph currentGraph;
        public Dictionary<string, object> LocalBlackboard => currentGraph.localBlackboard;
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