using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ButtonRemoveAD : MonoBehaviour
{
    private void Start()
    {
        if(User.isAdsRemove&&User.isAdsSkip)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }
    public void ShowShopUI(int type)
    {
        UI_Manager.instance.ShowShopUI(4);
    }
}
