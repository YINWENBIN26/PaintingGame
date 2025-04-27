using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    // NPCの状態を管理するための抽象クラスです。
    public abstract class INPCStateNode
    {
        // NPCが特定の状態で行うべき動作を定義します。
        public abstract void Execute(NPC npc);

        // NPCが特定の状態を離れる際に実行されるメソッドです。
        public abstract void Exit(NPC npc);

        // NPCが特定の状態に入る際に実行されるメソッドです。
        public abstract void Enter(NPC npc);
    }

    // NPCが待機状態にあることを表すクラスです。
    public class NpcWaitNode : INPCStateNode
    {
        public override void Enter(NPC npc)
        {
        }

        public override void Execute(NPC npc)
        {
            npc.ChangeState(new NpcIdleNode());
        }

        public override void Exit(NPC npc)
        {
        }
    }

    // NPCがアイドル状態にあることを表すクラスです。
    public class NpcIdleNode : INPCStateNode
    {
        private float startTime; // アイドル状態に入ってからの時間を測るための変数です。
        private float idleDuration; // NPCがアイドル状態に留まる期間です。

        // コンストラクタ。アイドル状態の持続時間をランダムに設定します。
        public NpcIdleNode()
        {
            idleDuration = Random.Range(30, 60);
            startTime = Time.time;
        }

        // 。NPCに移動機能を追加します。
        public override void Enter(NPC npc)
        {
            npc.gameObject.AddComponent<NpcMoveNav>();
        }

       
        public override void Execute(NPC npc)
        {
            if (Time.time - startTime > idleDuration)
            {
                npc.ChangeState(new NpcLeaveState()); // 指定時間経過後に離脱状態へ遷移
            }
        }

        // NPCから移動機能を削除します。
        public override void Exit(NPC npc)
        {
            Object.Destroy(npc.gameObject.GetComponent<NpcMoveNav>());
        }
    }

    // NPCが取引（Deal）状態にあることを表すクラスです。
    public class NpcDealState : INPCStateNode
    {
        Transform queueTransform; // NPCが向かうべき場所を示す。
        int rotation; // NPCが向くべき方向を示す整数値です。

        // コンストラクタ。取引場所と向くべき方向を設定します。
        public NpcDealState(Transform queueTransform, int rotation)
        {
            this.queueTransform = queueTransform;
            this.rotation = rotation;
        }

        // NPCが指定された場所に移動を開始します。
        public override void Enter(NPC npc)
        {
            npc.agent.SetDestination(npc.agent.FindMapcube(queueTransform), delegate { npc.ChangeRotation(rotation); }, (Vector2 dir) => { npc.MoveAhead(dir); });
        }

        public override void Execute(NPC npc)
        {
        }

        public override void Exit(NPC npc)
        {

            GameStateManager._Instance.nPCManager.waitQueue.QueueCount -= 1;
        }
    }

    // NPCが離脱状態にあることを表すクラスです。
    public class NpcLeaveState : INPCStateNode
    {
        // NPCが離脱地点に向かって移動を開始します。
        public override void Enter(NPC npc)
        {
            npc.agent.SetDestination(npc.agent.FindMapcube(GameStateManager._Instance.nPCManager.waitQueue.LeaveTrans), (Vector2 dir) => { npc.MoveAhead(dir); });
            npc.agent.OnDestinationReached += npc.Destoryit;

        }

        public override void Execute(NPC npc)
        {
        }

        public override void Exit(NPC npc)
        {
        }
    }

}