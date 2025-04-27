using UnityEngine;
using UnityEngine.UIElements;


namespace BehaviourTree.Editor.View
{
    public class SplitView : TwoPaneSplitView
    {
        public Label InspectorTitle;

        public InspectorView InspectorView;
        public TreeView TreeView;

        
        public new class UxmlFactory : UxmlFactory<SplitView,UxmlTraits>{}
        public SplitView()
        {
        }

    }
}