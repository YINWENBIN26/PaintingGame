using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTree.Editor.View
{
    public static class BehaviourTrerEditorExTools
    {
        /// <summary>
        /// 连接时添加数据
        /// </summary>
        public static void AddNodeData(this Edge edge)
        {
            NodeView outNodeView = edge.output.node as NodeView;
            NodeView inNodeView = edge.input.node as NodeView;

            inNodeView.Btnode.parent = outNodeView.Btnode;
            outNodeView?.AddChildNode(inNodeView);
        }
    
        /// <summary>
        /// 删除连接时清除数据
        /// </summary>
        public static void RemoveLink(this Edge edge)
        {
            NodeView outNodeView = edge.output.node as NodeView;
            NodeView inNodeView = edge.input.node as NodeView;
            inNodeView.Btnode.parent=null;
            outNodeView?.RemoveChildNode(inNodeView);
            
        }
        

    }


}
