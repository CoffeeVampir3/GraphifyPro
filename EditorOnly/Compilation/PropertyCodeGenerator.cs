using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Vampire.Runtime;

namespace Vampire.Graphify.EditorOnly
{
    internal static class PropertyCodeGenerator
    {
        private static readonly string invalidCharacters =
            new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        private static string SafeName(string key)
        {
            StringBuilder keyBuilder = new StringBuilder(key);
            
            keyBuilder = keyBuilder.Replace(" ", "_"); 
            foreach (char c in invalidCharacters)
            {
                keyBuilder = keyBuilder.Replace(c.ToString(), ""); 
            }

            return keyBuilder.ToString();
        }
        
        //get => (currentGraph.properties as IUnsafePropertyDictionary).Unsafe_GetDirect<TYPE_NAME>("VAR_NAME");
        private static string BuildUnsafeGetLambda(string fullName, string lookupKey)
            => "get => (currentGraph.properties as IUnsafePropertyDictionary).Unsafe_GetDirect<"
               + fullName + ">(\"" + lookupKey + "\");";
        
        //set => (currentGraph.properties as IUnsafePropertyDictionary).Unsafe_SetDirect("VAR_NAME", value);
        private static string BuildUnsafeSetLambda(string lookupKey)
            => "set => (currentGraph.properties as IUnsafePropertyDictionary).Unsafe_SetDirect(\"" 
               + lookupKey + "\", value);";
        
        //set => (currentGraph.properties as IUnsafePropertyDictionary).Unsafe_SetWrapped<TYPE_NAME>("VAR_NAME", value);
        private static string BuildWrappedUnsafeSetLambda(string fullName, string lookupKey)
            => "set => (currentGraph.properties as IUnsafePropertyDictionary).Unsafe_SetWrapped<"
               + fullName + ">(\"" + lookupKey + "\", value);";
        
        //get => currentGraph.values[VALUE_KEY] as TYPE_NAME;
        private static string BuildKeyedGetLambda(string fullName, int valueKey)
            => "get => currentGraph.values[" + valueKey + "] as " + fullName + ";";
        
        //set => currentGraph.values[VALUE_KEY] = value;
        private static string BuildKeyedSetLambda(int valueKey)
            => "set => currentGraph.values[" + valueKey + "]" + " = value;";
        
        //get => ((Vampire.Runtime.AntiAllocationWrapper<TYPE_NAME>)currentGraph.values[VALUE_KEY]).item;
        private static string BuildKeyedGetLambdaWrapped(string fullName, int valueKey)
            => "get => ((" + fullName + ")" + "currentGraph.values[" + valueKey + "])" + ".item;";
        
        //set => ((Vampire.Runtime.AntiAllocationWrapper<TYPE_NAME>)currentGraph.values[VALUE_KEY]).item = value;
        private static string BuildKeyedSetLambdaWrapped(string fullName, int valueKey)
            => "set => ((" + fullName + ")" + "currentGraph.values[" + valueKey + "])" + ".item = value;";
        private static string CreateDeclaration(ref string typeName, ref string getLambda, 
            ref string setLambda, ref string key)
            => "\t\t" + PCGConstants.publicStaticPrefix + typeName + PCGConstants.singleSpace + SafeName(key) + 
               " {\n\t\t\t" + getLambda + "\n\t\t\t" + setLambda + "\n\t\t}";
        private static string KeyedWrappedEntryToCode(int valKey, string key, object obj)
        {
            var args = obj.GetType().GetGenericArguments();
            var actualName = args.FirstOrDefault()?.FullName;
            var wrappedName = Generics.Unfuck(obj.GetType());
            string getLambda = BuildKeyedGetLambdaWrapped(wrappedName, valKey);
            string setLambda = BuildKeyedSetLambdaWrapped(wrappedName, valKey);

            return CreateDeclaration(ref actualName, ref getLambda, ref setLambda, ref key);
        }
        private static string KeyedDictionaryEntryToCode(int valKey, string key, string typeName, object obj)
        {
            if (obj is AntiAllocationWrapper)
            {
                return KeyedWrappedEntryToCode(valKey, key, obj);
            }
            typeName ??= Generics.Unfuck(obj?.GetType() ?? typeof(object));
            
            string getLambda = BuildKeyedGetLambda(typeName, valKey);
            string setLambda = BuildKeyedSetLambda(valKey);
            return CreateDeclaration(ref typeName, ref getLambda, ref setLambda, ref key);
        }
        
        private static string WrappedDictionaryEntryToCode(string key, object obj)
        {
            var args = obj.GetType().GetGenericArguments();
            var actualName = args.FirstOrDefault()?.FullName;
            
            string getLambda = BuildUnsafeGetLambda(actualName, key);
            string setLambda = BuildWrappedUnsafeSetLambda(actualName, key);
            return CreateDeclaration(ref actualName, ref getLambda, ref setLambda, ref key);
        }

        private static string DictionaryEntryToCode(string key, string typeName, 
            object obj)
        {
            typeName ??= Generics.Unfuck(obj?.GetType() ?? typeof(object));
            string getLambda = BuildUnsafeGetLambda(typeName, key);
            string setLambda = BuildUnsafeSetLambda(key);
            return CreateDeclaration(ref typeName, ref getLambda, ref setLambda, ref key);
        }
        
        //Rough guess for the "average size" of each element
        private static readonly int upperBoundsSizeGuess = BuildWrappedUnsafeSetLambda("some_long_lookup_key", "some_long_lookup_key").Length / 2;
        public static void Generate(RuntimeGraphBlueprint blueprint, Dictionary<string, string> typeNameHelper)
        {
            if (string.IsNullOrWhiteSpace(blueprint.generatedPropertyClassName))
            {
                Debug.LogError("Generating blueprint blackboard failed for: " + blueprint.name +
                               " because the blackboard does not have a valid name.");
                return;
            }

            var initialSizeGuess = (blueprint.localProperties.Properties.Count * upperBoundsSizeGuess) + 420;
            var safeGeneratedName = SafeName(blueprint.generatedPropertyClassName);
            string filePath = Application.dataPath + "/" + safeGeneratedName + ".cs";
            var propertyBuilder = new StringBuilder(PCGConstants.disclaimerAndCodeStart, initialSizeGuess);
            propertyBuilder.Append(safeGeneratedName);
            propertyBuilder.Append(" : " + nameof(RuntimeProperties) + " {\n");

            foreach (var item in blueprint.localProperties.Properties)
            {
                string typeName = typeNameHelper[item.Key];
                switch (item.Value)
                {
                    case ValueKeyedProperty vkProp:
                        var val = blueprint.initializationValues[vkProp.valueKey];
                        propertyBuilder.Append(KeyedDictionaryEntryToCode(vkProp.valueKey, item.Key, typeName, val));
                        propertyBuilder.Append("\n");
                        break;
                    case AntiAllocationWrapper:
                        propertyBuilder.Append(WrappedDictionaryEntryToCode(item.Key, item.Value));
                        propertyBuilder.Append("\n");
                        break;
                    default:
                        propertyBuilder.Append(DictionaryEntryToCode(item.Key, typeName, item.Value));
                        propertyBuilder.Append("\n");
                        break;
                }
            }

            propertyBuilder.Append("\t}\n}");
            File.WriteAllText(filePath, propertyBuilder.ToString());
        }
    }
}