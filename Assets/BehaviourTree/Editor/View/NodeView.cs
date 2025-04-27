using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourTree;
using BehaviourTree.Editor.View;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace BehaviourTree.Editor.View
{
    public class NodeView : Node
    {

        [LabelText("ノードビューのデータ"), OdinSerialize, HideReferenceObjectPicker]
         public BtNodeBase Btnode;

        [LabelText("入力ポート"), HideIf("@true")] public Port InputPort;
        [LabelText("出力ポート"), HideIf("@true")] public Port OutputPort;

        // ノードビューのコンストラクタ
        public NodeView(BtNodeBase node)
        {
            this.Btnode = node;
            InitNodeView();
        }

        // ノードビューを初期化するメソッド
        private void InitNodeView()
        {
            title = Btnode.NodeName;
            switch (Btnode.NodeType)
            {
                case NodeType.composite:
                    InputPort = PortInPut(this);
                    OutputPort = PortOutPut(this, false);
                    inputContainer.Add(InputPort);
                    outputContainer.Add(OutputPort);
                    break;
                case NodeType.pecondition:
                    InputPort = PortInPut(this);
                    OutputPort = PortOutPut(this, true);
                    inputContainer.Add(InputPort);
                    outputContainer.Add(OutputPort);
                    break;
                case NodeType.action:
                    InputPort = PortInPut(this);
                    inputContainer.Add(InputPort);
                    break;
                case NodeType.root:
                    OutputPort = PortOutPut(this, true);
                    outputContainer.Add(OutputPort);
                    break;
            }
        }

        // 子ノードを検索し、エッジを追加するメソッド
        public void AddEdge()
        {
            TreeView view = BehaviourTreeView.TreeWindow.WindowRoot.TreeView;
            switch (Btnode)
            {
                case BtComposite composite:
                    foreach (var t in composite.ChildNodes)
                    {
                        LinkNodes(OutputPort, view.NodeViewDict[t.Guid].InputPort);
                    }
                    break;
                case BtPrecondition precondition:
                    if (precondition.ChildNode == null) break;
                    LinkNodes(OutputPort, view.NodeViewDict[precondition.ChildNode.Guid].InputPort);
                    break;
            }
        }

        // 子ノードを追加するメソッド
        public void AddChildNode(NodeView node)
        {
            switch (Btnode)
            {
                case BtComposite composite:
                    composite.ChildNodes.Add(node.Btnode);
                    break;
                case BtPrecondition precondition:
                    precondition.ChildNode = node.Btnode;
                    break;
            }
        }

        // 子ノードを削除するメソッド
        public void RemoveChildNode(NodeView node)
        {
            switch (Btnode)
            {
                case BtComposite composite:
                    composite.ChildNodes.Remove(node.Btnode);
                    break;
                case BtPrecondition precondition:
                    if (precondition.ChildNode == node.Btnode)
                    {
                        precondition.ChildNode = null;
                    }
                    break;
            }
        }

        // ノード間のエッジを作成しリンクするメソッド
        void LinkNodes(Port output, Port input)
        {
            var temp = new Edge()
            {
                output = output,
                input = input
            };
            temp?.input.Connect(temp);
            temp?.output.Connect(temp);
            BehaviourTreeView.TreeWindow.WindowRoot.TreeView.Add(temp);
        }

        // 入力ポートを作成するメソッド
        public static Port PortInPut(NodeView nodeView)
        {
            Port port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, null);
            port.portName = "Input";
            return port;
        }

        // 出力ポートを作成するメソッド
        public static Port PortOutPut(NodeView nodeView, bool isSingle)
        {
            Port port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, isSingle ? Port.Capacity.Single : Port.Capacity.Multi, null);
            port.portName = "Output";
            return port;
        }

        // ノードが選択されたときの処理
        public override void OnSelected()
        {
            base.OnSelected();
            BehaviourTreeView.TreeWindow.WindowRoot.InspectorView.UpdateInspector();
        }

        // ノードの選択が解除されたときの処理
        public override void OnUnselected()
        {
            base.OnUnselected();
            BehaviourTreeView.TreeWindow.WindowRoot.InspectorView.UpdateInspector();
        }

        // ノードの位置を設定するメソッド
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Btnode.Position = new Vector2(newPos.xMin, newPos.yMin);
        }

    } 
}