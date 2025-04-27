using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace NPC
{
    [RequireComponent(typeof(PathFinding))]

    public class NpcMoveNav : MonoBehaviour
    {
        public Transform targetTran; // 現在の目標位置 (当前目标位置)
        public List<DestinationTranslist> targetTranList;
        private PathFinding _agent;
        MoveState NPCMovestate; // NPCの移動状態 (NPC的移动状态)

        // 移動中フラグ (是否正在移动)
        public bool isMoving = true;
        public MoveState nPCMovestate { get => NPCMovestate; set => NPCMovestate = value; }

        [SerializeField] private Animator animator; 

        void Awake()
        {
            animator = GetComponent<Animator>(); 
            targetTranList = GameStateManager._Instance.nPCManager.targetTranList; // 目標位置リストを初期化 (初始化目标位置列表)
        }

        void OnEnable()
        {
            agent.OnDestinationReached += MoveRandom;
            agent.OnDestinationInvalid += MoveRandom;
        }
        // idle状態への移行 (切换到等待状态)
        void Decideidle()
        {
            float a = UnityEngine.Random.Range(1.0f, 5.0f);
            NPCMovestate = MoveState.idle;
        }
        // 移動状態への移行 (切换到移动状态)
        void DecideMove()
        {
            int a = UnityEngine.Random.Range(1, targetTranList.Count - 1);
            targetTran = targetTranList[a].targetTran; // 目標位置を更新 (更新目标位置)
            print(targetTran.position);
            agent.SetDestination(agent.FindMapcube(targetTran));
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            MoveRandom();
        }
        // ランダムに移動 (随机移动)
        void MoveRandom()
        {
            isMoving = true;
            StartCoroutine(WaitAndMove());
        }
        // 移動前の待機と移動の実行 (移动前的等待和执行移动)
        IEnumerator WaitAndMove()
        {
            var isTarget = 0;
            var rotation = 1;
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 5));
            do
            {
                int a = UnityEngine.Random.Range(1, targetTranList.Count - 1);
                isTarget = targetTranList[a].IsTarget;
                targetTran = targetTranList[a].targetTran;
                rotation = targetTranList[a].Rotation;
            } while (isTarget < 0);
            agent.SetDestination(agent.FindMapcube(targetTran), delegate { ChangeRotation(rotation); }, (Vector2 dir) => { GetComponent<NPC>().MoveAhead(dir); });
            isMoving = false;
        }
        // 経路探索エージェントの取得 (获取路径寻找代理)
        private PathFinding agent
        {
            get { return _agent != null ? _agent : _agent = GetComponent<PathFinding>(); }
        }
        // NPCの向きの変更 (改变NPC的朝向)
        void ChangeRotation(int rotation)
        {
            switch (rotation)
            {
                case 1: animator.SetBool("Walking", false); animator.SetFloat("DirY", -1); animator.SetFloat("DirX", 0); break; // 上向き (朝上)
                case 2: animator.SetBool("Walking", false); animator.SetFloat("DirY", 1); animator.SetFloat("DirX", 0); break;  // 下向き (朝下)
                case 3: animator.SetBool("Walking", false); animator.SetFloat("DirY", 0); animator.SetFloat("DirX", -1); break; // 左向き (朝左)
                case 4: animator.SetBool("Walking", false); animator.SetFloat("DirY", 0); animator.SetFloat("DirX", 1); break;  // 右向き (朝右)
            }
        }

        private void OnDestroy()
        {
            agent.OnDestinationReached -= MoveRandom;
            agent.OnDestinationInvalid -= MoveRandom;
        }
    }

    public enum MoveState
    {
        move, // 移動 (移动)
        idle, // 待機 (等待)
    }
}