using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{

    public class Entry : BtPrecondition
    {
        public GameObject behaviorGameOBJ;
        public override BehaviourState Tick()
        {
            return ChildNode.Tick();
        }
    }
    public class Sequence : BtComposite
    {
        public int currnetNode;
        
        public override BehaviourState Tick()
        {
            if (ChildNodes.Count == 0)
            {
                NodeState = BehaviourState.fail;
                return BehaviourState.fail;
            }
            

            var state = ChildNodes[currnetNode].Tick();
            switch (state)
            {
                case BehaviourState.sucess:
                    currnetNode++;
                    if (currnetNode>=ChildNodes.Count)
                    {
                        currnetNode = 0;
                    }
                    NodeState = BehaviourState.sucess;
                    return BehaviourState.sucess;
                default:
                    NodeState = state;
                    return state;
            }
        }
    }
    
    public class Selector : BtComposite
    {
        public int selectIndex;
        
        public override BehaviourState Tick()
        {
            if (ChildNodes.Count == 0)
            {
                ChangeFailState();
                return NodeState;
            }
            
            var selectState = ChildNodes[selectIndex].Tick();

            switch (selectState)
            {
                case BehaviourState.fail:
                    ChangeFailState();
                    break;
                default:
                    selectIndex = 0;
                    NodeState = selectState;
                    return selectState;
            }
            
            for (int i = 0; i < ChildNodes.Count; i++)
            {
                var state = ChildNodes[i].Tick();
                if (state == BehaviourState.fail|| selectIndex == i)continue;
                selectIndex = i;
                NodeState = state;
                return state;
            }
            ChangeFailState();
            return BehaviourState.fail;
        }
    }

    public class Parallel : BtComposite
    {
        public override BehaviourState Tick()
        {
            List<BehaviourState> starts = new List<BehaviourState>();
            for (int i = 0; i < ChildNodes.Count; i++)
            {
                var start = ChildNodes[i].Tick();
                switch (start)
                {
                    case BehaviourState.fail:
                        ChangeFailState();
                        return NodeState;
                    default:
                        starts.Add(start);
                        break;
                }
            }

            for (int i = 0; i < starts.Count; i++)
            {
                if (starts[i] == BehaviourState.executing)
                {
                    NodeState = BehaviourState.executing;
                    return BehaviourState.executing;
                }
            }

            NodeState = BehaviourState.sucess;
            return  BehaviourState.sucess;
        }

    }
    
    public class Repeat : BtPrecondition
    {
        public int LoopNumber;
        [LabelText("回数"),FoldoutGroup("@NodeName")]
        public int LoopStop;
        public override BehaviourState Tick()
        {
            var start = ChildNode.Tick();
            if (LoopStop<=LoopNumber)
            {
                LoopNumber = 0;
                NodeState = BehaviourState.sucess;
                return BehaviourState.sucess;
            }
            LoopNumber++;
            
            if (start == BehaviourState.fail)
            {
                ChangeFailState();
            }
            else
            {
                NodeState = BehaviourState.executing;
            }
            
            return NodeState;
        }
    }
    
    public class So : BtPrecondition
    {
        [LabelText("実行条件"),FoldoutGroup("@NodeName")]
        public Func<bool> Condition;
        public override BehaviourState Tick()
        {
            if (Condition == null)
            {
                ChangeFailState();
                return BehaviourState.fail;
            }
            
            if (Condition.Invoke())
            {
                NodeState = ChildNode.Tick();
                return NodeState;
            }
            ChangeFailState();
            return BehaviourState.fail;
        }

    }
    
    public class Not : BtPrecondition
    {
        [LabelText("実行条件"),FoldoutGroup("@NodeName")]
        public Func<bool> Condition;

        public override BehaviourState Tick()
        {
            if (Condition == null)
            {
                ChangeFailState();
                return BehaviourState.fail;
            }
            
            if (!Condition.Invoke())
            {
                NodeState = ChildNode.Tick();
                return NodeState;
            }
            ChangeFailState();
            return BehaviourState.fail;
        }
    }

}