using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EvolutionItem : MonoBehaviour
{
    public GameObject parentPanel;
    public GameObject matItemSlot1;
    public GameObject matItemSlot2;
    private Button matItemSlotButton1;
    private Button matItemSlotButton2;
    public Image targetItemSlot;
    private Image matItemSlotImage1;
    private Image matItemSlotImage2;
    public Image resultItemSlot;
    public Item targetItem;
    public Item resultItem;
    public Button evolutionButton;
    public Text evolutionInformationText;
    public bool isEndEvolution = false;
    int paymentType;
    int paymentAmount;

    Dictionary<int, Item> matItems = new Dictionary<int, Item>();

    private void Awake()
    {
        matItemSlotButton1 = matItemSlot1.GetComponent<Button>();
        matItemSlotButton2 = matItemSlot2.GetComponent<Button>();
        matItemSlotImage1 = matItemSlot1.transform.GetChild(0).GetComponent<Image>();
        matItemSlotImage2 = matItemSlot2.transform.GetChild(0).GetComponent<Image>();
    }

    public void OpenUI(Item item)
    {
        isEndEvolution = false;
        resultItem = null;
        targetItem = item;
        matItems.Clear();
        matItems = new Dictionary<int, Item>();

        if (targetItem.itemClass < 5)
            paymentType = 0;
        else
            paymentType = 1;
        targetItemSlot.gameObject.SetActive(true);
        matItemSlotImage1.gameObject.SetActive(true);
        matItemSlotImage2.gameObject.SetActive(true);
        resultItemSlot.gameObject.SetActive(true);
        RefreshUI();
    }

    public void AddMatItem(Item item)
    {
        if(isEndEvolution)
        {
            OpenUI(item);
        }
        else
        {
            if (matItems.Count < 2 && !matItems.ContainsKey(item.customId) && item.customId != targetItem.customId)
            {
                Debugging.Log(item.customId + " 재료에 추가");
                matItems.Add(item.customId, item);
            }
            RefreshUI();
        }
    }
    public void RemoveMatItem(Item item)
    {
        if(matItems.Count>0&&matItems.ContainsKey(item.customId))
        {
            Debugging.Log(item.customId + " 재료에서 제거");
            matItems.Remove(item.customId);
        }
        RefreshUI();
    }
    void RefreshUI()
    {
        if (targetItem == null)
            targetItemSlot.sprite = ItemSystem.GetItemNoneImage();
        else
            targetItemSlot.sprite = ItemSystem.GetItemImage(targetItem.id);
        if (resultItem == null)
            resultItemSlot.sprite = ItemSystem.GetItemNoneImage();
        else
            resultItemSlot.sprite = ItemSystem.GetItemImage(resultItem.id);
        matItemSlotButton1.onClick.RemoveAllListeners();
        matItemSlotButton2.onClick.RemoveAllListeners();
        matItemSlotImage1.sprite = ItemSystem.GetItemNoneImage();
        matItemSlotImage2.sprite = ItemSystem.GetItemNoneImage();
        foreach (var i in matItems.Values)
        {
            if(matItemSlotImage1.sprite.name==ItemSystem.GetItemNoneImage().name)
            {
                matItemSlotImage1.sprite = ItemSystem.GetItemImage(i.id);
                matItemSlotButton1.onClick.AddListener(delegate
                {
                    RemoveMatItem(i);
                });
            }
            else if(matItemSlotImage2.sprite.name==ItemSystem.GetItemNoneImage().name)
            {
                matItemSlotImage2.sprite = ItemSystem.GetItemImage(i.id);
                matItemSlotButton2.onClick.AddListener(delegate
                {
                    RemoveMatItem(i);
                });
            }
        }
        int itemClass = targetItem.itemClass;
        if (matItems.Count==2&&itemClass<8)
        {
            evolutionButton.gameObject.SetActive(true);
            if(itemClass < 5)
                paymentAmount = itemClass * itemClass* (300+(itemClass*itemClass*100));
            else
                paymentAmount = itemClass * itemClass * 2;

            evolutionButton.GetComponentInChildren<Text>().text = Common.GetThousandCommaText(paymentAmount);
            evolutionButton.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(Common.GetCoinCrystalEnergyImagePath(paymentType));

            evolutionInformationText.gameObject.SetActive(false);
        }
        else
        {
            evolutionButton.gameObject.SetActive(false);
            evolutionInformationText.gameObject.SetActive(true);
        }
        if (parentPanel.GetComponent<UI_Manager_InventoryTab>() != null)
        {
            parentPanel.GetComponent<UI_Manager_InventoryTab>().RefreshEvolutionUI();
        }
    }

    public bool IsSelectedItem(Item item)
    {
        if(matItems.ContainsKey(item.customId)||(targetItem!=null&&targetItem.customId==item.customId))
        {
            return true;
        }
        return false;
    }

    private void OnDisable()
    {
        if(parentPanel.GetComponent<UI_Manager_InventoryTab>()!=null)
        {
            parentPanel.GetComponent<UI_Manager_InventoryTab>().RefreshUI();
        }
    }

    public void OnEvolutionButtonClick()
    {
        if(matItems.Count==2&&targetItem!=null&&ItemSystem.GetNextClassItemId(targetItem)!=-1)
        {
            Debugging.Log("진화버튼입니다.");
            if (evolutionButton.IsInteractable())
            {
                StartCoroutine("CheckingAlert");
                evolutionButton.interactable = false;
            }
        }
        else
        {

        }
    }

    IEnumerator CheckingAlert()
    {
        var alertPanel = UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(paymentType), string.Format("<color='yellow'>'{0}' <size='24'>x </size>{1}</color>  {2}", Common.GetCoinCrystalEnergyText(paymentType), paymentAmount, LocalizationManager.GetText("alertNeedMessage5")));
        while (!alertPanel.GetComponentInChildren<UI_CheckButton>().isChecking)
        {
            yield return new WaitForFixedUpdate();
        }
        if (alertPanel.GetComponentInChildren<UI_CheckButton>().isResult)
        {
            UI_Manager.instance.ClosePopupAlertUI();
            if(paymentType==0)
            {
                if(Common.PaymentCheck(ref User.coin, paymentAmount))
                {
                    StartCoroutine("StartEvolution");
                }
                else
                {
                    UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(paymentType), LocalizationManager.GetText("manageTab2EvolutionPaymentWarning1"));
                }
            }
            else
            {
                if(Common.PaymentCheck(ref User.blackCrystal, paymentAmount))
                {
                    StartCoroutine("StartEvolution");
                }
                else
                {
                    UI_Manager.instance.ShowNeedAlert(Common.GetCoinCrystalEnergyImagePath(paymentType), LocalizationManager.GetText("manageTab2EvolutionPaymentWarning2"));
                }
            }
        }
        else
        {
            UI_Manager.instance.ClosePopupAlertUI();
            evolutionButton.interactable = true;
            // 아니오를 클릭시
        }
        yield return null;
    }


    IEnumerator StartEvolution()
    {
        yield return new WaitForEndOfFrame();
        //ItemSystem.UseEquipmentItem(targetItem.customId, 1);
        targetItemSlot.GetComponent<AiryUIAnimatedElement>().HideElement();
        yield return new WaitForSeconds(0.1f);
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.equip);
        ItemSystem.UseItem(targetItem.customId, 1);
        matItemSlotImage1.GetComponent<AiryUIAnimatedElement>().HideElement();
        matItemSlotImage2.GetComponent<AiryUIAnimatedElement>().HideElement();
        foreach (var i in matItems.Values)
        {
            ItemSystem.UseItem(i.customId, 1);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        EffectManager.SkillUpgradeEffect(resultItemSlot.transform);
        yield return new WaitForSeconds(0.3f);
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.dropItem);
        ItemSystem.SetObtainItem(ItemSystem.GetNextClassItemId(targetItem));
        resultItem = ItemSystem.GetItem(ItemSystem.GetNextClassItemId(targetItem));
        resultItemSlot.GetComponent<AiryUIAnimatedElement>().ShowElement();
        Debugging.Log("합성성공!");
        targetItem = null;
        matItems.Clear();
        evolutionButton.interactable = true;
        isEndEvolution = true;
        RefreshUI();
        yield return null;
    }
}
