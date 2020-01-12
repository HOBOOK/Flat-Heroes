using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo : MonoBehaviour
{
    protected Text coinText;
    protected Text crystalText;
    protected Text energyText;
    protected Text magicStoneText;
    protected Text transcendenceText;
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
            else if (i.name.Equals("magicStoneText"))
                magicStoneText = i;
            else if (i.name.Equals("transcendenceText"))
                transcendenceText = i;

        }
    }
    private void Start()
    {
        for(var i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Button>()!=null)
            {
                var btn = transform.GetChild(i).GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                var index = i;
                btn.onClick.AddListener(delegate
                {
                    UI_Manager.instance.ShowShopUI(index);
                });
            }
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
        if (magicStoneText != null)
            magicStoneText.text = Common.GetThousandCommaText(User.magicStone);
        if (transcendenceText != null)
            transcendenceText.text = Common.GetThousandCommaText(User.transcendenceStone);
    }
    string GetEnergyText()
    {
        if (User.portalEnergy < 40)
            return GameManagement.instance.GetPortalEnergyTime() + string.Format(" {0}/40", User.portalEnergy);
        else
            return Common.GetThousandCommaText(User.portalEnergy);
    }
}
