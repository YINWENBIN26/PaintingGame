using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPCの作業状態を抽象化したクラスです。(抽象化NPC工作状态的类)
public abstract class IWorkNpcState : MonoBehaviour
{
    // NPCが現在の状態から抜ける時に呼ばれるメソッドです。(NPC从当前状态退出时调用的方法)
    public abstract void Exit(WorkingNpc workingNpc);

    // NPCが新しい状態に入る時に呼ばれるメソッドです。(NPC进入新状态时调用的方法)
    public abstract void Enter(WorkingNpc workingNpc);

    // NPCが移動先に到着した時に呼ばれるメソッドです。(NPC到达移动目的地时调用的方法)
    public abstract void MoveArrive(WorkingNpc workingNpc);
}

// NPCがアシスタントとして働く状態を表すクラスです。(表示NPC作为助手工作的状态的类)
public class IWorkNpcAssisantState : IWorkNpcState
{
    public override void Enter(WorkingNpc workingNpc)
    {
        workingNpc.agent.SetDestination(workingNpc.agent.FindMapcube(workingNpc.dealTrans), delegate { workingNpc.ChangeRotation(3); MoveArrive(workingNpc); }, (Vector2 dir) => { workingNpc.MoveAhead(dir); });
    }

    public override void Exit(WorkingNpc workingNpc)
    {
        // アシスタントに関連する後処理を行います。(执行与助手相关的后续处理)
        WorkNPCManager._Instance.dealNPC = null;
    }

    public override void MoveArrive(WorkingNpc workingNpc)
    {
        // アシスタント作業の対象NPCを設定します。(设置助手工作的目标NPC)
        WorkNPCManager._Instance.dealNPC = workingNpc;
    }
}

// NPCが待機状態にあることを表すクラスです。(表示NPC处于待机状态的类)
public class IWorkNpcWaitState : IWorkNpcState
{
    public override void Enter(WorkingNpc workingNpc)
    {
        // 待機位置への移動を開始します。(开始移动到待机位置)
        workingNpc.agent.SetDestination(workingNpc.agent.FindMapcube(workingNpc.waitTran), delegate { workingNpc.ChangeRotation(3); MoveArrive(workingNpc); }, (Vector2 dir) => { workingNpc.MoveAhead(dir); });
    }

    public override void Exit(WorkingNpc workingNpc)
    {
    }

    public override void MoveArrive(WorkingNpc workingNpc)
    {
    }
}

// NPCが作業状態にあることを表すクラスです。(表示NPC处于工作状态的类)
public class IWorkNpcWorkState : IWorkNpcState
{
    public override void Enter(WorkingNpc workingNpc)
    {
        if (workingNpc.nowPainting != null)
        {
            workingNpc.agent.SetDestination(workingNpc.agent.FindMapcube(workingNpc.nowPainting.paintingTrans), delegate { workingNpc.ChangeRotation(2); MoveArrive(workingNpc); }, (Vector2 dir) => { workingNpc.MoveAhead(dir); });
        }
    }

    public override void Exit(WorkingNpc workingNpc)
    {
        if (workingNpc.nowPainting != null)
        {
            // 作業に関連する後処理を行います。(执行与工作相关的后续处理)
            WorkNPCManager._Instance.PaintingStatusesList.Add(workingNpc.nowPainting);
            workingNpc.nowPainting.npc = null;
            workingNpc.nowPainting = null;
        }
    }

    public override void MoveArrive(WorkingNpc workingNpc)
    {
        // 作業中の絵画に関連付けます。(关联当前工作中的绘画)
        workingNpc.nowPainting.npc = workingNpc;
    }
}

// NPCが睡眠状態にあることを表すクラスです。(表示NPC处于睡眠状态的类)
public class IWorkNpcSleepState : IWorkNpcState
{
    public override void Enter(WorkingNpc workingNpc)
    {
        workingNpc.agent.SetDestination(workingNpc.agent.FindMapcube(workingNpc.sleepTran), delegate { workingNpc.ChangeRotation(1); }, (Vector2 dir) => { workingNpc.MoveAhead(dir); });
    }

    public override void Exit(WorkingNpc workingNpc)
    {
        // 睡眠に関連する後処理を行います。(执行与睡眠相关的后续处理)
        if (workingNpc.nowPainting != null)
        {
            WorkNPCManager._Instance.PaintingStatusesList.Add(workingNpc.nowPainting);
            workingNpc.nowPainting.npc = null;
            workingNpc.nowPainting = null;
        }
    }

    public override void MoveArrive(WorkingNpc workingNpc)
    {
    }
}