using BehaviourTree;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BehaviourTreeBase : SerializedMonoBehaviour
{
    [OdinSerialize, OpenView(ButtonName = "打开视图Button")]
    public  BehaviourTreeData TreeData ;


    public void FixedUpdate()
    {
        TreeData?.Root?.Tick();
    }

    public GameObject BehaviorOBJ;

#if UNITY_EDITOR
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
    #region Function
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

    #endregion
#endif



}
