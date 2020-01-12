using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbility
{
    public int id;
    public string name;
    public string description;
    public int lv;
    HeroAbility() { }
    public HeroAbility(int pId, string pName, string pDec, int pLv)
    {
        id = pId;
        name = pName;
        description = pDec;
        lv = pLv;
    }
}
