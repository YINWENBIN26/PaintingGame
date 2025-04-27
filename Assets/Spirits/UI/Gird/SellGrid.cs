using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
public class SellGrid : MonoBehaviour
{
    Image Icon;
    Text Name;
    Text Price;
    Text CountText;
    Button SelectBtn;
    public int price;
    private int goodsid;
        

    private void Awake()
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        Name = transform.Find("Name").GetComponent<Text>();
        Price = transform.Find("Price").GetComponent<Text>();
        CountText= transform.Find("Count/Text").GetComponent<Text>();
        SelectBtn = transform.Find("SelectBtn").GetComponent<Button>();
        SelectBtn.onClick.AddListener(delegate { Sell(); });
    }

    public void Sell()
    {
        if (goodsid>0) {
        AlwaysInstanceManager._Instance.gameContorller.nPCController.buyList.addSellList(AlwaysInstanceManager._Instance.gameContorller.currentSelectedGoodsShelf);//
        }
        else
        {
         AlwaysInstanceManager._Instance.knapsack.knapsackList(AlwaysInstanceManager._Instance.gameContorller.currentSelectedGoodsShelf.Goodsid, +1);//恢复库存
         AlwaysInstanceManager._Instance.gameContorller.nPCController.buyList.addSellList(AlwaysInstanceManager._Instance.gameContorller.currentSelectedGoodsShelf);//
        }
        //todo更新货架图标
        AlwaysInstanceManager._Instance.gameContorller.currentSelectedGoodsShelf.Goodsid = this.goodsid;
        
        //背包物品数量减少
        InstanceManager._Instance.selectedGoodsWIn.Hide();
    }

    //根据ID更新Gird的显示内容
    public void UpdateShow(int id,int count)
    {
        goodsid=id;
        ItemClass tempResName = AlwaysInstanceManager._Instance.gameDictionary.ItemDict[id];
        price = tempResName.Price;
        Price.text = tempResName.Price.ToString();
        CountText.text= count.ToString();
        int hashCode = tempResName.itemType.GetHashCode();
        string enumParseStr = ItemType.Parse(typeof(ItemType), hashCode.ToString()).ToString();
        Icon.sprite = Resources.Load<Sprite>("Icon/" + enumParseStr + "/" + tempResName.ResName);
        Name.text = tempResName.Name;
    }
    public void UpdateShow()
    {
        goodsid = 0;
        Price.text = "0";
        CountText.text = "0";
        Name.text = "下架此商品";
    }

}
*/