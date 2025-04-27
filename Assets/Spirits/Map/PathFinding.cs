using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

namespace Map
{
    public class PathFinding : MonoBehaviour
    {
        // このエージェントが割り当てられたPolyNav2Dマップのターゲット。(此代理分配的PolyNav2D地图目标)
        [SerializeField]
        private Mapdata _map = null;

        [Header("Steering")]
        // 最大速度 (最大速度)
        public float maxSpeed = 3.5f;

        // 進行方向の予測距離 (预测前进方向的距离)
        public float lookAheadDistance = 1;

        [Header("Options")]
        // 元のトランスフォーム位置からのカスタムセンターオフセット。(自原始变换位置的自定义中心偏移)
        public Vector2 centerOffset = Vector2.zero;

     
        public bool debugPath = true;

        // 新しい目的地がパスが見つかった後に開始された時に発生 (在找到新路径后开始导航时触发)
        public event System.Action OnNavigationStarted;

        // 目的地に到達したときに発生 (到达目的地时触发)
        public event System.Action OnDestinationReached;

        // 目的地が無効になったり、無効になるときに発生 (目标地点无效或变为无效时触发)
        public event System.Action OnDestinationInvalid;

        // パスを進んでいる間に「コーナー」ポイントに到達したときに発生 (沿路径移动时到达某个拐角点时触发)
        public event System.Action<Vector2> OnNavigationPointReached;

        // アニメーションを再生するためのコールバック (用于触发动画的回调)
        public event System.Action<Vector2> OnAnimateReached;

        // 目的地が無効になったり、無効になるときに発生 (目标地点无效或变为无效时触发)
        private event System.Action<bool> ReachedCallback;


        private Vector2 currentVelocity = Vector2.zero; // 現在の速度 (当前速度)
        private int requests = 0; 
        private List<MapCubeClass> _activePath = new List<MapCubeClass>(); 
        private MapCubeClass _primeGoal = null; // 現在の目標 (当前的目标)


        // エージェントの位置 (代理的位置)
        public Vector2 position
        {
            get { return transform.position; }
            set { transform.position = nowMapCube.transform.position; }
        }

        // 現在の位置を確認する (获取当前位置的方块)
        public MapCubeClass nowMapCube
        {
            get { return FindMapcube(transform); }
        }

        // エージェントの現在のアクティブパス (代理的当前活动路径)
        public List<MapCubeClass> activePath
        {
            get { return _activePath; }
            set
            {
                _activePath = value;
                if (_activePath.Count > 0 && _activePath[0] == nowMapCube)
                {
                    _activePath.RemoveAt(0);
                }
            }
        }

        // エージェントの現在のゴール (代理的当前目标)
        public MapCubeClass primeGoal
        {
            get { if (_primeGoal == null) return FindMapcube(transform); else return _primeGoal; }
            set { _primeGoal = value; }
        }

        //エージェントが割り当てられたPolyNavマップのインスタンス (代理分配的PolyNav地图实例)
        public Mapdata map
        {
            get { return _map != null ? _map : DataManager._Instance.mapdata; }
            set { _map = value; }
        }

        // パスが存在するかどうか (路径是否存在)
        public bool hasPath
        {
            get { return activePath.Count > 0; }
        }

        // エージェントが現在向かっているポイント (代理当前移动的目标点)
        public MapCubeClass nextPoint
        {
            get { return hasPath ? activePath[0] : nowMapCube; }
        }

        /// アクティブな経路の残りの距離。経路がない場合は0 (活动路径的剩余距离，如果没有路径则为0)
        public float remainingDistance
        {
            get
            {
                if (!hasPath)
                {
                    return 0;
                }
                float dist = Vector2.Distance(position, activePath[0].transform.position);
                for (int i = 0; i < activePath.Count; i++)
                {
                    dist += Vector2.Distance(activePath[i].transform.position, activePath[i == activePath.Count - 1 ? i : i + 1].transform.position);
                }
                return dist;
            }
        }

