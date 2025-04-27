using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    [BoxGroup]
    [HideReferenceObjectPicker]
    [LabelText("BehaviorTreeData")]
    public class BehaviourTreeData
    {

        [LabelText("TreeRoot"), OdinSerialize]
        public BtNodeBase Root;

        [LabelText("TreeRoot"), OdinSerialize ]
        public List<BtNodeBase> NodeData = new List<BtNodeBase>();

        public SaveTransform ViewTransform;
    }
}

public class SaveTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Matrix4x4 matrix;
}
