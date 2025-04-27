using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;
using System;
using BehaviorTree.WorkNpc;

public class WorkingNpc : MonoBehaviour, IWorkNPCData
{
    protected WorkingNpcState workingNPCState; 
    public NPCprop nPCprop; 
    public int npcid; 
    public Animator animator; 
    //public WorkingNPCWin workingUI; 

    public event Action OnReachDes; // NPCが目的地に到達したときにトリガーされるイベント。(NPC到达目的地时触发的事件)
    public bool isReached = false; // NPCが目的地に到達したかどうかのフラグ。(NPC是否到达目的地的标志)
    IWorkNpcState currentState; // 現在のNPC状態を管理する。(管理NPC当前状态)

    public bool _isArrived = false; // NPCが移動完了したかどうかのフラグ。(标志NPC是否完成移动)

    private void Awake()
    {
        // NPCの待機位置を設定します。(设置NPC的待机位置)
        waitTran = GameObject.Find("System/" + transform.name + "WaitTran").transform;
    }

    private void Start()
    {
        agent.OnDestinationReached += MoveArrived; 
        InitNPC(); // NPCの初期化処理。(初始化NPC)
    }
    // NPCの初期化処理。(初始化NPC)
    private void InitNPC()
    {
        // NPCの初期状態を設定し、必要なコンポーネントを初期化します。(设置NPC的初始状态并初始化必要组件)
        workingNPCState = WorkingNpcState.Sleep; 
        animator = GetComponent<Animator>(); 
        agent.map = DataManager._Instance.WorkingMapdata; 
        OnReachDes += () => isReached = true; 
        agent.OnDestinationReached += OnReachDes; 
        WorkNPCManager._Instance.workingNpcs.Add(this); 
        //workingUI = transform.GetChild(0).GetComponent<WorkingNPCWin>(); 
    }

