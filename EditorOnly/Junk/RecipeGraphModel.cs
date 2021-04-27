using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;


namespace Vampire.Graphify.EditorOnly
{
    public class RecipeGraphModel : GraphModel
    {
        
        protected override IDeclarationModel InstantiatePortal(string portalName)
        {
            Debug.Log("waow");
            return base.InstantiatePortal(portalName);
        }
        
    }
}