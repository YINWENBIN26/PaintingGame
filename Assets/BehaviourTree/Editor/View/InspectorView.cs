using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor.View
{
    public class InspectorView : VisualElement
    {
        public IMGUIContainer inspectorBar;

        public InspectorDataView InspectorDataView;
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        // コンストラクタ
        public InspectorView()
        {
            Init(); // 初期化メソッドの呼び出し
        }

        // 初期化メソッド
        void Init()
        {
            // IMGUIコンテナを作成し、名前を設定
            inspectorBar = new IMGUIContainer() { name = "inspectorBar" };
            inspectorBar.style.flexGrow = 1; // レイアウト全体を占有
            CreateInspectorView();
            Add(inspectorBar); // インスペクターバーを追加
        }

        // 選択されたノードのインスペクターパネルを更新
        public void UpdateInspector()
        {
            // 選択されたデータをクリア
            InspectorDataView.selectDatas.Clear();
            // 選択されたノードを取得し、各ノードのデータを追加
            BehaviourTreeView.TreeWindow.WindowRoot.TreeView.selection.Select(node => node as NodeView)
                .ForEach(node =>
                {
                    if (node != null)
                    {
                        InspectorDataView.selectDatas.Add(node.Btnode);
                    }
                });
        }

        // インスペクタービューを作成するメソッド
        private async void CreateInspectorView()
        {
            InspectorDataView = Resources.Load<InspectorDataView>("InspectorDataView");


            await Task.Delay(100); // 待機しないとエラーが発生しやすい
            var odinEditor = UnityEditor.Editor.CreateEditor(InspectorDataView);

            // インスペクターバーにGUIハンドラを設定
            inspectorBar.onGUIHandler += () => { odinEditor.OnInspectorGUI(); };  // インスペクターGUIを描画};
        }




    }
}