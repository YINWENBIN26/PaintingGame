using System.Collections.Generic;
using BehaviourTree;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BehaviourTree.Editor.View
{
    /// <summary>
    /// インスペクターパネルに表示される
    /// </summary>
    public class InspectorDataView : SerializedScriptableObject
    {
        [OdinSerialize, LabelText("Selected Node"), HideReferenceObjectPicker]
        [ListDrawerSettings(IsReadOnly = true)]
        public HashSet<BtNodeBase> selectDatas;

        public InspectorDataView()
        {
            selectDatas = new HashSet<BtNodeBase>();
        }
    }

}

