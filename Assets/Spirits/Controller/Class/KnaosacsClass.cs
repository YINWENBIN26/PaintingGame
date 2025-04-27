using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClass
{
    // Start is called before the first frame update
    public int id;
    public string ResName;
    public string Name;
    public int Price;
    public string Description;
    public int Attraction;
    public ItemType itemType;
    public ItemClass(int id, string ResName, string Name, int price,string Description,int type,int Attraction)
    {
        this.id = id;
        this.ResName = ResName;
        this.Name = Name;
        this.Price = price;
        this.Description = Description;
        this.itemType =(ItemType) type;
        this.Attraction = Attraction;
    }
}
public enum ItemType
{
    Food=1,
    Painting=2,
    ScreenShoot=3,
}
