using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Calculation;
using System;

namespace Map
{
    public class Mapdata : MonoBehaviour
    {
        public bool isNeedAccurately = true; // 精確な生成が必要かどうか (是否需要精确生成)
        public float accuracy = 0.5f; // 精度 (精确度)
        public PolygonCollider2D polygonCollider; 
        public List<MapCubeClass> mapCubeList = new List<MapCubeClass>(); 
        QuadTreeNode quadTree = null; // クワッドツリー (四叉树)
        private GameObject creatingMapCube; 
        private List<MapCubeClass> allMapCube; // すべてのマップキューブ (所有的地图方块)

        private void Start()
        {
            // マップの生成 (生成地图)
            CreateMap();
        }

        /// <summary>
        /// マップを生成する (生成地图)
        /// </summary>
        public void CreateMap()
        {
            if (isNeedAccurately)
            {
                AccuratelyGenerate(); // 正確に生成 (精确生成)
            }
            else
            {
                UnAccuratelyGenerate(); // 不正確に生成 (不精确生成)
            }
        }

        /// <summary>
        /// 正確にマップキューブを生成する (精确生成地图方块)
        /// </summary>
        private void AccuratelyGenerate()
        {
            Rect rect = GetBoundingBox(polygonCollider); // 最小の矩形を作成 (创建最小的矩形边界)
            quadTree = new QuadTreeNode(rect); // この範囲を記述 (初始化四叉树节点)
            if (!quadTree.Subdivide(polygonCollider)) Debug.Log("Fail"); // 分割に失敗した場合 (如果划分失败)
            allMapCube = DataManager._Instance.mapdata.mapCubeList;
            creatingMapCube = Resources.Load<GameObject>("Perfab/Mapcube"); // 追加機能を持つキューブをロード (加载具有附加功能的方块)
            GenerateGrid(rect, quadTree); // グリッドを生成 (生成网格)
        }

        /// <summary>
        /// 不正確にマップキューブを生成する (不精确生成地图方块)
        /// </summary>
        private void UnAccuratelyGenerate()
        {
            Rect rect = GetBoundingBox(polygonCollider); // 最小の矩形を作成 (创建最小的矩形边界)
            quadTree = new QuadTreeNode(rect); // この範囲を記述 (初始化四叉树节点)
            if (!quadTree.Subdivide(polygonCollider)) Debug.Log("Fail"); // 分割に失敗した場合 (如果划分失败)
            allMapCube = DataManager._Instance.mapdata.mapCubeList;
            creatingMapCube = Resources.Load<GameObject>("Perfab/Mapcube"); // 追加機能を持つキューブをロード (加载具有附加功能的方块)
            UnAccuratelyGenerateGrid(rect, quadTree); // グリッドを生成 (生成网格)
        }

        /// <summary>
        /// グリッドを生成する (生成网格)
        /// </summary>
        private void GenerateGrid(Rect rect, QuadTreeNode rootNode)
        {
            // 矩形内の各グリッドを走査 (遍历矩形内的每个网格)
            int transformNamex = 1, transformNamexy = 1;
            float rectXMax = rect.xMax - accuracy / 5;
            float rectYMax = rect.yMax - accuracy / 5;

            for (float x = rect.xMin + accuracy / 10; x < rectXMax; x += accuracy)
            {
                GameObject parentOBJ = new GameObject(transformNamex++.ToString()); // 新しい親オブジェクトを作成 (创建新的父对象)

                parentOBJ.transform.parent = polygonCollider.transform;
                parentOBJ.transform.localPosition = Vector3.zero;
                for (float y = rect.yMin + accuracy / 10; y < rectYMax; y += accuracy)
                {
                    Vector2 center = new Vector2(x + 0.5f, y + 0.5f); // グリッドの中心点を計算 (计算网格的中心点)

                    if (ShouldGenerateSquare(center, rootNode)) // 四角形を生成すべきか判断 (判断是否需要生成方块)
                    {
                        GameObject temp = Instantiate(creatingMapCube); // キューブをインスタンス化 (实例化方块)
                        temp.transform.name = transformNamexy++.ToString();

                        temp.transform.parent = parentOBJ.transform;
                        temp.transform.localScale = new Vector3(accuracy, accuracy, accuracy);
                        temp.transform.localPosition = center;
                    }
                }
            }
        }
        /// <summary>
        /// 不正確にグリッドを生成する
        /// </summary>
        private void UnAccuratelyGenerateGrid(Rect rect, QuadTreeNode rootNode)
        {
            // 矩形内の各グリッドを遍歴
            int transformNamex = 1, transformNamexy = 1;
            float rectXMax = rect.xMax - accuracy / 5;
            float rectYMax = rect.yMax - accuracy / 5;

            for (float x = rect.xMin + accuracy / 10; x < rectXMax; x += accuracy)
            {
                GameObject parentOBJ = new GameObject(transformNamex++.ToString());

                parentOBJ.transform.parent = polygonCollider.transform;
                parentOBJ.transform.localPosition = Vector3.zero;
                for (float y = rect.yMin + accuracy / 10; y < rectYMax; y += accuracy)
                {
                    Vector2 center = new Vector2(x + 0.5f, y + 0.5f); // 方格の中心点を計算

                    if (UnAccuratelyShouldGenerateSquare(center, rootNode))
                    {
                        GameObject temp = Instantiate(creatingMapCube);
                        temp.transform.name = transformNamexy++.ToString();

                        temp.transform.parent = parentOBJ.transform;
                        temp.transform.localScale = new Vector3(accuracy, accuracy, accuracy);
                        temp.transform.localPosition = center;
                    }
                }
            }
        }



