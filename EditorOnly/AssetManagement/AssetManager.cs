using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Vampire.Graphify.EditorOnly.AssetManagement
{
    public static class AssetManager
    {
        private const string AssetsName = "Assets";
        private const string PluginsName = "Plugins";
        private const string GraphifyName = "Graphify Pro";
        private const string GraphifyDirectory = AssetsName + "/" + PluginsName + "/" + GraphifyName;
        private const string PluginDirectory = AssetsName + "/" + PluginsName;
        private static void CreatePluginDirectoryIfNoneExists()
        {
            if (AssetDatabase.IsValidFolder(PluginDirectory)) return;
            AssetDatabase.CreateFolder(AssetsName, PluginsName);
        }
        private static void CreateGraphifyFolderIfNoneExists()
        {
            CreatePluginDirectoryIfNoneExists();
            if (AssetDatabase.IsValidFolder(GraphifyDirectory)) return;
            AssetDatabase.CreateFolder(PluginDirectory, GraphifyName);
        }

        public static void CreateThingy(string safeGeneratedName, ref StringBuilder stringToWrite)
        {
            CreateGraphifyFolderIfNoneExists();
            string filePath = Application.dataPath + "/" + 
                              PluginsName + "/" + GraphifyName + "/" + safeGeneratedName + ".cs";
            File.WriteAllText(filePath, stringToWrite.ToString());
        }
    }
}