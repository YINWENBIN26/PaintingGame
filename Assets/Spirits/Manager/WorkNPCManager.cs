using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorkNPCManager : MonoBehaviour
{
    public static WorkNPCManager _Instance; // シングルトンインスタンス

    // 取引のためのTransform
    public Transform DealTrans; 
    // 全ての描画状態を待つリスト
    public List<SDPaintingStatus> PaintingStatusesList = new List<SDPaintingStatus>();
    // 全ての作業NPCリスト
    public List<WorkingNpc> workingNpcs = new List<WorkingNpc>();
    // 一時的な睡眠のためのTransform
    public Transform tempSleepTran;
    // 選択された作業NPC
    public WorkingNpc selectedWorkNPC;
    // 取引NPC
    public WorkingNpc dealNPC;

    private void Awake()
    {
        Debug.Log(this);
        if (_Instance == null)
        {
            _Instance = this; 
        }
        else
        {

            Destroy(this); 
        }
    }
    private void Start()
    {
        SystemInstanceManager._Instance.timeManager.perSleep += allNpcSleep; // 全NPCが睡眠状態に入るイベントを登録
    }
    void Update()
    {
        /*
        // マウスカーソルが安全エリア内になく、またはUI要素上にない場合の処理
        if (!Screen.safeArea.Contains(Input.mousePosition)) return;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, 1 << LayerMask.NameToLayer("PaintingUI"));
            if (hit.collider != null) // UI上で作業NPCがクリックされた場合
            {
                if (hit.collider.tag == "WorkNPC")
                {
                    if (PaintingManager._Instance.sDPaintingController.paintingControllerWin != null)
                        PaintingManager._Instance.sDPaintingController.paintingControllerWin.Hide();
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (selectedWorkNPC != null) // 以前の作業UIが閉じられていない場合は閉じる
                        {
                            selectedWorkNPC.workingUI.gameObject.SetActive(false);
                        }
                        selectedWorkNPC = hit.collider.transform.GetComponent<WorkingNpc>();
                        if (selectedWorkNPC != null)
                        {
                            selectedWorkNPC.workingUI.show(); // 作業UIを表示
                        }
                    }
                }
            }
            else // 他の場所がクリックされた場合、UIを閉じる
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (selectedWorkNPC != null)
                        selectedWorkNPC.workingUI.Hide();
                }
            }
        }
        */
    }

    // 全てのNPCを睡眠状態にする
    void allNpcSleep()
    {
        foreach (var npcs in workingNpcs)
        {
            npcs.SetSleep(tempSleepTran);
        }
    }

    /// <summary>
    /// 指定されたインデックスに基づいてNPCに作業を命令する
    /// </summary>
    /// <param name="npcIndex">NPCのID</param>
    /// <param name="workIndex">作業状態のID</param>
    public void CommandNpcToWork(WorkingNpc workingNpc, int workIndex)
    {
         workingNpc.SetWork(PaintingStatusesList[workIndex]); // 作業を設定
         PaintingStatusesList.RemoveAt(workIndex); // リストから作業を削除

    }
    /// <summary>
    /// NPCをアシスタントとして作業させる
    /// </summary>
    /// <param name="npcIndex">NPCのID</param>
    public void CommandNpcToAssisant(WorkingNpc workingNpc, int npcIndex)
    {
        if (dealNPC == null)
        {
            workingNpc.SetAssisant(DealTrans); // アシスタントとして設定
        }
    }
}
