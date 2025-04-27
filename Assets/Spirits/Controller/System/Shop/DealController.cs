using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
public class DealController : MonoBehaviour
{
    public DealWinInfo dealWinInfo;  // 取引情報ウィンドウの参照
    public NPC.NPC nPC; // 取引するNPC
    public int price; // 提案された価格
    int presentPrice; // 現在の価格比率
    int Money; // NPCの所持金

    //0-1 
    int[] canDealPresent = new int[3]; // 取引可能範囲（最小、推奨、最大）
    PlayerModel playerModel; // プレイヤーモデル
    public ItemClass buything; // 購入対象アイテム
    // NPCの取得と設定
    public NPC.NPC NPC
    {
        get => nPC;
        set
        {
            nPC = value;
            price = nPC.wishSDPainting.painting.price;
            Money = value.nPCprop.coin; // NPCの所持金を設定
            canDealPresent[0] = value.nPCprop.Minafford; // 最小価格
            canDealPresent[1] = value.nPCprop.Dealafford; // 推奨価格
            canDealPresent[2] = value.nPCprop.MAXafford; // 最大価格
        }
    }
    public void ShowPricePanel()
    {
        dealWinInfo.Show(nPC.wishSDPainting.Painting, price);
    }

    public PlayerModel PlayerModel { get => PlayerModel.GetModel; set => playerModel = value; }

    void Awake()
    {
        dealWinInfo = UIManager._Instance.dealWinInfo;// 取引ウィンドウの参照を取得
        npcDialgueAsset = Resources.Load<TextAsset>("TextAssets/Dialogue");// NPCのダイアログデータをロード
    }
    void Start()
    {
        //StartCoroutine(dealProcess());
    }
    private void OnDestroy()
    {
        GameStateManager._Instance.nPCManager.waitQueue.waitQueue.Dequeue();
        GameStateManager._Instance.candeal = true;
    }
    /*
    IEnumerator dealProcess()
    {
        yield return StartCoroutine(I_dialgue());
        yield return StartCoroutine(I_dealui());

    }
    // NPCとのダイアログ表示コルーチン
    IEnumerator I_dialgue()
    {
        ReadNPCDialougeData(nPC.npcid - 1);
        yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
    }
    // 取引UI表示コルーチン
    IEnumerator I_dealui()
    {
        // NPCが欲しい絵を取引ウィンドウに表示
        dealWinInfo.Show(nPC.wishSDPainting.Painting);
        // 取引ウィンドウが開いている間、待機
        while (dealWinInfo.gameObject.activeInHierarchy)
        {
            yield return null;
        }
        // 提案価格を元の価格に対するパーセンテージとして計算
        presentPrice = price * 100 / nPC.wishSDPainting.Painting.price;
        if (presentPrice > canDealPresent[2]) // 最初の価格が高すぎる場合
        {
            // 取引終了
            Dealdialogue(nPC.name, "i haven't any money");
            yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
            CancelDeal();
        }
        else if (presentPrice >= canDealPresent[1]) // 価格が少し高い場合
        {
            dealWinInfo.Show(nPC.wishSDPainting.Painting, price);
            while (dealWinInfo.gameObject.activeInHierarchy)
            {
                yield return null;
            }
            presentPrice = price * 100 / nPC.wishSDPainting.Painting.price;
            if (presentPrice > canDealPresent[1]) // 価格がまだ高い場合
            {
                // 取引終了
                Dealdialogue(nPC.name, "it's too expersive");
                yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
                CancelDeal();
            }
            else
            {
                if (Money > price) // お金が足りる場合
                {
                    Dealdialogue(nPC.name, "I buy it");
                    yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
                    FinishDeal();
                    // 成立
                }
                else
                {
                    Dealdialogue(nPC.name, "i haven't any money");
                    yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
                    CancelDeal();
                }
            }
        }
        else if (presentPrice < canDealPresent[0]) // 価格が安い場合
        {
            if (Money > price)
            {
                Dealdialogue(nPC.name, "I buy it");
                yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
                FinishDeal();
                // 成立
            }
            else
            {
                Dealdialogue(nPC.name, "i haven't any money");
                yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
                CancelDeal();
            }
        }
        else // その他の条件
        {
            dealWinInfo.Show(nPC.wishSDPainting.Painting, price);
            while (dealWinInfo.gameObject.activeInHierarchy)
            {
                yield return null;
            }
            presentPrice = price * 100 / nPC.wishSDPainting.Painting.price;
            if (presentPrice < (int)nPC.wishSDPainting.Painting.price * 1.1)
            {
                if (Money > price)
                {
                    // 成立
                    Dealdialogue(nPC.name, "I buy it ");
                    yield return StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
                    FinishDeal();
                }
            }
        }
    }*/
    public TextAsset npcDialgueAsset;
    public Queue<DialogueData> dialgue = new Queue<DialogueData>();

    // NPCの対話データを読み込むメソッド
    void ReadNPCDialougeData(int npcid)
    {
        dialgue.Clear();
        string str = npcDialgueAsset.ToString();
        // 日ごとの対話データを分割
        string[] PerDayDialogue = str.Split('&');
        // NPCごとの対話データを分割
        string[] PerNPCStrArray = PerDayDialogue[SystemInstanceManager._Instance.timeManager.day - 20].Split('$');

        // 対話データをキューに格納
        string[] proArray = PerNPCStrArray[npcid].Split('|');
        for (int j = 0; j < proArray.Length; j++)
        {
            string[] tempArray = proArray[j].Split(':');
            DialogueData temp = new DialogueData(tempArray[0], tempArray[1]);
            dialgue.Enqueue(temp);
        }
    }

    // 特定の対話をキューに追加するメソッド
    void Dealdialogue(string name, string dialogue)
    {
        dialgue.Clear();
        dialgue.Enqueue(new DialogueData(name, dialogue));
    }
    // 取引を完了するメソッド
    public void FinishDeal()
    {
        PlayerModel.CoinChange(price); // プレイヤーのコインを更新
        nPC.wishSDPainting.Painting = null; // NPCの欲しい絵をnullに
        nPC.ChangeState(new NpcLeaveState()); // NPCの状態を変更
        Destroy(this.gameObject); // このコンポーネントを破棄
    }
    public void Showdialogue(string dialogue)
    {
        dialgue.Clear();
        dialgue.Enqueue(new DialogueData(nPC.name, dialogue));
        StartCoroutine(SystemInstanceManager._Instance.dialogueController.DialogueStart(dialgue));
    }

    // 取引をキャンセルするメソッド
    public void CancelDeal()
    {
        nPC.ChangeState(new NpcLeaveState()); // NPCの状態を変更
        GameStateManager._Instance.nPCManager.sdBuyList.addSellList(nPC.wishSDPainting); // 絵を販売リストに追加
        Destroy(this.gameObject); // このコンポーネントを破棄
    }
    public PriceSate JundgePrice()
    {
        print(price);
        print(nPC.wishSDPainting.Painting.price);
        presentPrice = price * 100 / nPC.wishSDPainting.Painting.price;
        print(presentPrice);
        print(canDealPresent[2]);
        if (presentPrice > canDealPresent[2])
        {
            return PriceSate.TooExpensive;
        }
        else if (presentPrice > canDealPresent[1])
        {
            return PriceSate.Expensive;
        }
        else if (presentPrice > canDealPresent[0])
        {
            return PriceSate.Suitable;

        }
        else
        {
            return PriceSate.cheap;
        }
    }
    public bool JundgeMoney()
    {
        return Money > price;

    }

}

public enum PriceSate
{
    TooExpensive,
    Expensive,
    Suitable,
    cheap,
    
}