        Rect CreateRectFromCenter(Vector2 center)
        {
            // 中心から矩形を作成 (从中心创建矩形)
            return new Rect(center.x - accuracy / 2, center.y - accuracy / 2, accuracy, accuracy);
        }

        // PolygonCollider2Dの境界ボックスを取得 (获取PolygonCollider2D的边界框)
        Rect GetBoundingBox(PolygonCollider2D polygon)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (Vector2 point in polygon.points)
            {
                if (point.x < minX) minX = point.x;
                if (point.x > maxX) maxX = point.x;
                if (point.y < minY) minY = point.y;
                if (point.y > maxY) maxY = point.y;
            }
            return new Rect(minX - accuracy / 10, minY - accuracy / 10, maxX - minX + accuracy / 5, maxY - minY + accuracy / 5);
        }

        // 方格を生成すべきか判断 (判断是否需要生成方块)
        private bool UnAccuratelyShouldGenerateSquare(Vector2 center, QuadTreeNode rootNode)
        {
            QuadTreeNode centerNode = rootNode.FindDeepestNode(center);
            if (centerNode == null) return false;
            if (centerNode.needCheck) return false;
            if (centerNode.bounds.Contains(center))
            {
                if (UnAccuratelyIsSquareInsideNode(center, rootNode))
                {
                    return true;
                }
            }
            return false;
        }

        // 方格がノード内にあるか判断 (判断方块是否在节点内)
        private bool UnAccuratelyIsSquareInsideNode(Vector2 center, QuadTreeNode node)
        {
            try
            {
                float halfSize = accuracy / 2;
                Vector2[] corners = {
            new Vector2(center.x - halfSize, center.y - halfSize),
            new Vector2(center.x + halfSize, center.y - halfSize),
            new Vector2(center.x + halfSize, center.y + halfSize),
            new Vector2(center.x - halfSize, center.y + halfSize)
        };

                foreach (var corner in corners)
                {
                    QuadTreeNode temp = node.FindDeepestNode(corner);
                    if (temp != null && !temp.needCheck)
                        return true;
                    else
                        return false;
                }
            }
            catch (NullReferenceException ex)
            {
            }
            return true;
        }

        // 正確な条件で方格を生成すべきか判断 (判断是否需要根据精确条件生成方块)
        private bool ShouldGenerateSquare(Vector2 center, QuadTreeNode node)
        {
            node = node.FindDeepestNode(center);
            if (node == null) return false;
            if (node.bounds.Contains(center))
            {
                if (!node.needCheck)
                {
                    if (IsSquareInsideNode(center, node))
                    {
                        return true;
                    }
                    else
                    {
                        return !IntersectClass.IsIncluding(CreateRectFromCenter(center), polygonCollider);
                    }
                }
                else
                {
                    return !IntersectClass.IsIncluding(CreateRectFromCenter(center), polygonCollider);
                }
            }
            print(5);
            return false;
        }

        // 方格の四隅がノード内にあるかチェック (检查方块的四个角是否在节点内)
        private bool IsSquareInsideNode(Vector2 center, QuadTreeNode node)
        {
            float halfSize = accuracy / 2;
            Vector2[] corners = {
        new Vector2(center.x - halfSize, center.y - halfSize),
        new Vector2(center.x + halfSize, center.y - halfSize),
        new Vector2(center.x + halfSize, center.y + halfSize),
        new Vector2(center.x - halfSize, center.y + halfSize)
    };
            foreach (var corner in corners)
            {
                if (!node.bounds.Contains(corner))
                {
                    // 一つでもノードの範囲外にある角があれば、falseを返す (如果有一个角在节点范围外，返回false)
                    return false;
                }
            }
            // すべての角がノード内にあれば、trueを返す (如果所有角都在节点内，返回true)
            return true;
        }


        /// <summary>
        /// A*による経路探索をリクエストする (请求A*路径搜索)
        /// </summary>
        /// <param name="stateCube">開始キューブ (起始方块)</param>
        /// <param name="targetCube">目標キューブ (目标方块)</param>
        /// <param name="callback">結果を返すコールバック (返回结果的回调函数)</param>
        public void RequestPathFindingList(MapCubeClass stateCube, MapCubeClass targetCube, System.Action<List<MapCubeClass>> callback)
        {
            List<MapCubeClass> result = new List<MapCubeClass>();
            MapCubeClass target = AstarPathFinds(stateCube, targetCube);
            while (target != null)
            {
                result.Add(target);
                target = target.parentCube;
            }

            callback(result);
        }

