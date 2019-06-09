using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo : MonoBehaviour
{
    protected Text coinText;
    protected Text crystalText;
    protected Text energyText;
    private void Awake()
    {
        foreach(var i in GetComponentsInChildren<Text>())
        {
            if (i.name.Equals("coinText"))
                coinText = i;
            else if (i.name.Equals("crystalText"))
                crystalText = i;
            else if (i.name.Equals("energyText"))
                energyText = i;
        }
    }
    void FixedUpdate()
    {
        if(coinText!=null)
            coinText.text = Common.GetThousandCommaText(User.coin);
        if(crystalText!=null)
            crystalText.text = Common.GetThousandCommaText(User.blackCrystal);
        if (energyText != null)
            energyText.text = GetEnergyText();
    }
    string GetEnergyText()
    {
        if (User.portalEnergy < 30)
            return GameManagement.instance.GetPortalEnergyTime() + string.Format(" {0}/30", User.portalEnergy);
        else
            return Common.GetThousandCommaText(User.portalEnergy);
    }
}
