using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopList : MonoBehaviour
{
    public List<int> shopList = new List<int>();
    // Start is called before the first frame update
    private void Awake()
    {
        ReadShopList();
    }
    public TextAsset shopAsset;
    public void ReadShopList()//讀取超市列表
    {
        shopAsset=Resources.Load<TextAsset>("TextAssets/UI/Shop/" + this.transform.name);
        string proarray = shopAsset.ToString();
        
        string[] PershopStrArray = proarray.Split('|');
        foreach(string temp in PershopStrArray)
        {
            shopList.Add(int.Parse(temp));
        }

    }
}
