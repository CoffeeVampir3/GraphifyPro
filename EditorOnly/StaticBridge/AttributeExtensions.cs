using System;
using Vampire.Runtime;

namespace Vampire.Graphify.EditorOnly.StaticBridge
{
    public static class AttributeExtensions
    {
        public static UnityEditor.GraphToolsFoundation.Overdrive.PortCapacity ToUnity(this PortCapacity pc)
        {
            return pc switch
            {
                PortCapacity.Single => UnityEditor.GraphToolsFoundation.Overdrive.PortCapacity.Single,
                PortCapacity.Multi => UnityEditor.GraphToolsFoundation.Overdrive.PortCapacity.Multi,
                _ => throw new ArgumentOutOfRangeException(nameof(pc), pc, null)
            };
        }
        
        public static UnityEditor.GraphToolsFoundation.Overdrive.Orientation ToUnity(this Orientation ori)
        {
            return ori switch
            {
                Orientation.Horizontal => UnityEditor.GraphToolsFoundation.Overdrive.Orientation.Horizontal,
                Orientation.Vertical => UnityEditor.GraphToolsFoundation.Overdrive.Orientation.Vertical,
                _ => throw new ArgumentOutOfRangeException(nameof(ori), ori, null)
            };
        }
    }
}