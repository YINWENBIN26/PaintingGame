using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// リュックサックモデルを表すクラス
public class KnapsackModel
{
    // イベントをリスンするためのオブジェクトプール (用于监听事件的对象池)
    private Dictionary<string, UnityAction<KnapsackModel>> eventData = new Dictionary<string, UnityAction<KnapsackModel>>();
    // 絵のリスト (绘画列表)
    private Dictionary<string, SDPaintingClass> paintingList = new Dictionary<string, SDPaintingClass>();
    // モデルのシングルトンインスタンス (模型的单例实例)
    private static KnapsackModel _model;
    /// <summary>
    /// モデルのインスタンスを取得します。 (获取模型的实例)
    /// </summary>
    public static KnapsackModel GetModel
    {
        get
        {
            if (_model == null)
                _model = new KnapsackModel();
            return _model;
        }
    }

    /// <summary>
    /// 絵のリストを取得します。 (获取绘画列表)
    /// </summary>
    public Dictionary<string, SDPaintingClass> GetPaintingList { get => paintingList; }

    /// <summary>
    /// 絵のリストに新しい絵を追加します。 (向绘画列表中添加新绘画)
    /// </summary>
    /// <param name="paintingClass">追加する絵 (要添加的绘画)</param>
    public void paintingListAdd(SDPaintingClass paintingClass)
    {
        paintingList.Add(paintingClass.guid, paintingClass);
    }

    /// <summary>
    /// 絵のリストから指定された絵を削除します。 (从绘画列表中移除指定的绘画)
    /// </summary>
    /// <param name="guid">削除する絵のGUID (要移除的绘画的GUID)</param>
    /// <returns>削除された絵 (被移除的绘画)</returns>
    public SDPaintingClass paintingListDelete(string guid)
    {
        SDPaintingClass temp = paintingList[guid];
        paintingList.Remove(guid);
        return temp;
    }

    /// <summary>
    /// リュックサックリストを更新する (更新背包列表)
    /// </summary>
    /// <param name="painting"></param>
    private void UpdataknapsackList(SDPaintingClass painting)
    {
        if (eventData["AddPating"] != null)
            eventData["AddPating"].Invoke(this);
    }

    /// <summary>
    /// リュックサックリストから削除する (从背包列表中移除)
    /// </summary>
    private void RemoveKnapsackList()
    {
        if (eventData["ReomovePating"] != null)
            eventData["ReomovePating"].Invoke(this);
    }

    /// <summary>
    /// レベルアップイベントのリスナーを追加します (添加升级事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void AddLevListener(UnityAction<KnapsackModel> function)
    {
        if (!eventData.ContainsKey("AddPating"))
            eventData.Add("AddPating", function);
        else if (eventData.ContainsKey("AddPating") && !eventData["AddPating"].Equals(function))
            eventData["AddPating"] += function;
    }

    /// <summary>
    /// 外部から使用するためのレベルアップイベントのリスナーを削除します (移除供外部使用的升级事件监听器)
    /// </summary>
    /// <param name="function"></param>
    public void RemoveLevListener(UnityAction<KnapsackModel> function)
    {
        if (eventData.ContainsKey("ReomovePating") &&
            eventData["ReomovePating"].Equals(function))
            eventData["ReomovePating"] -= function;
    }
}