        /// 他のエージェントを避けている活動時間（秒） (避开其他代理的活动时间（秒）)
        public float avoidingElapsedTime { get; private set; }

        /// エージェントに目的地を設定する (为代理设置目标点)
        public bool SetDestination(MapCubeClass goal) { return SetDestination(goal, null, null); }
        public bool SetDestination(MapCubeClass goal, Action<Vector2> animate) { return SetDestination(goal, null, animate); }
        public bool SetDestination(MapCubeClass goal, Action<bool> callback) { return SetDestination(goal, callback, null); }

        /// <summary>
        /// 目的地を設定する (设置目标点)
        /// </summary>
        public bool SetDestination(MapCubeClass goal, Action<bool> callback, Action<Vector2> animate)
        {
            // 目的地がnullの場合は、コールバックを呼び出して処理を終了 (如果目标点为空，则调用回调并结束处理)
            if (goal == null)
            {
                if (callback != null)
                {
                    Debug.Log("goal is null");
                    callback(false);
                }
                if (OnDestinationInvalid != null) { OnDestinationInvalid(); }
                return false;
            }

            if (map == null)
            {
                return false;
            }
            if ((goal.transform.position - primeGoal.transform.position).sqrMagnitude < Mathf.Epsilon)
            {
                OnDestinationReached();
                return true;
            }

            ReachedCallback = callback;
            OnAnimateReached = animate;
            primeGoal = goal;

            // 目的地が現在のエージェント位置とほぼ同じ場合、直ちに到着したとみなす (如果目标点与代理当前位置几乎相同，立即视为到达)
            if (goal == nowMapCube)
            {
                OnArrived();
                return true;
            }

            // パスが保留中の場合、新しいパスの計算は行わない (如果路径计算已挂起，则不进行新的路径计算)
            if (requests > 0)
            {
                return true;
            }

            Debug.Log("StartMoving");

            // パスの計算を開始 (开始路径计算)
            requests++;
            map.RequestPathFindingList(nowMapCube, goal, SetPath);
            if (OnAnimateReached != null) LookAhead();
            return true;
        }


        /// <summary>
        /// パスをクリアし、エージェントの移動を停止する (清除路径并停止代理移动)
        /// </summary>
        public void Stop()
        {
            activePath.Clear(); 
            currentVelocity = Vector2.zero; 
            requests = 0; 
            primeGoal = FindMapcube(transform); 
            avoidingElapsedTime = 0;
        }

        /// <summary>
        /// TransformからMapCubeを検索する (从Transform查找MapCube)
        /// </summary>
        /// <returns>見つかった場合はMapCubeClass (如果找到，返回MapCubeClass)</returns>
        public MapCubeClass FindMapcube(UnityEngine.Transform transform)
        {
            Debug.Log(transform);
            var collider = Physics2D.OverlapCircle(transform.position, 0.2f, 1 << LayerMask.NameToLayer("MapPath")); // 指定位置でのコライダーを探す (查找指定位置的碰撞体)
            if (collider != null)
            {
                // 注：MapCubeタグが付いたコライダーをチェック (检查带有MapCube标签的碰撞体)
                if (collider.CompareTag("MapCube"))
                {
                    return collider.GetComponent<MapCubeClass>();
                }
            }
            return null; // 見つからない場合はnullを返す (如果未找到则返回null)
        }

        /// <summary>
        /// パスを設定する (设置路径)
        /// </summary>
        void SetPath(List<MapCubeClass> path)
        {
            path.Reverse(); // パスを逆順にする (反转路径)

            // エージェントがなんらかの理由で停止したが、パスが保留中の場合 (代理由于某种原因停止，但路径仍挂起)
            if (requests == 0)
            {
                return;
            }

            requests--; // リクエスト数を減少 (减少请求计数)

            if (path == null || path.Count == 0)
            {
                OnInvalid(); // 無効なパスの場合 (路径无效时)
                return;
            }

            if (debugPath) TestPath(path); // デバッグモードの場合、パスをテスト (调试模式下测试路径)

            activePath = path; // アクティブパスを設定 (设置活动路径)

            if (OnNavigationStarted != null)
            {
                OnNavigationStarted(); // ナビゲーション開始イベントを呼び出す (触发导航开始事件)
            }
        }

