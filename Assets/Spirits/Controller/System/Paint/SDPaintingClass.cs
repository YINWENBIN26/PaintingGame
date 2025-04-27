using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDPaintingClass
{
    public string guid;
    public string name;
    public E_PaintingPrompt prompt1;//SDのPrompt1
    public E_PaintingPrompt prompt2;//SDのPrompt2
    public int price;
    public string description;
    public E_PaintingType paintingType;
    public int attraction;
    public string completeTime;
    public int costTime;
    public int rare;
    public SDPaintingClass(string guid, string name, int rare, int price, string description, E_PaintingType paintingType, E_PaintingPrompt prompt1, E_PaintingPrompt prompt2, int attraction)
    {
        this.guid = guid;
        this.price = price;
        this.description = description;
        this.paintingType = (E_PaintingType)paintingType;
        this.prompt1 = prompt1;
        this.prompt2 = prompt2;
        this.attraction = attraction;
        this.rare = rare;
    }
}


/// <summary>
/// 絵画のプロンプトを表す列挙型
/// </summary>
public enum E_PaintingPrompt
{
    OneDog, // 一匹の犬
    OneGirl, // 一人の女の子
    Kawai, // かわいい
    OneCat, // 一匹の猫
}

/// <summary>
/// 絵画のタイプを表す列挙型
/// </summary>
public enum E_PaintingType
{
    None=0,
    Icon, // アイコン
    Illustration // イラストレーション
}