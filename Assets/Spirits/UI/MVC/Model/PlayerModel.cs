using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerModel
{
    // イベントのリスナー用オブジェクトプール (事件监听器的对象池)
    private Dictionary<string, UnityAction<PlayerModel>> eventData = new Dictionary<string, UnityAction<PlayerModel>>();
    // 通貨量 (货币量)
    private int coin = 50;
    // 魅力値 (魅力值)
    private int attration = 10;
    // 移動速度 (移动速度)
    private int speed = 3000;
    // 絵のプロンプトのセット (绘画提示的集合)
    private HashSet<E_PaintingPrompt> paintingPromptSet = new HashSet<E_PaintingPrompt>();
    // 最新のプロンプト (最新的绘画提示)
    E_PaintingPrompt newPrompt;
    // モデルのシングルトンインスタンス (模型的单例实例)
    private static PlayerModel _model;
    /// <summary>
    /// モデルのインスタンスを取得します。 (获取模型的实例)
    /// </summary>
    public static PlayerModel GetModel
    {
        get
        {
            if (_model == null)
                _model = new PlayerModel();
            return _model;
        }
    }

    /// 絵のプロンプトセットを取得します。 (获取绘画提示集合)
    public HashSet<E_PaintingPrompt> GetPaintingPromptSet { get => paintingPromptSet; }
    public int GetCoin { get => coin; }
    public int GetAttration { get => attration; }
    public int GetSpeed { get => speed; }
    public E_PaintingPrompt GetNewPrompt { get => this.newPrompt; }

    /// <summary>
    /// 通貨量の変更を処理します。 (处理货币量的变化)
    /// </summary>
    /// <param name="value">変更する通貨の量 (要更改的货币量)</param>
    public void CoinChange(int value)
    {
        coin += value;
        if (eventData.ContainsKey("CoinChange"))
            UpdateCoin();
    }

    /// <summary>
    /// 魅力値の変更を処理します。 (处理魅力值的变化)
    /// </summary>
    /// <param name="attration">変更する魅力値 (要更改的魅力值)</param>
    public void AttrationChange(int attration)
    {
        this.attration += attration;
        if (eventData.ContainsKey("AttrationChange"))
            UpdateAttration();
    }

    /// 魅力値変更イベントを実行します。 (执行魅力值更改事件)
    private void UpdateAttration()
    {
        if (eventData["AttrationChange"] != null)
            eventData["AttrationChange"].Invoke(this);
    }

    /// <summary>
    /// 魅力値変更イベントのリスナーを追加します。 (添加魅力值更改事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void AddAttrationChangeListener(UnityAction<PlayerModel> function)
    {
        if (!eventData.ContainsKey("AttrationChange"))
            eventData.Add("AttrationChange", function);
        else if (eventData.ContainsKey("AttrationChange") && !eventData["AttrationChange"].Equals(function))
            eventData["AttrationChange"] += function;
    }

    /// <summary>
    /// 魅力値変更イベントのリスナーを削除します。 (移除魅力值更改事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void RemoveAttrationChangeListener(UnityAction<PlayerModel> function)
    {
        if (eventData.ContainsKey("AttrationChange") &&
            eventData["AttrationChange"].Equals(function))
            eventData["AttrationChange"] -= function;
    }

    /// 通貨変更イベントを実行します。 (执行货币更改事件)
    private void UpdateCoin()
    {
        if (eventData["CoinChange"] != null)
            eventData["CoinChange"].Invoke(this);
    }

    /// <summary>
    /// 通貨変更イベントのリスナーを追加します。 (添加货币更改事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void AddCoinChangeListener(UnityAction<PlayerModel> function)
    {
        if (!eventData.ContainsKey("CoinChange"))
            eventData.Add("CoinChange", function);
        else if (eventData.ContainsKey("CoinChange") && !eventData["CoinChange"].Equals(function))
            eventData["CoinChange"] += function;
    }

    /// <summary>
    /// 通貨変更イベントのリスナーを削除します。 (移除货币更改事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void RemoveCoinChangeListener(UnityAction<PlayerModel> function)
    {
        if (eventData.ContainsKey("CoinChange") &&
            eventData["CoinChange"].Equals(function))
            eventData["CoinChange"] -= function;
    }

    /// <summary>
    /// 絵のプロンプトを追加します。 (添加绘画提示的监听器)
    /// </summary>
    /// <param name="PaintingPrompt"></param>
    public void AddPaintingPromptSet(E_PaintingPrompt PaintingPrompt)
    {
        paintingPromptSet.Add(PaintingPrompt);
        newPrompt = PaintingPrompt;
        if (eventData.ContainsKey("AddPaintingPrompt"))
            eventData["AddPaintingPrompt"].Invoke(this);
    }

    /// <summary>
    /// 絵のプロンプトを削除します。 (移除绘画提示的监听器)
    /// </summary>
    /// <param name="PaintingPrompt"></param>
    public void DeletePaintingPromptSet(E_PaintingPrompt PaintingPrompt)
    {
        paintingPromptSet.Remove(PaintingPrompt);
        if (eventData.ContainsKey("RemovePaintingPrompt"))
            eventData["RemovePaintingPrompt"].Invoke(this);
    }

    /// <summary>
    /// 絵のプロンプトイベントのリスナーを追加します。 (添加绘画提示事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void AddPaintingPromptSetListener(UnityAction<PlayerModel> function)
    {
        if (!eventData.ContainsKey("AddPaintingPrompt"))
            eventData.Add("AddPaintingPrompt", function);
        else if (eventData.ContainsKey("AddPaintingPrompt") && !eventData["AddPaintingPrompt"].Equals(function))
            eventData["AddPaintingPrompt"] += function;
    }

    /// <summary>
    /// 絵のプロンプトイベントのリスナーを削除します。 (移除绘画提示事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void RemovePaintingPromptSetListener(UnityAction<PlayerModel> function)
    {
        if (eventData.ContainsKey("RemovePaintingPrompt") &&
            eventData["RemovePaintingPrompt"].Equals(function))
            eventData["RemovePaintingPrompt"] -= function;
    }
}





