using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using Unity.VisualScripting;
using UnityEngine;
using Calculation;

namespace Map
{

    public class QuadTreeNode
    {
        public readonly Rect bounds; 
        public QuadTreeNode[] children; // 子ノード (子节点)
        public bool needCheck; // チェックが必要か (是否需要检查)

        public QuadTreeNode(Rect bounds)
        {
            this.bounds = bounds; 
            needCheck = true; 
            children = null; 
        }

        /// <summary>
        /// 範囲を分割する (分割范围)
        /// </summary>
        /// <param name="polygon">多角形コライダー (多边形碰撞体)</param>
        public bool Subdivide(PolygonCollider2D polygon)
        {
            if (!IntersectClass.IsIncluding(this.bounds, polygon))
            {
                // 範囲が多角形の外にある場合、分割を停止 (如果范围完全或部分位于多边形外部，则停止分割)
                needCheck = false;
                return false;
            }

            if (bounds.width < 1.0f)
            {
                // 分割範囲が最小の場合 (当分割范围达到最小值时)
                return false; 
            }

            float halfWidth = bounds.width / 2; 
            float halfHeight = bounds.height / 2; 

            // 範囲を4つに分割 (将范围分为四个部分)
            children = new QuadTreeNode[4];
            children[0] = new QuadTreeNode(new Rect(bounds.x, bounds.y, halfWidth, halfHeight));
            children[1] = new QuadTreeNode(new Rect(bounds.x + halfWidth, bounds.y, halfWidth, halfHeight));
            children[2] = new QuadTreeNode(new Rect(bounds.x, bounds.y + halfHeight, halfWidth, halfHeight));
            children[3] = new QuadTreeNode(new Rect(bounds.x + halfWidth, bounds.y + halfHeight, halfWidth, halfHeight));

            foreach (var child in children)
            {
                if (child.Subdivide(polygon))
                {
                    continue;
                }
            }

            return true; // 分割成功 (分割成功)
        }

        /// <summary>
        /// 指定されたポイントを含む最も深いノードを探す (查找包含指定点的最深节点)
        /// </summary>
        /// <param name="point">ポイント (点)</param>
        /// <returns>最深ノード (最深的节点)</returns>
        public QuadTreeNode FindDeepestNode(Vector2 point)
        {
            // 現在のノードがポイントを含むかを確認 (检查当前节点是否包含该点)
            // 含まない場合はnullを返す (如果不包含则返回null)
            if (!bounds.Contains(point))
            {
                return null; 
            }

            if (children == null)
            {
                return this; // 子ノードがない場合は現在のノードを返す (如果没有子节点，则返回当前节点)
            }

            // 子ノードを調べ、ポイントを含むノードがあれば再帰的に呼び出す (遍历子节点，如果子节点包含该点，则递归调用)
            foreach (var child in children)
            {
                if (child != null)
                {
                    QuadTreeNode node = child.FindDeepestNode(point);
                    if (node != null)
                    {
                        return node; 
                    }
                }
            }

            // 子ノードがポイントを含まない場合、または現在のノードが子ノードを持たない場合は現在のノードを返す
            // (如果子节点不包含该点或当前节点没有子节点，则返回当前节点)
            return this;
        }
    }
}