        /// <summary>
        /// テスト用パス (测试路径)
        /// </summary>
        private void TestPath(List<MapCubeClass> path)
        {
            OnNavigationPointReached += TestDestoryPath; // パス破壊イベントを登録 (注册路径销毁事件)
            Debug.Log(path[path.Count - 1]); // パスの最後のポイントをログ出力 (日志输出路径的最后一个点)
            foreach (var c in path)
            {
                if (c != null)
                {
                    c.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red; // パスのポイントを赤く表示 (将路径点显示为红色)
                }
            }
        }

        /// <summary>
        /// テスト用のパスを破壊 (销毁测试路径)
        /// </summary>
        private void TestDestoryPath(Vector2 vector2)
        {
            // 現在のポイントを白に戻す(将当前点恢复为白色)
            nowMapCube.transform.GetChild(0).
                GetComponent<SpriteRenderer>().color = Color.white; 
        }

        Vector3 moveVector = Vector3.zero; // 移動ベクトル (移动向量)

        /// <summary>
        /// メインループ (主循环)
        /// </summary>
        void Update()
        {
            if (nowMapCube == null) { return; } // 現在のキューブがない場合は終了 (如果当前方块不存在，则退出)

            if (map == null)
            {
                return; // マップがない場合は終了 (如果地图不存在，则退出)
            }

            if (!hasPath)
            {
                return; // パスがない場合は終了 (如果没有路径，则退出)
            }
            if (maxSpeed <= 0)
            {
                return; // 最大速度がゼロの場合は終了 (如果最大速度为零，则退出)
            }

            moveVector = ((Vector2)nextPoint.transform.position - position).normalized * maxSpeed; // 次のポイントに向けた移動ベクトルを計算 (计算指向下一个点的移动向量)
            transform.position += moveVector * Time.deltaTime; // 移動処理 (执行移动)

            // 到着したポイントをチェックする (检查是否到达点)
            if (hasPath)
            {
                // 次のポイントに十分に近づいた場合 (如果足够接近下一个点)
                if (((Vector2)(transform.position - nextPoint.transform.position)).sqrMagnitude <= 0.1f)
                {
                    transform.position = nextPoint.transform.position; // 次のポイントにスナップ (将位置对齐到下一个点)
                    activePath.RemoveAt(0); // パスから削除 (从路径中移除)

                    if (OnAnimateReached != null) LookAhead(); // アニメーションの処理 (处理动画)

                    if (!hasPath)
                    {
                        OnArrived(); // 最後のポイントに到着した場合、完了イベントを呼び出す (到达最后一个点时触发完成事件)
                        return;
                    }
                    else
                    {
                        if (OnNavigationPointReached != null)
                        {
                            OnNavigationPointReached(position); // ナビゲーションポイント到達イベントを呼び出す (触发导航点到达事件)
                        }
                    }
                }
            }
        }

        // NPOが前を見据える (让NPC朝向目标)
        void LookAhead()
        {
            if (nextPoint != null)
            {
                Vector2 dir = nextPoint.transform.position - transform.position; // 方向ベクトルを計算 (计算方向向量)
                OnAnimateReached(dir.normalized); // 正規化してアニメーションイベントを呼び出す (归一化后触发动画事件)
            }
        }

        // 目的地に到着した時 (到达目标时)
        void OnArrived()
        {
            Stop(); // 停止 (停止移动)

            if (ReachedCallback != null)
            {
                ReachedCallback(true); 
            }

            if (OnDestinationReached != null)
            {
                OnDestinationReached(); 
            }
        }

        // 無効な場合の処理 (目标无效时的处理)
        void OnInvalid()
        {
            if (ReachedCallback != null)
            {
                ReachedCallback(false); 
            }

            if (OnDestinationInvalid != null)
            {
                OnDestinationInvalid(); 
            }
        }

    }
}