        /// <summary>
        /// A*アルゴリズムの主処理 (A*算法的主逻辑)
        /// </summary>
        /// <param name="stateCube">開始キューブ (起始方块)</param>
        /// <param name="targetCube">目標キューブ (目标方块)</param>
        /// <returns>探索結果のキューブ (搜索结果方块)</returns>
        private MapCubeClass AstarPathFinds(MapCubeClass stateCube, MapCubeClass targetCube)
        {
            // オープンリストとクローズリストを初期化 (初始化开启列表和关闭列表)
            List<MapCubeClass> openList = new List<MapCubeClass>();
            List<MapCubeClass> closeList = new List<MapCubeClass>();

            // 開始キューブをオープンリストに追加 (将起始方块添加到开启列表)
            openList.Add(stateCube);

            // コストを初期化 (初始化所有方块的代价)
            InitCost();

            // 開始キューブのGコストとHコストを設定 (设置起始方块的G代价和H代价)
            stateCube.gCost = 0;
            stateCube.hCost = CalculateDistanceCost(stateCube, targetCube);

            // オープンリストが空になるまでループ (循环直到开启列表为空)
            while (openList.Count > 0)
            {
                // オープンリストの中で最もFコストが低いキューブを取得 (从开启列表中获取F代价最低的方块)
                MapCubeClass cube = GetLowestFCostCube(openList);

                // 現在のキューブがターゲットキューブである場合、キューブを返す (如果当前方块是目标方块，返回该方块)
                if (cube == targetCube)
                {
                    return cube;
                }

                // 現在のキューブをオープンリストから削除し、クローズリストに追加 (将当前方块从开启列表移除并添加到关闭列表)
                openList.Remove(cube);
                closeList.Add(cube);

                // 現在のキューブの全ての隣接キューブを取得 (获取当前方块的所有相邻方块)
                List<MapCubeClass> neighborsList = cube.GetNeighborGameOBJ();
                foreach (MapCubeClass neighbor in neighborsList)
                {
                    // 隣接キューブが既にクローズリストまたはオープンリストにある場合、スキップ (跳过已在关闭列表或开启列表中的方块)
                    if (closeList.Contains(neighbor) || openList.Contains(neighbor))
                    {
                        continue;
                    }

                    // 隣接キューブが移動可能でない場合、クローズリストに追加してスキップ (如果相邻方块不可移动，添加到关闭列表并跳过)
                    if (!neighbor.isMoveable)
                    {
                        closeList.Add(neighbor);
                        continue;
                    }

                    // 隣接キューブのGコストとHコストを計算 (计算相邻方块的G代价和H代价)
                    neighbor.gCost = CalculateDistanceCost(neighbor, stateCube);
                    neighbor.hCost = CalculateDistanceCost(neighbor, targetCube);

                    // 隣接キューブのFコストを計算 (计算相邻方块的F代价)
                    neighbor.CalculateFCost();

                    // 隣接キューブの親キューブを現在のキューブに設定 (将相邻方块的父方块设置为当前方块)
                    neighbor.parentCube = cube;

                    // 隣接キューブをオープンリストに追加 (将相邻方块添加到开启列表)
                    openList.Add(neighbor);
                }
            }

            // オープンリストが空で経路が見つからない場合、デバッグ情報を出力してnullを返す (如果开启列表为空且未找到路径，输出调试信息并返回null)
            Debug.Log(stateCube);
            Debug.Log(targetCube);
            Debug.Log("No Path");
            return null;
        }

        // コストの初期化 (初始化代价)
        private void InitCost()
        {
            foreach (var cube in allMapCube)
            {
                cube.ResetCost();
            }
        }

        // 最小のFコストを持つキューブを取得 (获取F代价最小的方块)
        private MapCubeClass GetLowestFCostCube(List<MapCubeClass> list)
        {
            MapCubeClass lowestCostCube = list[0];
            foreach (var cube in list)
            {
                if (cube.fCost < lowestCostCube.fCost)
                {
                    lowestCostCube = cube;
                }
            }
            return lowestCostCube;
        }

        /// <summary>
        /// 二点間のマンハッタン距離を計算 (计算两点之间的曼哈顿距离)
        /// </summary>
        private int CalculateDistanceCost(MapCubeClass cubeA, MapCubeClass cubeB)
        {
            int xDistance = Mathf.Abs((int)(cubeA.transform.position.x - cubeB.transform.position.x));
            int yDistance = Mathf.Abs((int)(cubeA.transform.position.y - cubeB.transform.position.y));
            return xDistance + yDistance;
        }
    }
}