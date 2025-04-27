using BehaviourTree;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BehaviorTree.Deal

{
    public class DealBTNode : SerializedMonoBehaviour
    {
        [OdinSerialize, OpenView(ButtonName = "打开视图Button")]
        public BehaviourTreeData TreeData = new BehaviourTreeData();


        public void FixedUpdate()
        {
            TreeData?.Root?.Tick();
        }
        public GameObject BehaviorOBJ;
        #region Function
        protected virtual void OnValidate()
        {
            if (TreeData != null)
            {
                if (BehaviorOBJ != null)
                {
                    Entry entry = TreeData.Root as Entry;
                    entry.behaviorGameOBJ = BehaviorOBJ;
                }
            }
        }
#if UNITY_EDITOR
        [Button]
        void BuildTree()
        {
            TreeData = new BehaviourTreeData();
            Entry entry = new Entry();
            entry.behaviorGameOBJ = BehaviorOBJ == null ? this.gameObject : BehaviorOBJ;
            entry.Guid = System.Guid.NewGuid().ToString();
            entry.NodeType = NodeType.root;
            entry.NodeName = "Entry";
            TreeData.Root = entry;
            TreeData.NodeData.Add(entry);

        }
        [Button]
        private void LoadTree()
        {
            string path = EditorUtility.OpenFilePanel("Load Behavior Tree", "Assets/", "json");

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    TextAsset jsonFile = new TextAsset(System.IO.File.ReadAllText(path));
                    byte[] bytes = jsonFile.bytes;
                    TreeData = Sirenix.Serialization.SerializationUtility.DeserializeValue<BehaviourTreeData>(bytes, DataFormat.JSON);
                    Debug.Log(TreeData.Root);
                    ShowMessage("読み込み成功", "JSONファイルの読み込みに成功しました！");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("JSONファイルの読み込みに失敗しました: " + ex.Message);
                    ShowMessage("読み込み失敗", "JSONファイルの読み込みに失敗しました。ファイル形式を確認してください。");
                }
            }
            else
            {
                ShowMessage("読み込みキャンセル", "ファイルが選択されませんでした。");
            }
        }

        private void ShowMessage(string title, string message)
        {
            // 使用 Unity 的 EditorUtility.DisplayDialog 来显示消息框
            UnityEditor.EditorUtility.DisplayDialog(title, message, "确定");
        }

        [Button]

        public void SaveTree()
        {
            byte[] bytes = Sirenix.Serialization.SerializationUtility.SerializeValue(this.TreeData, DataFormat.JSON);

            string path = EditorUtility.SaveFilePanel("Save Behavior Tree", "Assets/", "BehaviorTree", "json");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                File.WriteAllBytes(path, bytes);
                ShowMessage("保存成功", "行動ツリーJSONファイルの保存に成功しました！");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("JSONファイルの保存に失敗しました: " + ex.Message);
                ShowMessage("保存失敗", "行動ツリーJSONファイルの保存に失敗しました。ファイルパスと権限を確認してください。");
            }
        }
#endif
        #endregion


    }


    public class DealJundge : BtPrecondition
    {
        [LabelText("価格レーベル"), SerializeField, FoldoutGroup("@NodeName")]
        public PriceSate priceSate;//1とても高い　2//高い　3//ふさわしい 4//安い

        public override BehaviourState Tick()
        {
            if(NodeState==BehaviourState.executing)
            {
                NodeState = ChildNode.Tick();
                return NodeState;
            }
            Entry entry = GetRoot() as Entry;
            Debug.Log(entry.behaviorGameOBJ.GetComponent<DealController>().JundgePrice());
            if ((int)entry.behaviorGameOBJ.GetComponent<DealController>().JundgePrice() <= (int)priceSate)
            {
                NodeState = ChildNode.Tick();
                return NodeState;
            }
            ChangeFailState();
            return BehaviourState.fail;

        }
    }
    //弹出价格窗口
    public class SetPrcie : BtActionNode
    {
        private bool _isDone=false;//一回のみ

        public override BehaviourState Tick()
        {
            if(NodeState== BehaviourState.sucess)
                return NodeState= BehaviourState.sucess;
            if (!_isDone)
            {
                Entry entry = GetRoot() as Entry;
                entry.behaviorGameOBJ.GetComponent<DealController>().ShowPricePanel();
                _isDone = true;
                return NodeState = BehaviourState.executing;
            }
            if (UIManager._Instance.dealWinInfo.gameObject.activeSelf)
            {
                return NodeState = BehaviourState.executing;
            }
            return NodeState = BehaviourState.sucess;
        }
    }
    //取消交易窗口
    public class CancelDeal : BtActionNode
    {
        public override BehaviourState Tick()
        {
            Entry entry = GetRoot() as Entry;
            entry.behaviorGameOBJ.GetComponent<DealController>().CancelDeal();
            return NodeState = BehaviourState.executing;
        }
    }
    //完成交易窗口
    public class FinshlDeal : BtActionNode
    {
        public override BehaviourState Tick()
        {
            Entry entry = GetRoot() as Entry;
            entry.behaviorGameOBJ.GetComponent<DealController>().FinishDeal();
            return NodeState = BehaviourState.executing;
        }
    }
    //对白窗口
    public class Diagloue : BtActionNode
    {
        private bool _isDone = false;
        [LabelText("対話Key"), SerializeField, FoldoutGroup("@NodeName")]
        public int index;
        [LabelText("会話内容"), SerializeField, FoldoutGroup("@NodeName")]
        public string dialgue;

        public override BehaviourState Tick()
        {
            if (NodeState == BehaviourState.sucess)
            {
                return BehaviourState.sucess;
            }
            if (!_isDone)
            {
                Debug.Log("IN dialgoue"+ dialgue);
                Entry entry = GetRoot() as Entry;
                entry.behaviorGameOBJ.GetComponent<DealController>().Showdialogue(dialgue);
                _isDone = true;

                return NodeState = BehaviourState.executing;
            }
            if (UIManager._Instance.dialogueUI.gameObject.activeSelf)
            {
                return NodeState = BehaviourState.executing;
            }
            return NodeState = BehaviourState.sucess;
        }
    }
    //十分なお金があるかどうかを判断する
    public class JudgeMoney: BtPrecondition
    {
        public override BehaviourState Tick()
        {

            if (NodeState == BehaviourState.executing|| NodeState==BehaviourState.sucess)
            {
                NodeState = ChildNode.Tick();
                return NodeState;
            }
            
            Entry entry = GetRoot() as Entry;
            if (entry.behaviorGameOBJ.GetComponent<DealController>().JundgeMoney())
            {
                NodeState = ChildNode.Tick();
                return NodeState;
            }
            ChangeFailState();
            return BehaviourState.fail;

        }

    }


}