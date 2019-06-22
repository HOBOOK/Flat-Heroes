using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager_AbilityTab : MonoBehaviour
{
    public GameObject abilitySlotPrefab;
    public GameObject SelectImage;
    
    protected GameObject[] PanelAbilitys;
    protected GameObject PanelAbilityInfo;
    protected GameObject PanelAbilitySelection;
    private List<GameObject> abilitySlotList = new List<GameObject>();
    private List<Ability> abilityList = new List<Ability>();
    Image abilityInfoImage;
    Text abilityInfoNameText;
    Text abilityInfoDescriptionText;
    Text abilityTotalStatText;
    Text scrollCountText;

    bool isSelectStart = false;
    private void Awake()
    {
        if(PanelAbilitys==null)
        {
            PanelAbilitys = new GameObject[3];
            for(int i = 0; i < 3; i++)
            {
                PanelAbilitys[i] = this.transform.GetChild(0).transform.GetChild(i).gameObject;
            }
        }
        if (PanelAbilityInfo == null)
            PanelAbilityInfo = this.transform.GetChild(1).gameObject;
        if (PanelAbilitySelection == null)
            PanelAbilitySelection = this.transform.GetChild(2).gameObject;
        if (scrollCountText == null)
            scrollCountText = PanelAbilitySelection.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        if (abilityInfoImage == null)
            abilityInfoImage = PanelAbilityInfo.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        
        if (abilityInfoNameText==null||abilityInfoDescriptionText==null||abilityTotalStatText==null)
        {
            foreach(var txt in PanelAbilityInfo.GetComponentsInChildren<Text>())
            {
                if (txt.name.Equals("name"))
                    abilityInfoNameText = txt;
                else if (txt.name.Equals("description"))
                    abilityInfoDescriptionText = txt;
                else if (txt.name.Equals("totalStat"))
                    abilityTotalStatText = txt;
            }
        }
    }
    private void OnEnable()
    {
        if (UI_Manager.instance.PopupInterActiveCover.activeSelf)
            UI_Manager.instance.PopupInterActiveCover.SetActive(false);
        SelectImage.GetComponent<Image>().enabled = false;
        if(abilityInfoImage!=null&&abilityInfoNameText!=null&&abilityInfoDescriptionText!=null&&abilityTotalStatText!=null)
        {
            abilityInfoImage.sprite = Resources.Load<Sprite>("UI/ui_ability_lock");
            abilityInfoNameText.enabled = false;
            abilityInfoDescriptionText.enabled = false;
            abilityTotalStatText.enabled = true;
        }
        RefreshUI();
    }

    private void Start()
    {
        if(abilitySlotPrefab!=null)
        {
            foreach(var ab in AbilitySystem.GetAllAbilities())
            {
                GameObject abObj = Instantiate(abilitySlotPrefab, PanelAbilitys[ab.id%3].transform);
                if (AbilitySystem.isAbleAbility(ab.id))
                {
                    Ability userAbility = AbilitySystem.GetUserAbility(ab.id);
                    abObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(userAbility.image);
                    abObj.transform.GetComponentInChildren<Text>().text = userAbility.level.ToString();
                    abObj.transform.GetComponentInChildren<Text>().enabled = true;
                    abObj.GetComponent<Button>().onClick.RemoveAllListeners();
                    abObj.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnAbilityClick(userAbility);
                    });
                    abilityList.Add(userAbility);
                }
                else
                {
                    abObj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ui_ability_lock");
                    abObj.transform.GetComponentInChildren<Text>().enabled = false;
                    abilityList.Add(ab);
                }
                abilitySlotList.Add(abObj);
            }

            if (abilityTotalStatText != null)
            {
                AbilitySystem.SetAbilityStats();
                abilityTotalStatText.text = AbilitySystem.GetAllAbilityStatToString();
            }

            if (scrollCountText != null)
            {
                scrollCountText.text = Common.GetThousandCommaText(ItemSystem.GetUserScrollCount());
            }
        }
    }
    private void RefreshUI()
    {
        abilityList.Clear();
        foreach (var ab in AbilitySystem.GetAllAbilities())
        {
            abilityList.Add(ab);
        }
        if (abilitySlotList!=null)
        {
            for(var i = 0; i < abilitySlotList.Count; i++)
            {
                if(AbilitySystem.isAbleAbility(abilityList[i].id))
                {
                    Ability userAb = AbilitySystem.GetUserAbility(abilityList[i].id);
                    abilitySlotList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(userAb.image);
                    abilitySlotList[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    int abIndex = i;
                    userAb = AbilitySystem.GetUserAbility(abilityList[abIndex].id);
                    abilitySlotList[i].transform.GetComponentInChildren<Text>().text = userAb.level.ToString();
                    abilitySlotList[i].transform.GetComponentInChildren<Text>().enabled = true;
                    abilitySlotList[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    abilitySlotList[i].GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnAbilityClick(abilityList[abIndex]);
                    });
                }
            }
        }

        if (abilityTotalStatText != null)
        {
            AbilitySystem.SetAbilityStats();
            abilityTotalStatText.text = AbilitySystem.GetAllAbilityStatToString();
        }
        
        if(scrollCountText!=null)
        {
            scrollCountText.text = Common.GetThousandCommaText(ItemSystem.GetUserScrollCount());
        }
    }

    private void OnAbilityClick(Ability ab)
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if (ab != null)
        {
            abilityInfoImage.sprite = Resources.Load<Sprite>(ab.image);
            abilityInfoNameText.text = ab.name;
            abilityInfoDescriptionText.text = AbilitySystem.GetAbilityDescription(ab.powerType, (ab.power*ab.level));


            abilityInfoNameText.enabled = true;
            abilityInfoDescriptionText.enabled = true;

            if (abilityInfoImage.GetComponentInParent<AiryUIAnimatedElement>() != null)
                abilityInfoImage.GetComponentInParent<AiryUIAnimatedElement>().ShowElement();


            if (abilityTotalStatText != null)
            {
                AbilitySystem.SetAbilityStats();
                abilityTotalStatText.text = AbilitySystem.GetAllAbilityStatToString();
            }
        }
    }

    IEnumerator CheckingAlert()
    {
        isCheckAlertOn = true;
        int amount = User.abilityCount;
        Item userAbilityScroll = ItemSystem.GetItem(8001);
        Debugging.Log(amount);
        var alertPanel = UI_Manager.instance.ShowNeedAlert(userAbilityScroll.image, string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  사용하여 능력을 개방하시겠습니까?", userAbilityScroll.name, amount));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            if (ItemSystem.GetUserScrollCount() >= amount)
            {
                if (!isSelectStart)
                    StartCoroutine(RandomSelectAbility(amount));
            }
            else
            {
                UI_Manager.instance.ShowAlert(UI_Manager.PopupAlertTYPE.scroll, amount);
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
        }
        isCheckAlertOn = false;
        yield return null;
    }
    bool isCheckAlertOn = false;
    public void StartSelectAbility()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
        if(!isCheckAlertOn)
            StartCoroutine("CheckingAlert");
    }

    IEnumerator RandomSelectAbility(int useItemAmount)
    {
        isSelectStart = true;
        int index = 0;
        int tempIndex = 0;
        float time = 0;
        UI_Manager.instance.PopupInterActiveCover.SetActive(true);
        int selectedIndex = Random.Range(0, abilitySlotList.Count);
        SelectImage.GetComponent<Image>().enabled = true;
        while (time<1)
        {
            tempIndex = Random.Range(0, abilitySlotList.Count);
            if (tempIndex == index)
                index = Mathf.Clamp(tempIndex + 1, 0, abilitySlotList.Count - 1);
            else
                index = tempIndex;
            SelectImage.transform.SetParent(abilitySlotList[index].transform);
            SelectImage.transform.localPosition = Vector3.zero;
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            yield return new WaitForSeconds(Mathf.Clamp(1.0f-(time*2),0.1f,1));
            time += 0.05f;
        }
        while(index==selectedIndex)
        {
            index += 1;
            if (index > abilitySlotList.Count - 1)
                index = 0;
            SelectImage.transform.SetParent(abilitySlotList[index].transform);
            SelectImage.transform.localPosition = Vector3.zero;
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_button_default);
            yield return new WaitForSeconds(0.1f);
        }
        SelectImage.transform.SetParent(abilitySlotList[selectedIndex].transform);
        SelectImage.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(0.3f);
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        UI_Manager.instance.PopupInterActiveCover.SetActive(false);
        SelectImage.GetComponent<Image>().enabled = false;
        isSelectStart = false;

        if (ItemSystem.UseItem(ItemSystem.GetUserScroll().customId, useItemAmount))
        {
            AbilitySystem.SetObtainAbility(abilityList[selectedIndex].id);
            RefreshUI();
            UI_Manager.instance.PopupGetAbility(abilityList[selectedIndex]);
        }
        else
        {
            RefreshUI();
        }
        yield return null;
    }

}
