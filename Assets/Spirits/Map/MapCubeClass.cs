using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{


    public class MapCubeClass : MonoBehaviour
    {
        // このキューブを通過するのに必要なコスト。(通过该方块所需的代价)
        public int needMoveConsumer = 1;
        // このキューブが移動可能かどうか。(该方块是否可移动)
        public bool isMoveable = true;
        // このキューブのタイプ。(该方块的类型)
        public MapCubeTybe cubeTybe = MapCubeTybe.Normal;

        void Start()
        {
            // ゲーム開始時にマップデータのリストに自身を追加し、キューブ上のユニットを確認します。(游戏开始时，将自身添加到地图数据列表，并检查方块上的单位)
            DataManager._Instance.mapdata.mapCubeList.Add(this);
            UnitOnMapCube();
        }

        // このキューブの隣接キューブを取得します。(获取该方块的相邻方块)
        public List<MapCubeClass> GetNeighborGameOBJ()
        {
            List<MapCubeClass> neighbors = new List<MapCubeClass>();
            // 前方、右方、左方、後方のキューブを検出し、隣接リストに追加します。(检测前方、右方、左方和后方的方块，并添加到邻居列表中)
            var collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, -1, 0), 0.2f, 1 << LayerMask.NameToLayer("MapPath"));
            if (collider != null && collider.CompareTag("MapCube"))
            {
                neighbors.Add(collider.GetComponent<MapCubeClass>());
            }
            collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, 1, 0), 0.2f, 1 << LayerMask.NameToLayer("MapPath"));
            if (collider != null && collider.CompareTag("MapCube"))
            {
                neighbors.Add(collider.GetComponent<MapCubeClass>());
            }
            collider = Physics2D.OverlapCircle(transform.position - new Vector3(1, 0, 0), 0.2f, 1 << LayerMask.NameToLayer("MapPath"));
            if (collider != null && collider.CompareTag("MapCube"))
            {
                neighbors.Add(collider.GetComponent<MapCubeClass>());
            }
            collider = Physics2D.OverlapCircle(transform.position - new Vector3(-1, 0, 0), 0.2f, 1 << LayerMask.NameToLayer("MapPath"));
            if (collider != null && collider.CompareTag("MapCube"))
            {
                neighbors.Add(collider.GetComponent<MapCubeClass>());
            }

            return neighbors;
        }

        // このキューブを通過するのに必要なコストを取得します（現在は固定値を返しますが、将来的には変更可能）。(获取通过该方块所需的代价，目前返回固定值，未来可能更改)
        public int GetNeedMvoeConsumer()
        {
            return 1;
        }

        // キューブ上にユニット（または障害物）が存在するかを検出し、キューブのタイプを設定します。(检测方块上是否存在单位（或障碍物），并设置方块类型)
        public void UnitOnMapCube()
        {
            var collider = Physics2D.OverlapCircle(transform.position, 0.5f, 1 << LayerMask.NameToLayer("Collider"));
            if (collider != null && collider.CompareTag("Collider"))
            {
                cubeTybe = MapCubeTybe.Collider;
                isMoveable = false;
            }
        }

        public int gCost; // 開始点からのコスト。(从起点到当前点的代价)
        public int hCost; // 目標点までのコスト。(到目标点的估算代价)
        public int fCost; // gCostとhCostの合計。(gCost和hCost的总和)
        public MapCubeClass parentCube; // 親キューブ（このキューブに至る最短経路上のキューブ）。(父方块，该方块到达路径上的前一个方块)

        // fCostを計算する。(计算fCost)
        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        // コストをリセットする。(重置代价)
        public void ResetCost()
        {
            gCost = int.MaxValue;
            hCost = int.MaxValue;
            parentCube = null;
        }
    }

    // キューブのタイプを定義する列挙型。(定义方块类型的枚举)
    public enum MapCubeTybe
    {
        Collider, // 移動不可能な障害物があるキューブ。(包含不可移动障碍物的方块)
        Normal, // 通常の移動可能なキューブ。(普通的可移动方块)
    }
}

