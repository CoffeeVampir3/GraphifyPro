using System;

namespace Vampire.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DynamicPortDefinition : Attribute
    {
        public readonly int min;
        public readonly int max;
        public DynamicPortDefinition(int min = 0, int max = byte.MaxValue)
        {
            this.min = min;
            this.max = max;
        }
    }
}