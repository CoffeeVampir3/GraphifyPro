using System;

namespace Vampire.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DynamicRange : Attribute
    {
        public readonly int initialSize;
        public readonly int min;
        public readonly int max;
        public DynamicRange(int initialSize = 0, int min = 0, int max = byte.MaxValue)
        {
            this.initialSize = initialSize;
            this.min = min;
            this.max = max;
        }
    }
}