using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroAbilityPanel : MonoBehaviour
{
    public Transform abilityListTransform;
    public GameObject abilityInfoPanel;
    public Button abilityGetButton;
    public UI_HeroInfo ui_heroInfo;
    Dictionary<int, HeroAbility> abilities;
    HeroData targetHeroData;

    bool isCheckAlert = false;
    private void OnEnable()
    {
        foreach (Transform i in abilityListTransform.transform)
        {
            i.GetChild(1).GetComponent<Image>().color = new Color32(0, 0, 0, 192);
        }
    }

    public void OpenUI(HeroData data)
    {
        targetHeroData = data;
        RefreshUI();
    }

    void RefreshUI()
    {
        abilityInfoPanel.SetActive(false);
        abilities = new Dictionary<int, HeroAbility>();
        for (var i = 0; i < abilityListTransform.childCount; i++)
        {
            UnityEngine.Random.InitState(DateTime.Now.Day+i+targetHeroData.id);
            int abilityId = 0;
            if (!HeroSystem.IsSupportHero(targetHeroData.id))
                abilityId = HeroAbilitySystem.heroAbilityList[UnityEngine.Random.Range(0, HeroAbilitySystem.heroAbilityList.Count)].id;
            else
            {
                abilityId = HeroAbilitySystem.supportHeroAbilityList[UnityEngine.Random.Range(0, HeroAbilitySystem.supportHeroAbilityList.Count)].id;
            }
            int lv = UnityEngine.Random.Range(1, UnityEngine.Random.Range(5,9));
            HeroAbility heroAbility = new HeroAbility(abilityId, string.Format("{0} {1}", HeroAbilitySystem.GetHeroAbilityName(abilityId), getRomeNumber(lv)), HeroAbilitySystem.GetHeroAbilityDescription(abilityId), lv);
            abilityListTransform.transform.GetChild(i).GetComponentInChildren<Text>().text = heroAbility.name;
            int index = i;
            abilities.Add(index, heroAbility);
            abilityListTransform.transform.GetChild(index).GetComponent<Button>().onClick.RemoveAllListeners();
            abilityListTransform.transform.GetChild(index).GetComponent<Button>().onClick.AddListener(delegate
            {
                OnClickAbilityInfoButton(string.Format("{0}", heroAbility.description), abilityListTransform.transform.GetChild(index).GetComponent<Button>(),abilityId,lv);
            });
        }
        if (targetHeroData.ability == 0)
            abilityGetButton.GetComponentInChildren<Text>().text = "150";
        else
            abilityGetButton.GetComponentInChildren<Text>().text = "200";
    }

    IEnumerator GetAbility()
    {
        abilityInfoPanel.SetActive(false);
        foreach(Transform i in abilityListTransform.transform)
        {
            i.GetChild(1).GetComponent<Image>().color = new Color32(0,0,0,192);
        }
        int index = 0;
        int tempIndex = 0;
        float time = 0;
        UI_Manager.instance.PopupInterActiveCover.SetActive(true);
        int selectedIndex = UnityEngine.Random.Range(0, abilityListTransform.childCount);
        GameObject cacheObject=null;
        while (time < 1)
        {
            tempIndex = UnityEngine.Random.Range(0, abilityListTransform.childCount);
            if (tempIndex == index)
                index = Mathf.Clamp(tempIndex + 1, 0, abilityListTransform.childCount-1);
            else
                index = tempIndex;
            cacheObject = abilityListTransform.GetChild(index).gameObject;
            abilityListTransform.GetChild(index).GetChild(1).GetComponent<Image>().enabled = false;
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            yield return new WaitForSeconds(Mathf.Clamp(1.0f - (time * 2), 0.1f, 1));
            time += 0.1f;
            if (cacheObject != null)
                cacheObject.transform.GetChild(1).GetComponent<Image>().enabled = true;
        }
        float alpha = 0.0f;
        while(alpha<1.0f)
        {
            abilityListTransform.GetChild(selectedIndex).GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            alpha += 2*Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SoundManager.instance.EffectOnOff(AudioClipManager.instance.ui_pop);
        while (alpha > 0.0f)
        {
            abilityListTransform.GetChild(selectedIndex).GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            alpha -= 0.7f* Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        HeroSystem.SaveAbilityHero(targetHeroData, abilities[selectedIndex].id, abilities[selectedIndex].lv);
        UI_Manager.instance.PopupInterActiveCover.SetActive(false);
        ui_heroInfo.RefreshHeroStatusEquipmentPanel();
        yield return null;
    }

    public void OnClickAbilityInfoButton(string txt, Button button,int type, int level)
    {
        abilityInfoPanel.SetActive(true);
        abilityInfoPanel.GetComponent<UI_InformationPanel>().ShowInformation(button.transform.position-new Vector3(0,80,0), string.Format("{0} <color='red'>{1}</color>", txt, HeroAbilitySystem.GetAbilityPowerDescription(type, level)));
    }

    string getRomeNumber(int n)
    {
        if(n<10&&n>0)
        {
            string[] romeNumbers = { "I","II","III","IV","V","VI","VII","VIII","IX" };
            return romeNumbers[n - 1];
        }
        return "";
    }

    public void OnClickGetAbility()
    {
        if (!isCheckAlert)
        {
            isCheckAlert = true;
            StartCoroutine("CheckingAlert");
        }
    }
    IEnumerator CheckingAlert()
    {
        GameObject alertPanel;
        if(targetHeroData.ability==0)
            alertPanel = UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(1), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}\r\n", Common.GetCoinCrystalEnergyText(1), 150, LocalizationManager.GetText("alertHeroAbilityMessage1")));
        else
            alertPanel = UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(1), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}\r\n", Common.GetCoinCrystalEnergyText(1), 200, LocalizationManager.GetText("alertHeroAbilityMessage2")));

        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            if (Common.PaymentCheck(ref User.blackCrystal, targetHeroData.ability==0?150:200))
            {
                yield return StartCoroutine("GetAbility");
            }
            else
            {
                UI_Manager.instance.ShowAlert(Common.GetCoinCrystalEnergyImagePath(1), LocalizationManager.GetText("alertCrystal"));
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            // 아니오를 클릭시
        }

        isCheckAlert = false;
        yield return null;
    }
}
