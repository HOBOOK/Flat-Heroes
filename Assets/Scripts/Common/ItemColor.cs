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
        else if(itemClass==3)
        {
            return B;
        }
        else if(itemClass==2)
        {
            return C;
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
            UnityEngine.Color color = new Color(0.15f, 0.3f, 1f,1f);
            return color;
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
    public static Color B
    {
        get
        {
            return Color.cyan;
        }
    }
    public static Color C
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

public class HeroClassColor
{
    public static Color GetHeroColor(int over)
    {
        if (over == 3)
        {
            return ItemColor.Legend;
        }
        if (over == 2)
        {
            UnityEngine.Color color = new Color(0.75f, 0.3f, 0.5f, 1f);
            return color;
        }
        else if (over == 1)
        {
            UnityEngine.Color color = new Color(1f, 0.55f, 0.15f, 1f);
            return color;
        }
        else
        {
            return Color.cyan;
        }
    }
}
