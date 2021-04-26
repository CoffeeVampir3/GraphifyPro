using System.Collections.Generic;
using System.Reflection;
using Vampire.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    public static class ReflectionExtensions
    {
        public static void GetDefinedPortFields(this RuntimeNode rtNode, out FieldInfo[] fieldInfo, out PortDefinition[] portDefs)
        {
            var fields = rtNode.GetType().GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var definedFields = new List<FieldInfo>(fields.Length);
            var defs = new List<PortDefinition>(fields.Length);
            foreach (var field in fields)
            {
                var portDef = field.GetCustomAttribute<PortDefinition>();
                if (portDef == null) continue;
                definedFields.Add(field);
                defs.Add(portDef);
            }

            fieldInfo = definedFields.ToArray();
            portDefs = defs.ToArray();
        }
    }
}