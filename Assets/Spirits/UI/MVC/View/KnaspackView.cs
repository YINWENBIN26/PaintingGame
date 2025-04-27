
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KnaspackView : MonoBehaviour
{
    Transform ListTrans;
    SDPaintingGird paintingGird;
    Button btnClose;

    protected void Awake()
    {
        // グリッドをリソースからロードする (从资源中加载网格)
        paintingGird = Resources.Load<SDPaintingGird>("UI/Gird/SDPaintingGird");
        // リストのTransformを探す (查找列表的Transform)
        ListTrans = transform.Find("BG/Scroll View/Viewport/Content");
        // システムUIコントローラーにイベントを追加 (向系统UI控制器添加事件)
        SystemUIController._ctrl.AddEvent(UpdateInfo);

        // 閉じるボタンにクリックイベントを追加 (为关闭按钮添加点击事件)
        btnClose.onClick.AddListener(() =>
        {
            SystemUIController._ctrl.HideView(E_SystemUIPanel.KnapsackPanel);
        });
    }

    /// <summary>
    /// ナップザックの情報を更新します (更新背包的信息)
    /// </summary>
    /// <param name="knapsackModel"></param>
    public void UpdateInfo(KnapsackModel knapsackModel)
    {
        // 絵のリストを取得 (获取绘画列表)
        Dictionary<string, SDPaintingClass> paintingClassList = knapsackModel.GetPaintingList;

        // 既存のリストをクリア (清空现有列表)
        foreach (Transform temp in ListTrans)
        {
            Destroy(temp.gameObject);
        }

        if (paintingClassList != null)
        {
            // デフォルトのグリッドを作成 (创建默认网格)
            SDPaintingGird Grid = Instantiate(paintingGird);
            Grid.UpdateShow();
            // グリッドをリストに追加 (将网格添加到列表中)
            Grid.transform.SetParent(ListTrans, false);
        }

        // 絵のリストをループして表示 (遍历绘画列表并显示)
        foreach (KeyValuePair<string, SDPaintingClass> temp in paintingClassList)
        {
            // 各絵のグリッドを作成 (为每个绘画创建网格)
            SDPaintingGird tempGrid = Instantiate(paintingGird);
            tempGrid.UpdateShow(temp.Value);
            // グリッドをリストに追加 (将网格添加到列表中)
            tempGrid.transform.SetParent(ListTrans, false);
        }
    }
}