using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class NPCCreater : MonoBehaviour
    {
        public Transform CreaterTran; // NPCを生成する位置 (生成NPC的位置)
        PlayerModel playerModel;

        public bool canCreate = true; // NPCを生成できるかどうかのフラグ (是否可以生成NPC的标志)

        // Awake is called when the script instance is being loaded (脚本实例加载时调用)
        void Awake()
        {
            CreaterTran = this.transform; // このオブジェクトのTransformを設定 (设置此对象的Transform)
            playerModel = PlayerModel.GetModel; // プレイヤーモデルを取得 (获取玩家模型)
        }

        // Start is called before the first frame update (在第一帧更新之前调用)
        private void Start()
        {
            StartCoroutine(NPCIncubator()); // NPC生成器を開始 (启动NPC生成器)
        }

        // NPCを生成するメソッド (生成NPC的方法)
        public void CreateNPC()
        {
            playerModel.CoinChange(1); // コインを変更 (修改硬币数量)
            int tempindex = Random.Range(1, DataManager._Instance.gameDictionary.NPCPrefabDict.Count); // ランダムなインデックス (随机索引)

            // 特定の条件下でのNPC生成処理の調整 (根据特定条件调整NPC生成逻辑)
            if (SystemInstanceManager._Instance.timeManager.day >= 23)
            {
                while (tempindex == 2)
                {
                    tempindex = Random.Range(1, DataManager._Instance.gameDictionary.NPCPrefabDict.Count);
                }
            }

            // NPCのPrefabをロードしてインスタンス化 (加载NPC的预制体并实例化)
            GameObject temp = Resources.Load<GameObject>("Perfab/NPC/" + DataManager._Instance.gameDictionary.NPCPrefabDict[tempindex].ResName);
            temp = Instantiate(temp);

            // NPCのプロパティを設定 (设置NPC的属性)
            temp.GetComponent<NPC>().nPCprop = DataManager._Instance.gameDictionary.NPCPrefabDict[temp.GetComponent<NPC>().npcid];
            temp.transform.position = CreaterTran.position; // NPCの位置を設定 (设置NPC的位置)
        }

        // NPC生成のコルーチン (NPC生成的协程)
        IEnumerator NPCIncubator()
        {
            NPCManager nPCController = GameStateManager._Instance.nPCManager;
            yield return new WaitForSeconds(SimulateClass.NPCSimulate(playerModel.GetAttration)); // 初期待機時間 (初始等待时间)
            // NPCを生成 (生成NPC)
            while (true)
            {
                // NPCの生成条件をチェック (检查NPC生成条件)
                if (nPCController.sdBuyList.SellList.Count != 0)
                {
                    if ((nPCController.npcList.Count + nPCController.waitQueue.QueueCount) < SimulateClass.NPCSimulate(playerModel.GetAttration))
                    {
                        if (canCreate) // 生成可能な場合 (如果可以生成)
                        {
                            CreateNPC(); 
                        }
                    }
                    // 店の魅力に基づく待機時間 (基于店铺魅力的等待时间)
                    yield return new WaitForSeconds(SimulateClass.NPCSimulate(playerModel.GetAttration)+1);
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines(); // オブジェクト破棄時に全てのコルーチンを停止 (在对象销毁时停止所有协程)
        }
    }
}