using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaSystem
{
    public enum GachaClass { D,C,B,A,S,SS,SSS};
    public enum GachaType { SpecialFive, SpecialOne, NormalFive, NormalOne, FreeAd }

    public static List<Item> StartSpeicalGachaMultiple(List<Item> items, int count)
    {
        List<Item> returnGachaItemList = new List<Item>();
        for(int i = 0; i<count; i++)
        {
            Item item = StartSpeicalGacha(items,User.gachaSeed+i);
            if(item!=null)
            {
                returnGachaItemList.Add(item);
            }
        }
        return returnGachaItemList;
    }

    [SerializeField] static Random.State lastState;
    public static Item StartSpeicalGacha(List<Item> items, int seed)
    {
        Random.InitState(seed);
        int gachaClassNumber = Random.Range(0, 1000);
        User.gachaSeed = Random.Range(0, 1000);
        GachaClass gachaClass;
        if (gachaClassNumber <= 5)
            gachaClass = GachaClass.SSS;
        else if (gachaClassNumber > 5 && gachaClassNumber <= 25)
            gachaClass = GachaClass.SS;
        else if (gachaClassNumber > 25 && gachaClassNumber <= 170)
            gachaClass = GachaClass.S;
        else if (gachaClassNumber > 150 && gachaClassNumber <= 350)
            gachaClass = GachaClass.A;
        else
            gachaClass = GachaClass.B;
        List<Item> gachaItemList = new List<Item>();
        Item returnGachaItem = null;
        switch(gachaClass)
        {
            case GachaClass.SSS:
                gachaItemList = items.FindAll(x => x.itemClass == 7);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.SS:
                gachaItemList = items.FindAll(x => x.itemClass == 6);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.S:
                gachaItemList = items.FindAll(x => x.itemClass == 5);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.A:
                gachaItemList = items.FindAll(x => x.itemClass == 4);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.B:
                gachaItemList = items.FindAll(x => x.itemClass == 3);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
        }
        if(returnGachaItem!=null)
            ItemSystem.SetObtainItem(returnGachaItem.id);

        Debugging.Log(returnGachaItem.name + " 아이템이 랜덤으로 뽑힘");
        return returnGachaItem;
    }

    public static List<Item> StartNormalGachaMultiple(List<Item> items, int count)
    {
        List<Item> returnGachaItemList = new List<Item>();
        for (int i = 0; i < count; i++)
        {
            Item item = StartNormalGacha(items, User.gachaSeed+i);
            if (item != null)
            {
                returnGachaItemList.Add(item);
            }
        }
        return returnGachaItemList;
    }

    public static Item StartNormalGacha(List<Item> items, int seed)
    {
        Random.InitState(seed);
        int gachaClassNumber = Random.Range(0, 1000);
        User.gachaSeed = Random.Range(0, 1000);
        GachaClass gachaClass;
        if (gachaClassNumber > 0 && gachaClassNumber <= 50)
            gachaClass = GachaClass.S;
        else if (gachaClassNumber > 50 && gachaClassNumber <= 250)
            gachaClass = GachaClass.A;
        else if (gachaClassNumber > 250 && gachaClassNumber <= 450)
            gachaClass = GachaClass.B;
        else if (gachaClassNumber > 450 && gachaClassNumber <= 750)
            gachaClass = GachaClass.C;
        else
            gachaClass = GachaClass.D;

        List<Item> gachaItemList = new List<Item>();
        Item returnGachaItem = null;
        switch (gachaClass)
        {

            case GachaClass.S:
                gachaItemList = items.FindAll(x => x.itemClass == 5);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.A:
                gachaItemList = items.FindAll(x => x.itemClass == 4);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.B:
                gachaItemList = items.FindAll(x => x.itemClass == 3);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.C:
                gachaItemList = items.FindAll(x => x.itemClass == 2);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
            case GachaClass.D:
                gachaItemList = items.FindAll(x => x.itemClass == 1);
                returnGachaItem = gachaItemList[Random.Range(0, gachaItemList.Count)];
                break;
        }
        if (returnGachaItem != null)
            ItemSystem.SetObtainItem(returnGachaItem.id);
        Debugging.Log(returnGachaItem.name + " 아이템이 랜덤으로 뽑힘");
        return returnGachaItem;
    }
}
