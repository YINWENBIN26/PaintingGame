using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;


namespace NPC
{
    public class NPC : MonoBehaviour
    {
        public SDGoodsShelf wishSDPainting; // NPCが購入を希望する絵画 (NPC想要购买的绘画)
        public NPCprop nPCprop;
        public int npcid;
        private Animator animator;
        public INPCStateNode currentState; // 現在の状態ノード (当前状态节点)

        // NPCの初期化 (初始化NPC)
        private void InitNPC()
        {
            nPCprop = DataManager._Instance.gameDictionary.NPCPrefabDict[npcid]; // NPCのプロパティを辞書から取得 (从字典中获取NPC属性)
            GameStateManager._Instance.nPCManager.npcList.Add(this);
            animator = GetComponent<Animator>();
            transform.name = transform.name.Replace("(Clone)", ""); // 名前からクローンを削除 (去掉名字中的“(Clone)”)
        }

        private void Awake()
        {
            InitNPC(); // NPCを初期化 (初始化NPC)
            ChangeState(new NpcIdleNode()); // 初期状態を設定 (设置初始状态)
        }

        private void Update()
        {
            currentState?.Execute(this); // 現在の状態を実行 (执行当前状态)
        }

        // 状態の変更 (切换状态)
        public void ChangeState(INPCStateNode newState)
        {
            currentState?.Exit(this); // 現在の状態から退出 (退出当前状态)
            currentState = newState; // 新しい状態に切り替え (切换到新状态)
            currentState?.Enter(this); // 新しい状態に入る (进入新状态)
        }

        // NPCの破棄 (销毁NPC)
        public void Destoryit()
        {
            Destroy(this.gameObject); // NPCオブジェクトを破棄 (销毁NPC对象)
        }

        private void OnDestroy()
        {
            GameStateManager._Instance.nPCManager.npcList.Remove(this); // NPCリストから削除 (从NPC列表中移除)
        }

        private PathFinding _agent; // 経路探索エージェント (路径寻找代理)
        public PathFinding agent
        {
            get
            {
                return _agent != null ? _agent : _agent = GetComponent<PathFinding>(); // 経路探索エージェントを取得 (获取路径寻找代理)
            }
        }

        // アニメーション用の方向ベクトル (动画用的方向向量)
        public Vector2 UP = new Vector2(0, 1);
        public Vector2 DOWN = new Vector2(0, -1);
        public Vector2 LEFT = new Vector2(1, 0);
        public Vector2 RIGHT = new Vector2(-1, 0);
        private Vector2 lastDir; // 最後の方向 (上一次的方向)

        // NPCの移動処理 (NPC的移动处理)
        public void MoveAhead(Vector2 dir)
        {
            var x = Mathf.Round(dir.x);
            var y = Mathf.Round(dir.y);

            // 対角方向を排除 (消除对角方向)
            // y = Mathf.Abs(y) == Mathf.Abs(x) ? 0 : y;

            if (dir != lastDir)
            {
                if (dir == Vector2.zero) // 停止状態 (停止状态)
                {
                    animator.SetBool("Walking", false);
                }
                else if (dir.x < 0) // 右方向 (向右)
                {
                    SetAnimator(RIGHT);
                }
                else if (dir.x > 0) // 左方向 (向左)
                {
                    SetAnimator(LEFT);
                }
                else if (dir.y < 0) // 下方向 (向下)
                {
                    SetAnimator(DOWN);
                }
                else if (dir.y > 0) // 上方向 (向上)
                {
                    SetAnimator(UP);
                }

                lastDir = dir; // 最後の方向を更新 (更新上次方向)
            }
        }

        // アニメーションパラメータの設定 (设置动画参数)
        void SetAnimator(Vector2 vector)
        {
            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);
            animator.SetBool("Walking", true);
        }

        // NPCの向きを変更 (改变NPC的朝向)
        public void ChangeRotation(int rotation)
        {
            switch (rotation)
            {
                case 1: animator.SetBool("Walking", false); animator.SetFloat("DirY", -1); animator.SetFloat("DirX", 0); break; // 上方向 (朝上)
                case 2: animator.SetBool("Walking", false); animator.SetFloat("DirY", 1); animator.SetFloat("DirX", 0); break;  // 下方向 (朝下)
                case 3: animator.SetBool("Walking", false); animator.SetFloat("DirY", 0); animator.SetFloat("DirX", -1); break; // 左方向 (朝左)
                case 4: animator.SetBool("Walking", false); animator.SetFloat("DirY", 0); animator.SetFloat("DirX", 1); break;  // 右方向 (朝右)
            }
        }
    }
}