using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;

public class SDBuyList
{
    //the list that npc can buy
    public List<SDGoodsShelf> SellList = new List<SDGoodsShelf>();

    //add goodshelf to list
    public void addSellList(SDGoodsShelf goodsShelf)
    {
        SellList.Add(goodsShelf);
    }
    //npc buy the goods
    public SDGoodsShelf buySDpainting(SDGoodsShelf goodsShelf, NPC.NPC nPC)
    {
        SellList.Remove(goodsShelf);
        return goodsShelf;

    }
    //cancel buying
    public void CancelSellList(SDGoodsShelf goodsShelf)
    {
        SellList.Add(goodsShelf);

    }
    //remove form list
    public void RomoveSellList(SDGoodsShelf goodsShelf)
    {
        SellList.Remove(goodsShelf);
    }
}