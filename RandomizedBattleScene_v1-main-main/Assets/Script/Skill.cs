using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public Character.Element element;
    public int MPCost, ID;

    public Skill(Character.Element element, int MPCost, int ID)
    {
        this.element = element;
        this.MPCost = MPCost;
        this.ID = ID;
    }

}
