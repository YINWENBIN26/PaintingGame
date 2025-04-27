
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 将来新しいパネルを追加するための列挙型を提供します (提供一个枚举类型，用于未来添加新面板)
/// </summary>
public enum E_SystemUIPanel
{
    StorePanel,
    KnapsackPanel,
}

/// <summary>
/// ケースで唯一のControllerです (案例中唯一的控制器)
/// </summary>
public class SystemUIController : MonoBehaviour
{
    // Modelを取得 (获取模型)
    private KnapsackModel model = null;
    // MainViewを取得 (获取主视图)
    private KnaspackView knaspackView = null;
    // RoleViewを取得 (获取角色视图)
    // private KnaspackView storePanel = null;
    // パネルのオブジェクトプールを保存 (保存面板对象池)
    private Dictionary<E_SystemUIPanel, GameObject> panelDic = new Dictionary<E_SystemUIPanel, GameObject>();

    /// <summary>
    /// 外部から使用するための唯一のグローバルフィールドを提供します (提供一个唯一的全局字段供外部使用)
    /// </summary>
    public static SystemUIController _ctrl;

    private void Awake()
    {
        _ctrl = this;
    }

    private void Start()
    {
        model = KnapsackModel.GetModel;
    }

    /// <summary>
    /// パネルの表示と非表示を制御します (控制面板的显示与隐藏)
    /// </summary>
    /// <param name="panelName">表示するパネルの名前 (要显示的面板名称)</param>
    public void ShowView(E_SystemUIPanel panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].SetActive(true);
            return;
        }
        GameObject go = null;

        switch (panelName)
        {
            case E_SystemUIPanel.StorePanel:
                go = Instantiate(Resources.Load<GameObject>("UI/KnaspackPanel"));
                knaspackView = go.GetComponent<KnaspackView>();
                knaspackView.UpdateInfo(model);
                break;
                /*
            case E_SystemUIPanel.KnapsackPanel:
                go = Instantiate(Resources.Load<GameObject>("UI/RolePanel"));
                roleView = go.GetComponent<RoleView>();
                roleView.UpdateInfo(model.GetHp, model.GetPlayerLev, model.GetAtk, model.GetDef, model.GetCrit, model.GetMiss, model.GetLuck);
                break;
                */
        }

        go.SetActive(true);
        go.transform.SetParent(GameObject.Find("Canvas").transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        (go.transform as RectTransform).offsetMax = Vector3.zero;
        (go.transform as RectTransform).offsetMin = Vector3.zero;

        AddPool(panelName, go);
    }

    /// <summary>
    /// パネルを非表示にします (隐藏面板)
    /// </summary>
    /// <param name="panelName">非表示にするパネルの名前 (要隐藏的面板名称)</param>
    public void HideView(E_SystemUIPanel panelName)
    {
        if (panelDic.ContainsKey(panelName) && panelDic[panelName].activeSelf == true)
        {
            panelDic[panelName].SetActive(false);
        }
    }

    /// <summary>
    /// 非表示にしたパネルをオブジェクトプールに追加します (将隐藏的面板添加到对象池中)
    /// </summary>
    /// <param name="panelName">非表示にしたパネルの名前 (隐藏的面板名称)</param>
    /// <param name="go">シーンで実際に非表示にするUIパネ  ル (场景中实际隐藏的UI面板)</param>
    public void AddPool(E_SystemUIPanel panelName, GameObject go)
    {
        if (!panelDic.ContainsKey(panelName))
            panelDic.Add(panelName, go);
    }

    /// <summary>
    /// レベルアップイベントのリスナーを追加します (添加升级事件的监听器)
    /// </summary>
    /// <param name="function"></param>
    public void AddEvent(UnityAction<KnapsackModel> function)
    {
        model.AddLevListener(function);
    }

    /// <summary>
    /// レベルアップイベントのリスナーを削除します (移除升级事件的监听器)
    /// </summary>
    /// <param name="function">削除する関数 (要移除的事件)</param>
    public void RemoveLevelUpListener(UnityAction<KnapsackModel> function)
    {
        model.RemoveLevListener(function);
    }
}