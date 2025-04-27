using System;
using System.Collections.Generic;
using BehaviourTree;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BehaviourTree
{
    public enum BehaviourState
    {
        unexecuting, sucess,fail, executing
    }
    
    public enum NodeType
    {
        none,root,composite, pecondition, action
    }

    #region 根数据
    [BoxGroup]
    [HideLabel]
    [HideReferenceObjectPicker]
    public abstract class BtNodeBase
    {

        public abstract BehaviourState Tick();



        [ReadOnly, FoldoutGroup("基本データ"), LabelText("ロゴ")]
        public string Guid;
        [FoldoutGroup("基本データ"),LabelText("名前")]
        public string NodeName;
        [FoldoutGroup("基本データ"),LabelText("タイプ")]
        public NodeType NodeType;
        [FoldoutGroup("基本データ"),LabelText("状態")]
        public BehaviourState NodeState;
        [ReadOnly, FoldoutGroup("基本データ"), LabelText("位置")]
        public Vector2 Position;
        [HideInInspector]
        public BtNodeBase parent;

        protected BtNodeBase GetRoot()
        {
            BtNodeBase btNodeBase = this;
            while (btNodeBase.parent != null)
            {
                btNodeBase = btNodeBase.parent;
            }
            return btNodeBase;
        }
        protected void ChangeFailState()
        {
            NodeState = BehaviourState.fail;
            switch (this)
            {
                case BtPrecondition precondition:
                    precondition.ChildNode?.ChangeFailState();
                    break;
                case BtComposite composite:
                    composite.ChildNodes.ForEach(n => n.ChangeFailState());
                    break;
            }
        }

    }

    public abstract class BtComposite : BtNodeBase
    {
        [FoldoutGroup("@NodeName"),LabelText("ChildNode")]
        public List<BtNodeBase> ChildNodes = new List<BtNodeBase>();
    }

    public abstract class BtPrecondition : BtNodeBase
    {
        [FoldoutGroup("@NodeName"),LabelText("ChildNode")]
        public BtNodeBase ChildNode;
    }

    public abstract class BtActionNode : BtNodeBase { }
    #endregion
    
}
