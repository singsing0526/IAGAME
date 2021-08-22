using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemAmount = 0, ID;

    public Item(string name, int amount, int ID)
    {
        itemName = name;
        itemAmount = amount;
        this.ID = ID;
    }
}
