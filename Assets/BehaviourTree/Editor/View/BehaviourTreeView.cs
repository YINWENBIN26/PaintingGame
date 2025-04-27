using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace BehaviourTree.Editor.View
{



    [InitializeOnLoad]

    public class BehaviourTreeView : EditorWindow
    {
        public static BehaviourTreeView TreeWindow;
        public static BehaviourTreeData TreeData;

        public SplitView WindowRoot; // 分割ビューのルート

        [MenuItem("Tools/BehaviourTree/BehaviourTreeView")]
        public static void OpenView()
        {
            // エディターウィンドウを開く
            BehaviourTreeView wnd = GetWindow<BehaviourTreeView>();
            TreeWindow = wnd;
        }

        /// <summary>
        /// GUIをロード
        /// </summary>
        public void CreateGUI()
        {
            // ツリーデータを初期化
            BehaviourTreeDataInit(TreeData);
        }

        private void OnDestroy()
        {
            TreeData = null;
        }


        public void Refresh()
        {
            rootVisualElement.Clear(); // 既存の要素をクリア
            BehaviourTreeDataInit(TreeData); // ツリーデータを再初期化
        }

        // ツリーデータを初期化するメソッド
        public void BehaviourTreeDataInit(BehaviourTreeData treeData)
        {
            TreeWindow = this;
            VisualElement root = rootVisualElement;
            var visualTree = Resources.Load<VisualTreeAsset>("BehaviourTreeView");
            visualTree.CloneTree(root);
            WindowRoot = root.Q<SplitView>("SplitView");
            WindowRoot.TreeView = WindowRoot.Q<TreeView>();
            WindowRoot.InspectorView = WindowRoot.Q<InspectorView>();

            // ウィンドウサイズと位置を設定
            if (treeData == null) return;
            var tr = treeData.ViewTransform;
            if (tr != null)
            {
                WindowRoot.TreeView.viewTransform.position = tr.position;
                WindowRoot.TreeView.viewTransform.scale = tr.scale;
            }

            if (treeData.NodeData == null) return;
            treeData.NodeData.ForEach(n => CreateNodes(n, treeData.Root)); //ノードを作成する
            WindowRoot.TreeView.nodes.OfType<NodeView>().ForEach(n => n.AddEdge());//ツリー全体を作成
        }

        // ノードを作成するメソッド
        public void CreateNodes(BtNodeBase nodeData, BtNodeBase rootNode = null)
        {
            TreeView view = TreeWindow.WindowRoot.TreeView;
            NodeView nodeView = new NodeView(nodeData);
            if (nodeData == rootNode)
            {
                view.RootNode = nodeView;
            }
            nodeView.SetPosition(new Rect(nodeData.Position, Vector2.one));
            view.AddElement(nodeView);
        }
    }

    // 右クリックメニューのクラス


    public class OpenViewAttributeDrawer : OdinAttributeDrawer<OpenViewAttribute, BehaviourTreeData>
    {

        // プロパティのレイアウトを描画するメソッド
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // 次の描画メソッドを呼び出す
            this.CallNextDrawer(label);

            // ボタンを描画し、クリックされたらビューを開く
            if (GUILayout.Button(Attribute.ButtonName))
            {
                // ボタンがクリックされたら、BehaviourTreeViewのデータを設定し、ビューを開く
                BehaviourTreeView.TreeData = ValueEntry.SmartValue;
                BehaviourTreeView.OpenView();
            }
        }
    }

}
