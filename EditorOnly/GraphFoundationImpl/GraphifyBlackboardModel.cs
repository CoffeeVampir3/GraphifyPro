using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;

namespace Vampire.Graphify.EditorOnly
{
    public class GraphifyBlackboardModel : BlackboardGraphModel
    {
        /// <inheritdoc />
        public GraphifyBlackboardModel(IGraphAssetModel graphAssetModel)
            : base(graphAssetModel)
        {
        }

        public override string GetBlackboardTitle()
        {
            return AssetModel?.FriendlyScriptName == null
                ? "Blackboard"
                : AssetModel?.FriendlyScriptName + " Blackboard";
        }

        public override string GetBlackboardSubTitle()
        {
            return "Blackboard";
        }

        protected virtual IEnumerable<TypeHandle> GetSupportedTypes()
        {
            var supportedTypes = new List<TypeHandle>
            {
                TypeHandle.Bool,
                TypeHandle.Char,
                TypeHandle.Float,
                TypeHandle.Double,
                TypeHandle.Int,
                TypeHandle.Object,
                TypeHandle.Quaternion,
                TypeHandle.String,
                TypeHandle.Vector2,
                TypeHandle.Vector3,
                TypeHandle.Vector4,
                TypeHandle.GameObject,
                typeof(Color).GenerateTypeHandle(),
                typeof(AnimationClip).GenerateTypeHandle(),
                typeof(Mesh).GenerateTypeHandle(),
                typeof(ScriptableObject).GenerateTypeHandle(),
                typeof(Texture2D).GenerateTypeHandle(),
                typeof(Texture3D).GenerateTypeHandle()
            };

            return supportedTypes;
        }

        public override void PopulateCreateMenu(string sectionName, GenericMenu menu,
            CommandDispatcher commandDispatcher)
        {
            foreach (var typeHandle in GetSupportedTypes())
            {
                menu.AddItem(new GUIContent(typeHandle.Name), false,
                    () => { CreateVariableDeclaration(commandDispatcher, typeHandle.Name, typeHandle); });
            }
        }

        static void CreateVariableDeclaration(CommandDispatcher commandDispatcher, string name, TypeHandle type)
        {
            var finalName = name;
            var i = 0;

            // ReSharper disable once AccessToModifiedClosure
            while (commandDispatcher.GraphToolState.WindowState.GraphModel.VariableDeclarations.Any(v =>
                v.Title == finalName))
                finalName = name + i++;

            commandDispatcher.Dispatch(new CreateGraphVariableDeclarationCommand(finalName, true, type));
        }
    }
}