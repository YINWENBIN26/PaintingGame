using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkingNPCWin : UIBase
{
    Button workButton;
    Button assistantButton; 
    WorkNPCManager workNPCManager; // NPC管理マネージャー
    public WorkingNpc workingNpc; // 働いているNPC
    Button restButton; // 「休む」ボタン

    protected override void Awake()
    {
        // 親オブジェクトからWorkingNpcコンポーネントを取得
        workingNpc = transform.parent.GetComponent<WorkingNpc>();
        // 各ボタンコンポーネントを取得
        workButton = transform.Find("WorkButton").GetComponent<Button>();
        assistantButton = transform.Find("AssistantButton").GetComponent<Button>();
        restButton = transform.Find("RestButton").GetComponent<Button>();
        // 各ボタンにリスナーを設定
        workButton.onClick.AddListener(delegate { Work(); });
        assistantButton.onClick.AddListener(delegate { Assisant(); });
        restButton.onClick.AddListener(delegate { Rest(); });
        // UIを非表示にする
        Hide();
    }

    private void Start()
    {
        workNPCManager = WorkNPCManager._Instance;
    }

    // 「働く」ボタンの処理
    public void Work()
    {
        workNPCManager.CommandNpcToWork(workingNpc, 0);
        Hide();
    }

    // 「助手」ボタンの処理
    public void Assisant()
    {
        workNPCManager.CommandNpcToAssisant(workingNpc, 0);
        Hide();
    }

    // 「休む」ボタンの処理
    public void Rest()
    {
        workingNpc.SetWait();
        Hide();
    }
}
