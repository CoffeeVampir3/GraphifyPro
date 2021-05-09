using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vampire.Runtime
{
    public class Context
    {
        public RuntimeGraph currentGraph;
        private readonly Stack<RuntimeNode> contextStack = new();
        
        public Context(RuntimeGraph virtGraph)
        {
            this.currentGraph = virtGraph;
        }

        /// <summary>
        /// Migrational context to swap from an old context to a new one in the editor.
        /// </summary>
        public Context(Context oldContext, RuntimeGraph virtGraph, Func<RuntimeNode, RuntimeNode> migrateNodeAction)
        {
            Context newStack = new(virtGraph);
            List<RuntimeNode> nodes = new(oldContext.contextStack);
            nodes.Reverse();
            foreach (var node in nodes)
            {
                newStack.Push(migrateNodeAction.Invoke(node));
            }
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