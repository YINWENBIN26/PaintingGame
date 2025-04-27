using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{

    public class WaitQueue : SerializedMonoBehaviour
    {
        public int queueCount; 
        public List<GameObject> NPCList = new List<GameObject>(); 
        public Queue<NPC> waitQueue = new Queue<NPC>(); 
        public Dictionary<int, DestinationTranslist> QueueTransDict = new Dictionary<int, DestinationTranslist>(); 
        public Transform QueueTrans; // キューの位置 (队列位置)
        public Transform LeaveTrans; // 離脱位置 (离开位置)

        private void Awake()
        {
            // キューと離脱位置のTransformを初期化 (初始化队列和离开位置的Transform)
            QueueTrans = GameObject.Find("Trans/QueueTrans").GetComponent<Transform>();
            LeaveTrans = GameObject.Find("System/NPCIncubator").GetComponent<Transform>();
            waitQueue.Clear(); // キューをクリア (清空队列)
            InitalQueue(); // 初期キューを設定 (设置初始队列)
        }

        /// <summary>
        /// キューの数を取得または設定します (获取或设置队列的数量)
        /// </summary>
        public int QueueCount
        {
            get => queueCount;
            set
            {
                if (queueCount > value)
                {
                    // キュー数が減少した場合、NPCの状態を更新 (当队列数量减少时，更新NPC状态)
                    for (int i = 1; i <= value; i++)
                    {
                        NPCList[i].GetComponent<NPC>().ChangeState(new NpcDealState(QueueTransDict[i - 1].targetTran, QueueTransDict[i - 1].Rotation));
                    }
                    queueCount = value;
                }
            }
        }

        // 初期キューを設定する (设置初始队列)
        private void InitalQueue()
        {
            queueCount = 0;
            // キュー位置に応じてDestinationTranslistを追加 (根据队列位置添加目标位置列表)
            for (int i = 0; i < 5; i++)
            {
                QueueTransDict.Add(i, new DestinationTranslist(QueueTrans.GetChild(i), 1, QueueTrans.GetChild(i).GetComponent<FaceSetting>().a));
            }
        }

        /// <summary>
        /// 新しいNPCをキューに追加する (向队列中添加新的NPC)
        /// </summary>
        /// <param name="npc">追加するNPC (要添加的NPC)</param>
        public void NewEnqueue(GameObject npc)
        {
            queueCount += 1; 
            NPCList.Add(npc); 
            waitQueue.Enqueue(npc.GetComponent<NPC>()); // キューに追加 (添加到队列)
                                                        // NPCの状態をキューに合わせて変更 (根据队列位置更新NPC状态)
            npc.GetComponent<NPC>().ChangeState(new NpcDealState(QueueTransDict[queueCount - 1].targetTran, QueueTransDict[queueCount - 1].Rotation));
        }
    }
}