using UnityEditor.GraphToolsFoundation.Overdrive;

namespace Vampire.Graphify.EditorOnly
{
    public class CustomToolbarProvider : IToolbarProvider
    {
        public bool ShowButton(string buttonName)
        {
            return true;
            //return buttonName != MainToolbar.ShowBlackboardButton;
        }
    }
}