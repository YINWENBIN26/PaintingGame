using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
public class ShopGrid : MonoBehaviour
{
     Image Icon;
     Text Name;
     Text Price;
     Text CountText;
     Button UpBtn;
     Button DownBtn;
     private int count=0;
     public int price;
     int Goodsid=0;

    public int Count
    {
        get => count;
        set { 
            
            if (count == 1&&value==0)//原來是1現在變為0
            {
                InstanceManager._Instance.buttonUIController.shopWin.shoppingList.Remove(Goodsid);
            }
            if(count == 0 && value == 1)
            {
                if(!InstanceManager._Instance.buttonUIController.shopWin.shoppingList.ContainsKey(Goodsid))
                InstanceManager._Instance.buttonUIController.shopWin.shoppingList.Add(Goodsid,value);
            }
            InstanceManager._Instance.buttonUIController.shopWin.shoppingList[Goodsid]=value;
            count = value;
            CountText.text = value.ToString();
            if (count > 0) 
            { 
               Price.text = (price * count).ToString();
            }
            else
            {
                Price.text = price .ToString();
            }
        }
    }

    private void Awake()
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        Name = transform.Find("Name").GetComponent<Text>();
        Price = transform.Find("Price").GetComponent<Text>();
        CountText = transform.Find("Count").GetComponent<Text>();
        UpBtn = transform.Find("UPBtn").GetComponent<Button>();
        DownBtn = transform.Find("DownBtn").GetComponent<Button>();
        UpBtn.onClick.AddListener(delegate { UpCount(); });
        DownBtn.onClick.AddListener(delegate { DownCount(); });
    }
    //根据ID更新Gird的显示内容
    public void UpdateShow(int id)
    {
        Goodsid = id;
        ItemClass tempResName = AlwaysInstanceManager._Instance.gameDictionary.ItemDict[id];
        price = (int)(tempResName.Price * 0.8);
        Price.text = price.ToString();
        int hashCode =tempResName.itemType.GetHashCode();
        string enumParseStr = ItemType.Parse(typeof(ItemType), hashCode.ToString()).ToString();
        Icon.sprite = Resources.Load<Sprite>("Icon/" + enumParseStr +"/" +tempResName.ResName);
        Name.text = tempResName.Name;
        CountText.text = 0.ToString();
    }
   public void UpCount()
    {
        Count += 1;
    }
    public void DownCount()
    {
        Count -= 1;
    }
}
*/