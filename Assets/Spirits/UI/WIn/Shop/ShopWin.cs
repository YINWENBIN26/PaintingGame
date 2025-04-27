using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
public class ShopWin : UIBase
{
    // Start is called before the first frame update
    Transform ListTrans;
    ShopGrid shopGrid;
    public Dictionary<int, int> shoppingList = new Dictionary<int, int>();
    public Button BuyButton;
    protected override void Awake()
    {
        base.Awake();
        shopGrid = Resources.Load<ShopGrid>("UI/Gird/ShopGrid");
        ListTrans = transform.Find("BG/Scroll View/Viewport/Content");
        BuyButton = transform.Find("BuyButton").GetComponent<Button>();
        BuyButton.onClick.AddListener(delegate { BuyAll(); });
    }
    void BuyAll()
    {
        float price=0.0f;
       foreach(KeyValuePair<int,int> temp in shoppingList)
        {
            price += AlwaysInstanceManager._Instance.gameDictionary.ItemDict[temp.Key].Price * temp.Value;
        }
        if (price <= PlayerModel.GetModel.GetCoin)
        {
            PlayerModel.GetModel.CoinChange((int)- price);
            foreach (KeyValuePair<int, int> temp in shoppingList)
            {
                AlwaysInstanceManager._Instance.knapsack.knapsackList(temp.Key,+temp.Value);
            }
        }
        else
        {
            //提示金钱不讈E
        }
    }
    public void UpdateShow(List<int> shopList)
    {
        foreach (Transform temp in ListTrans)
        {
            Destroy(temp.gameObject);
        }
        foreach (int temp in shopList)
        {
            ShopGrid tempGrid = Instantiate(shopGrid);
            tempGrid.UpdateShow(temp);
            tempGrid.transform.SetParent(ListTrans, false);
        }
    }
    public void show(GameObject shop) //从选中目眮E匣竦肔ist来竵E律痰丒
    {
        base.show();
        List<int> temp = shop.GetComponent<ShopList>().shopList;
        UpdateShow(temp);
    }
    
}
*/