using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ButtonETC : MonoBehaviour
{
    public void Button_Review()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.HobookGames.FlatHeros");
    }

    public void Button_Cafe()
    {
        Application.OpenURL("https://m.cafe.naver.com/hobookgamesfh.cafe");
    }

    public void Button_Quit()
    {
        Application.Quit();
    }
}
