using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecletedPaintingWin : UIBase
{
    Transform ListTrans;
    SDPaintingGird paintingGird;
    protected override void Awake()
    {
        base.Awake();
        paintingGird = Resources.Load<SDPaintingGird>("UI/Gird/SDPaintingGrid");
        ListTrans = transform.Find("BG/Scroll View/Viewport/Content");
    }
    public void UpdateShow()
    {
        foreach (Transform temp in ListTrans)
        {
            Destroy(temp.gameObject);
        }
        if (GameStateManager._Instance.currentSelectedGoodsShelf.Painting!=null)
        {
            SDPaintingGird Grid = Instantiate(paintingGird);
            Grid.UpdateShow();
            Grid.transform.SetParent(ListTrans, false);//竵E孪录馨磁?
        }
        foreach (KeyValuePair<string,SDPaintingClass> temp in KnapsackModel.GetModel.GetPaintingList)
        {
            SDPaintingGird tempGrid = Instantiate(paintingGird);
            tempGrid.UpdateShow(temp.Value);
            tempGrid.transform.SetParent(ListTrans, false);
        }
    }
    public override void show() //show the
    {
        base.show();
        UpdateShow();
    }
}
