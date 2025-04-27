using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Calculation
{
    public static class IntersectClass
    {
        /// <summary>
        /// エッジが多角形と交差しているかを確認します (边是否与多边形相交)
        /// </summary>
        /// <param name="start">エッジの開始点 (边的起点)</param>
        /// <param name="end">エッジの終了点 (边的终点)</param>
        /// <param name="polygon">多角形 (多边形)</param>
        /// <returns>交差している場合はtrue、そうでない場合はfalse (如果相交返回true，否则返回false)</returns>
        public static bool IsEdgeIntersectingPolygon(Vector2 start, Vector2 end, PolygonCollider2D polygon)
        {
            for (int i = 0; i < polygon.pathCount; i++)
            {
                // 多角形の各パスを取得 (获取多边形的每条线)
                Vector2[] path = polygon.GetPath(i);
                for (int j = 0; j < path.Length; j++)
                {
                    Vector2 polyStart = path[j];
                    Vector2 polyEnd = path[(j + 1) % path.Length]; // 最初の点にループする (循环到第一个点)
                    if (LinesIntersect(start, end, polyStart, polyEnd))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 2つのラインが交差しているかを確認します (两条线是否相交)
        /// </summary>
        /// <param name="p1">ライン1の開始点 (线1的起点)</param>
        /// <param name="p2">ライン1の終了点 (线1的终点)</param>
        /// <param name="p3">ライン2の開始点 (线2的起点)</param>
        /// <param name="p4">ライン2の終了点 (线2的终点)</param>
        /// <returns>交差している場合はtrue、そうでない場合はfalse (如果相交返回true，否则返回false)</returns>
        public static bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            // 分母を計算 (计算分母)
            float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
            if (denominator == 0)
            {
                return false; // ラインは平行 (线是平行的)
            }

            // u値を計算 (计算u值)
            float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
            float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

            // 両方の線分上に交点があるかを確認 (检查交点是否在两条线段上)
            return (ua >= 0 && ua <= 1) && (ub >= 0 && ub <= 1);
        }

        /// <summary>
        /// 矩形が多角形内に含まれているかを確認します (矩形是否包含在多边形)
        /// </summary>
        /// <returns>含まれていない場合はtrue、完全に含まれている場合はfalse (如果不包含返回true，如果完全包含返回false)</returns>
        public static bool IsIncluding(Rect rect, PolygonCollider2D polygon)
        {
            // 四分木ノードの四つの角点を取得 (获取四叉树节点的四个角点)
            Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
            Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
            Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
            Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);

            // 各エッジが多角形の境界と交差しているかを確認 (检查每条边是否与多边形的边界相交)
            if (IntersectClass.IsEdgeIntersectingPolygon(topLeft, topRight, polygon) ||
                IntersectClass.IsEdgeIntersectingPolygon(topRight, bottomRight, polygon) ||
                IntersectClass.IsEdgeIntersectingPolygon(bottomRight, bottomLeft, polygon) ||
                IntersectClass.IsEdgeIntersectingPolygon(bottomLeft, topLeft, polygon))
            {
                return true; // さらに分割する必要あり (还需继续分割)
            }

            // 追加チェック: 四分木ノードの角点が多角形の中にない場合、かつ多角形の点がノード内にある場合 (额外检查，如果四个角点都不在多边形内，且多边形某点在节点内)
            if (polygon.OverlapPoint(topLeft + (Vector2)polygon.transform.position) &&
                polygon.OverlapPoint(topRight + (Vector2)polygon.transform.position) &&
                polygon.OverlapPoint(bottomLeft + (Vector2)polygon.transform.position) &&
                polygon.OverlapPoint(bottomRight + (Vector2)polygon.transform.position))
            {
                return false;
            }

            return true;
        }
    }
}