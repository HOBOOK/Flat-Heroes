using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemColor
{
    public static Color GetItemColor(int itemClass)
    {
        if(itemClass==8)
        {
            return Legend;
        }
        if(itemClass==7)
        {
            return SSS;
        }
        else if(itemClass==6)
        {
            return SS;
        }
        else if(itemClass==5)
        {
            return S;
        }
        else if(itemClass==4)
        {
            return A;
        }
        else if(itemClass>1 && itemClass<4)
        {
            return BC;
        }
        else
        {
            return D;
        }
    }

    #region 아이템 클래스별 색상
    public static Color Legend
    {
        get
        {
            return Color.black;
        }
    }
    public static Color SSS
    {
        get
        {
            return Color.magenta;
        }
    }
    public static Color SS
    {
        get
        {
            return Color.red;
        }
    }
    public static Color S
    {
        get
        {
            return Color.yellow;
        }
    }
    public static Color A
    {
        get
        {
            return Color.green;
        }
    }
    public static Color BC
    {
        get
        {
            return Color.white;
        }
    }
    public static Color D
    {
        get
        {
            return Color.gray;
        }
    }
    #endregion region
}
