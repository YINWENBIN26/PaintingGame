using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NPC
{
    public class NPCManager : MonoBehaviour
    {
        public SDBuyList sdBuyList = new SDBuyList(); // 絵の売買リスト
        public List<NPC> npcList = new List<NPC>(); // NPCリスト
        public List<DestinationTranslist> targetTranList = new List<DestinationTranslist>(); // NPCが訪れる場所のリスト（AI用）
        public GameObject TransObject; // 移動オブジェクト
        public WaitQueue waitQueue; // 待機キュー
        public PlayerModel playerModel; // プレイヤーモデル
        public DealController dealController;

        void Awake()
        {
            npcList.Clear(); // NPCリストの初期化
            TransObject = GameObject.Find("Trans/NPCTran"); // 移動オブジェクトの取得
            InitialTargetTrans(); // 目標地点の初期化
            gameObject.AddComponent<NPCCreater>(); // NPCCreaterコンポーネントの追加
            playerModel = PlayerModel.GetModel; // プレイヤーモデルの取得
        }

        private void Start()
        {
            StartCoroutine(buySDPainting()); // 絵の購入プロセス開始
        }

        void InitialTargetTrans()
        {
            // 目標地点の初期設定
            for (int i = 0; i < TransObject.transform.childCount; i++)
            {
                Transform tempchild = TransObject.transform.GetChild(i);
                DestinationTranslist temp = new DestinationTranslist(tempchild, 0, tempchild.GetComponent<FaceSetting>().a);
                targetTranList.Add(temp);
            }
        }

        // 絵の購入プロセス
        IEnumerator buySDPainting()
        {
            waitQueue = this.gameObject.AddComponent<WaitQueue>(); // WaitQueueコンポーネントの追加
           // yield return new WaitForSeconds(SimulateClass.AttracionSimulate(playerModel.GetAttration)); // 魅力による待機時間
            while (true)
            {
                // 指定商品の購入準備
                if (npcList.Count > 0 && sdBuyList.SellList.Count > 0 && waitQueue.QueueCount < 5 )
                {
                    yield return new WaitForSeconds(SimulateClass.AttracionSimulate(playerModel.GetAttration) * 2); // 魅力による待機時間の調整
                    int temp = Random.Range(0, npcList.Count - 1);
                    int tempbuy = Random.Range(0, sdBuyList.SellList.Count - 1);
                    NPC tempnpc = npcList[temp];
                    if (tempnpc.currentState is NpcLeaveState ) continue;
                    tempnpc.wishSDPainting = sdBuyList.buySDpainting(sdBuyList.SellList[tempbuy], tempnpc);
                    npcList.RemoveAt(temp);
                    tempnpc.ChangeState(new NpcWaitNode()); // NPCの状態を待機状態に変更
                    waitQueue.NewEnqueue(tempnpc.gameObject); // 待機キューに追加
                }
                yield return new WaitForSeconds(SimulateClass.AttracionSimulate(playerModel.GetAttration)); // 魅力による待機時間
            }
        }
    }

    public class DestinationTranslist
    {
        public Transform targetTran; // 目標地点のTransform
        public int IsTarget; // 目標かどうか
        public int Rotation; // 回転値 0：上 1：下 2：左 3：右
        
        public DestinationTranslist(Transform targetTran, int IsTarget, int Rotation)
        {
            this.targetTran = targetTran;
            this.IsTarget = IsTarget;
            this.Rotation = Rotation;
        }
    }
}