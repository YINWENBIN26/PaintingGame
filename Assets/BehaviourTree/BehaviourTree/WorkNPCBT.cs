using BehaviourTree;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace BehaviourTree.WorkNpc
{
    public class WorkNPCBT : SerializedMonoBehaviour
    {
        public class JudgePosition : BtPrecondition
        {
           [LabelText("判定関数"), FoldoutGroup("@NodeName")]
            public Func<bool> isNearBy;
            [LabelText("リセット関数"), FoldoutGroup("@NodeName")]
            public Action ResetFlag;
            public override BehaviourState Tick()
            {
                
                   Debug.Log(ChildNode.GetType());
                
               
                if (NodeState == BehaviourState.executing)
                {
                    NodeState = ChildNode.Tick();
                    return NodeState;
                }
                if (!isNearBy.Invoke())
                {
                    ResetFlag();
                    ChildNode.Tick();
                    return NodeState = BehaviourState.executing;
                }
                else
                {
                    return BehaviourState.unexecuting;
                }
            }
        }
    }

    public class GotoDes : BtActionNode
    {
        [LabelText("判定関数"), FoldoutGroup("@NodeName")]
        public Func<bool> IsArrive;
        [LabelText("行き先"), FoldoutGroup("@NodeName")]
        public Transform DesTran;
        [LabelText("到着後"), FoldoutGroup("@NodeName")]
        public Action AfterArrived;
        [LabelText("移動関数"), FoldoutGroup("@NodeName")]
        public Action<Transform> HowtoMove;
        public override BehaviourState Tick()
        {
            if (IsArrive.Invoke())
            {
                AfterArrived?.Invoke();
                return NodeState = BehaviourState.sucess;
            }
            if (NodeState == BehaviourState.executing) return BehaviourState.executing;
            Entry entry = GetRoot() as Entry;
            HowtoMove?.Invoke(entry.behaviorGameOBJ.GetComponent<WorkingNpc>().waitTran);
            return NodeState = BehaviourState.executing;
        }
    }
    public class GotoWork : BtActionNode
    {
        [LabelText("判定関数"), FoldoutGroup("@NodeName")]
        public Func<bool> IsArrive;
        [LabelText("到着後"), FoldoutGroup("@NodeName")]
        public Action AfterArrived;
        [LabelText("移動関数"), FoldoutGroup("@NodeName")]
        public Action<Transform> HowtoMove;
        public override BehaviourState Tick()
        {
            if (IsArrive.Invoke())
            {
                AfterArrived?.Invoke();
                return NodeState = BehaviourState.sucess;
            }
            if (NodeState == BehaviourState.executing) return BehaviourState.executing;
            Entry entry =GetRoot() as Entry;
            HowtoMove?.Invoke(entry.behaviorGameOBJ.GetComponent<WorkingNpc>().nowPainting.paintingTrans);
            return NodeState = BehaviourState.executing;
        }
    }

}