    public void ChangeState(IWorkNpcState newState)
    {
        if (GameStateManager._Instance.CurrentState.gameState == GameState.Setting)
        {
            return;
        }
        // 現在の状態を終了し、新しい状態に変更します。(退出当前状态并更改为新状态)
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    private PathFinding _agent; // パス探索を行うエージェント。(用于路径搜索的Agent)
    public PathFinding agent
    {
        get
        {
            return _agent != null ? _agent : _agent = GetComponent<PathFinding>();
        }
    }

    public Vector2 UP = new Vector2(0, 1); 
    public Vector2 DOWN = new Vector2(0, -1); 
    public Vector2 LEFT = new Vector2(1, 0); 
    public Vector2 RIGHT = new Vector2(-1, 0); 
    private Vector2 lastDir = new Vector2(0, 0); 

    public void MoveAhead(Vector2 dir)
    {
        // NPCを指定された方向に移動させます。アニメーションの設定もここで行われます。(让NPC朝指定方向移动，同时设置动画)
        var x = Mathf.Round(dir.x);
        var y = Mathf.Round(dir.y);
        try
        {
            if (dir != lastDir)
            {
                if (dir == Vector2.zero)
                {
                    // 移動がない場合は歩行アニメーションを停止します。(如果没有移动，则停止行走动画)
                    animator.SetBool("Walking", false);
                }
                else if (dir.x < 0)
                {
                    SetAnimator(RIGHT);
                }
                else if (dir.x > 0)
                {
                    SetAnimator(LEFT);
                }
                else if (dir.y < 0)
                {
                    SetAnimator(DOWN);
                }
                else if (dir.y > 0)
                {
                    SetAnimator(UP);
                }
                lastDir = dir; // 最後の移動方向を更新します。(更新最后移动的方向)
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }

    void SetAnimator(Vector2 vector)
    {
        // アニメーターに方向と歩行状態を設定します。(为动画设置方向和行走状态)
        animator.SetFloat("DirX", vector.x);
        animator.SetFloat("DirY", vector.y);
        animator.SetBool("Walking", true);
    }

    public void ChangeRotation(int rotation)
    {
        // NPCの向きを変更します。歩行アニメーションは停止状態にします。(更改NPC的朝向，并停止行走动画)
        switch (rotation)
        {
            case 1: animator.SetBool("Walking", false); animator.SetFloat("DirY", -1); animator.SetFloat("DirX", 0); break;
            case 2: animator.SetBool("Walking", false); animator.SetFloat("DirY", 1); animator.SetFloat("DirX", 0); break;
            case 3: animator.SetBool("Walking", false); animator.SetFloat("DirY", 0); animator.SetFloat("DirX", -1); break;
            case 4: animator.SetBool("Walking", false); animator.SetFloat("DirY", 0); animator.SetFloat("DirX", 1); break;
        }
    }

    public Transform sleepTran; // NPCの睡眠位置。(NPC的睡眠位置)
    public SDPaintingStatus nowPainting; // 現在NPCが作業している絵。(NPC当前正在工作的画)
    public Transform waitTran; // NPCの待機位置。(NPC的待机位置)
    public Transform dealTrans; // 取引位置。(交易位置)

    public void SetSleep(Transform sleepTran)
    {
        // NPCを睡眠状態に設定します。睡眠位置もここで指定されます。(将NPC设置为睡眠状态，同时指定睡眠位置)
        this.sleepTran = sleepTran;
        ChangeState(new IWorkNpcSleepState());
    }

    public void SetWork(SDPaintingStatus sDPaintingStatus)
    {
        // 指定された作業をNPCに割り当てます。現在の作業がある場合は、リストに戻します。(为NPC分配指定的工作，如果已有工作则返回到列表中)
        if (sDPaintingStatus == null) return;
        if (nowPainting != null)
        {
            WorkNPCManager._Instance.PaintingStatusesList.Add(nowPainting);
            nowPainting.npc = null;
        }
        nowPainting = sDPaintingStatus;
        ChangeState(new IWorkNpcWorkState());
    }

    public void SetAssisant(Transform transform)
    {
        // NPCを補助状態に設定します。(设置NPC为辅助状态)
        dealTrans = transform;
        ChangeState(new IWorkNpcAssisantState());
    }

    public void SetWait()
    {
        // NPCを待機状態に設定します。(设置NPC为待机状态)
        //ChangeState(new IWorkNpcWaitState());
    }

    #region ビヘイビアツリー関数
    public bool IsNeedCash()
    {
        // NPCが現金を必要としているかを確認します。(检查NPC是否需要现金)
        return (GameStateManager._Instance.nPCManager.waitQueue.waitQueue.Count > 0);
    }

    public bool isNeedWork()
    {
        // NPCが作業可能かどうかを確認します。(检查NPC是否有工作需要)
        return (WorkNPCManager._Instance.PaintingStatusesList.Count > 0) || nowPainting != null;
    }

    public bool IsNearByCasshier()
    {
        // NPCが取引地点に近いかを確認します。(检查NPC是否靠近交易地点

        _isArrived = false;
        return agent.FindMapcube(this.transform) == agent.FindMapcube(WorkNPCManager._Instance.DealTrans);
    }

    public bool IsNearByWork()
    {
        // NPCが作業地点に近いかを確認します。(检查NPC是否靠近工作地点)
        _isArrived = false;
        if (nowPainting == null && WorkNPCManager._Instance.PaintingStatusesList.Count <= 0) return false;
        if (nowPainting != null) return agent.FindMapcube(this.transform) == agent.FindMapcube(nowPainting.transform);
        nowPainting = WorkNPCManager._Instance.PaintingStatusesList[0];
        WorkNPCManager._Instance.PaintingStatusesList.RemoveAt(0);
        return agent.FindMapcube(this.transform) == agent.FindMapcube(nowPainting.transform);
    }

    public bool IsNearByWait()
    {
        // NPCが待機地点に近いかを確認します。(检查NPC是否靠近待机地点)
        _isArrived = false;
        return agent.FindMapcube(this.transform) == agent.FindMapcube(waitTran);
    }

    public void MoveToDES(Transform DesTransform)
    {
        // NPCを指定された目的地に移動させます。(将NPC移动到指定的目的地)
        agent.SetDestination(agent.FindMapcube(DesTransform), (Vector2 dir) => { MoveAhead(dir); });
    }

    public void MoveToWork(Transform DesTransform)
    {
        // 作業地点に移動します。(移动到工作地点)
        agent.SetDestination(agent.FindMapcube(DesTransform), delegate { WorkMoveArrived(); }, (Vector2 dir) => { MoveAhead(dir); });
    }

    public void MoveArrived()
    {
        // NPCが目的地に到達したフラグを設定します。(设置NPC到达目的地的标志)
        _isArrived = true;
    }

    public void DealMoveArrived()
    {
        // NPCが取引地点に到達した後の処理。(NPC到达交易地点后的处理)
        ChangeRotation(3);
        WorkNPCManager._Instance.dealNPC = this;
    }

    public bool CheckMoving() => _isArrived;

    public bool ISAtDealTran(Transform Trans)
    {
        // NPCが取引地点にいるかを確認します。(检查NPC是否在交易地点)
        return agent.FindMapcube(this.transform) == agent.FindMapcube(Trans);
    }

    void WorkMoveArrived()
    {
        // NPCが作業地点に到達した後の処理。(NPC到达工作地点后的处理)
        ChangeRotation(2);
        nowPainting.npc = this;
    }

    public void WaitMoveArrived()
    {
        // NPCが待機地点に到達した後の処理。(NPC到达待机地点后的处理)
        ChangeRotation(0);
    }

    public void ResetFlagWhenWaitting()
    {
        // NPCが待機中にフラグをリセットします。(在NPC待机时重置标志)
        if (WorkNPCManager._Instance.dealNPC != null)
        {
            WorkNPCManager._Instance.dealNPC = null;
        }
        if (nowPainting != null)
        {
            WorkNPCManager._Instance.PaintingStatusesList.Add(nowPainting);
            nowPainting.npc = null;
            nowPainting = null;
        }
    }

    public void RestFlagWhenDealing()
    {
        // NPCが取引中にフラグをリセットします。(在NPC交易时重置标志)
        if (nowPainting != null)
        {
            WorkNPCManager._Instance.PaintingStatusesList.Add(nowPainting);
            nowPainting.npc = null;
            nowPainting = null;
        }
    }

    public void ResetFlagWhenWorking()
    {
        // NPCが作業中にフラグをリセットします。(在NPC工作时重置标志)
        if (WorkNPCManager._Instance.dealNPC != null)
        {
            WorkNPCManager._Instance.dealNPC = null;
        }
    }
    #endregion
}

public enum WorkingNpcState
{
    Sleep, // 睡眠状態。(睡眠状态)
    Work, // 作業状態。(工作状态)
    Assistant, // 補助状態。(辅助状态)
    Wait // 待機状態。(待机状态)
}

namespace BehaviorTree.WorkNpc
{
    public interface IWorkNPCData
    {
        public bool IsNeedCash(); // NPCが現金を必要としているか。(NPC是否需要现金)
    }
}
