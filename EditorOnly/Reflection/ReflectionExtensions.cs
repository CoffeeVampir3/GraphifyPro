using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vampire.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    public static class ReflectionExtensions
    {
        private static void GetDefinedPortFields(Type t, ref IEnumerable<FieldInfo> fieldInfo, ref IEnumerable<PortDefinition> portDefs)
        {
            var fields = t.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | 
                BindingFlags.Instance | BindingFlags.DeclaredOnly);

            var definedFields = new List<FieldInfo>(fields.Length);
            var defs = new List<PortDefinition>(fields.Length);
            foreach (var field in fields)
            {
                var portDef = field.GetCustomAttribute<PortDefinition>();
                if (portDef == null) continue;
                definedFields.Add(field);
                defs.Add(portDef);
            }

            fieldInfo = fieldInfo.Concat(definedFields);
            portDefs = portDefs.Concat(defs);
            if(t.BaseType != t && t.BaseType != null)
                GetDefinedPortFields(t.BaseType, ref fieldInfo, ref portDefs);
        }
        
        public static void GetDefinedPortFields(this RuntimeNode rtNode, out FieldInfo[] fieldInfo, out PortDefinition[] portDefs)
        {
            IEnumerable<FieldInfo> fieldInfos = new List<FieldInfo>();
            IEnumerable<PortDefinition> portDefinitions = new List<PortDefinition>();
            GetDefinedPortFields(rtNode.GetType(), ref fieldInfos, ref portDefinitions);
            fieldInfo = fieldInfos.ToArray();
            portDefs = portDefinitions.ToArray();
        }
    }
}