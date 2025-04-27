using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor.View
{
    public class TreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits> { }

        public NodeView RootNode; // ルートノード
        public Dictionary<string, NodeView> NodeViewDict; // ノードビューの辞書

        public TreeView()
        {
            // グリッド背景を挿入
            Insert(0, new GridBackground());

            // スタイルシートをロードして追加
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/Resources/BehaviourTreeView.uss"));

            // 右クリックメニューを追加
            RightButtonMenu();

            // マウスが要素に入ったときのイベントを登録
            RegisterCallback<MouseEnterEvent>(MouseEnterControl);
            this.graphViewChanged = OnGraphViewChanged;
            NodeViewDict = new Dictionary<string, NodeView>();

            // ズーム機能を追加
            this.AddManipulator(new ContentZoomer());
            // コンテンツのドラッグ機能を追加
            this.AddManipulator(new ContentDragger());
            // 選択してドラッグする機能を追加
            this.AddManipulator(new SelectionDragger());
            // 矩形選択機能を追加
            this.AddManipulator(new RectangleSelector());
        }


        // ノードを追加する際に辞書に挿入する
        public new void AddElement(GraphElement graphElement)
        {
            base.AddElement(graphElement);
            if (graphElement is NodeView nodeView)
            {
                NodeViewDict.Add(nodeView.Btnode.Guid, nodeView); // ノードビューを辞書に追加
            }
        }

        // マウスが要素に入ったときの処理
        private void MouseEnterControl(MouseEnterEvent evt)
        {
            BehaviourTreeView.TreeWindow.WindowRoot.InspectorView.UpdateInspector();
        }

        // グラフビューが変更されたときの処理
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // 作成されたエッジを処理
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    View.BehaviourTrerEditorExTools.AddNodeData(edge);
                }
            }

            // 削除された要素を処理
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var etr in graphViewChange.elementsToRemove)
                {
                    if (etr is Edge edge)
                    {
                        View.BehaviourTrerEditorExTools.RemoveLink(edge); // リンクを削除
                    }
                    if (etr is NodeView view)
                    {
                        NodeViewDict.Remove(view.Btnode.Guid); // ノードビューを辞書から削除
                        BehaviourTreeView.TreeData.NodeData.Remove(view.Btnode); // ノードデータを削除
                    }
                }
            }
            return graphViewChange;
        }

        #region 右クリックメニュー

        private RightClickMenu rightClickMenu; // 右クリックメニューのプロバイダー

        public void RightButtonMenu()
        {
            rightClickMenu = ScriptableObject.CreateInstance<RightClickMenu>();
            rightClickMenu.OnSelectEntryHandler = OnMenuSelectEntry;

            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), rightClickMenu);
            };
        }

        // 右クリックメニューを構築
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Create Node", actionEvent =>
            {
                var windowRoot = BehaviourTreeView.TreeWindow.rootVisualElement;
                var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent,actionEvent.eventInfo.mousePosition + BehaviourTreeView.TreeWindow.position.position);
                SearchWindow.Open(new SearchWindowContext(windowMousePosition), rightClickMenu);
            });
            base.BuildContextualMenu(evt);
        }

        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = BehaviourTreeView.TreeWindow.rootVisualElement;

            // ウィンドウ上のマウス位置を取得
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - BehaviourTreeView.TreeWindow.position.position);

            // グラフ上のマウス位置を取得
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);

            // 反射を使用してノードを作成
            var nodeBase = Activator.CreateInstance((Type)searchTreeEntry.userData) as BtNodeBase;

            // ノードの初期設定を行う
            nodeBase.NodeName = nodeBase.GetType().Name;
            nodeBase.NodeType = nodeBase.GetType().GetNodeType();
            nodeBase.Position = graphMousePosition;
            nodeBase.Guid = Guid.NewGuid().ToString();
            NodeView group = new NodeView(nodeBase);
            group.SetPosition(new Rect(graphMousePosition, Vector2.one));
            this.AddElement(group); // グラフにノードを追加
            BehaviourTreeView.TreeData.NodeData.Add(nodeBase); // ノードデータを追加
            AddToSelection(group); // 選択状態にする
            return true;
        }

        // リンクルールを定義
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return ports.Where(endPorts => endPorts.direction != startAnchor.direction && endPorts.node != startAnchor.node).ToList();
        }

        #endregion
    }
    public class RightClickMenu : ScriptableObject, ISearchWindowProvider
    {
        public delegate bool SelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

        public SelectEntryDelegate OnSelectEntryHandler; // 選択エントリのデリゲート

        //ISearchWindowProvider
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
            entries = AddNodeType<BtComposite>(entries, "CompositeNode");
            entries = AddNodeType<BtPrecondition>(entries, "PreconditionNode");
            entries = AddNodeType<BtActionNode>(entries, "ActionNode");
            return entries;
        }

        /// <summary>
        /// リフレクションを使用して対応するメニューデータを取得
        /// </summary>
        public List<SearchTreeEntry> AddNodeType<T>(List<SearchTreeEntry> entries, string menuName)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(menuName)) { level = 1 });
            List<System.Type> rootNodeTypes = GetDerivedClasses(typeof(T));
            foreach (var rootType in rootNodeTypes)
            {
                if (rootType.Name == "Entry")
                    continue;
                string rootName = rootType.Name;
                entries.Add(new SearchTreeEntry(new GUIContent(rootName)) { level = 2, userData = rootType });
            }
            return entries;
        }

        // リフレクションを使用してメニューの対応するデータ型を取得
        public List<Type> GetDerivedClasses(Type type)
        {
            List<Type> derivedClasses = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))
                    {
                        derivedClasses.Add(t);
                    }
                }
            }
            return derivedClasses;
        }

        // 選択エントリの処理
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntryHandler == null)
            {
                return false;
            }
            return OnSelectEntryHandler(searchTreeEntry, context);
        }
    }




}