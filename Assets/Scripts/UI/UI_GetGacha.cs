using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GetGacha : MonoBehaviour
{
    public GameObject slotGachaPrefab;
    public Transform ItemView;
    public Text tabText;
    public Image TitleImage;

    Image itemContainerImage;
    Image itemImage;
    Image itemCoverImage;
    Text itemClassText;
    Text itemNameText;
    ParticleSystem itemEffect;
    GameObject BoxCover;

    GachaSystem.GachaType gachaType;

    public void GachaStart(GachaSystem.GachaType type)
    {
        GetComponent<Button>().interactable = false;
        tabText.enabled = false;
        foreach (Transform child in ItemView.transform)
        {
            Destroy(child.gameObject);
        }
        gachaType = type;

        if (gachaType == GachaSystem.GachaType.NormalFive || gachaType == GachaSystem.GachaType.NormalOne)
            TitleImage.color = new Color32(55, 55, 6, 255);
        else
            TitleImage.color = new Color32(52, 14, 77, 255);

        switch(gachaType)
        {
            case GachaSystem.GachaType.SpecialFive:
                List<Item> gachaItems = GachaSystem.StartSpeicalGachaMultiple(ItemSystem.GetEquipmentItems(),5);
                StartCoroutine(ShowGetGachas(gachaItems));
                break;
            case GachaSystem.GachaType.SpecialOne:
                Item gachaItem = GachaSystem.StartSpeicalGacha(ItemSystem.GetEquipmentItems(), User.gachaSeed);
                StartCoroutine(ShowGetGacha(gachaItem));
                break;
            case GachaSystem.GachaType.NormalFive:
                List<Item> gachaItemsNormal = GachaSystem.StartNormalGachaMultiple(ItemSystem.GetEquipmentItems(), 5);
                StartCoroutine(ShowGetGachas(gachaItemsNormal));
                break;
            case GachaSystem.GachaType.NormalOne:
                Item gachaItemNormal = GachaSystem.StartNormalGacha(ItemSystem.GetEquipmentItems(),User.gachaSeed);
                StartCoroutine(ShowGetGacha(gachaItemNormal));
                break;
            case GachaSystem.GachaType.FreeAd:
                Item gachaItemFree = GachaSystem.StartSpeicalGacha(ItemSystem.GetEquipmentItems(), User.gachaSeed);
                StartCoroutine(ShowGetGacha(gachaItemFree));
                break;
        }
    }
    public IEnumerator ShowGetGacha(Item item)
    {
        yield return new WaitForSeconds(1.0f);
        GameObject slotItem = Instantiate(slotGachaPrefab, ItemView);
        itemCoverImage = slotItem.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        itemContainerImage = itemCoverImage.transform.GetChild(1).GetComponent<Image>();
        itemImage = itemCoverImage.transform.GetChild(0).GetComponent<Image>();

        itemClassText = slotItem.transform.GetChild(1).GetComponentInChildren<Text>();
        itemNameText = slotItem.transform.GetChild(2).GetComponent<Text>();
        itemEffect = slotItem.transform.GetComponentInChildren<ParticleSystem>();

        BoxCover = slotItem.transform.GetChild(4).gameObject;
        if(gachaType==GachaSystem.GachaType.NormalFive||gachaType==GachaSystem.GachaType.NormalOne)
        {
            slotItem.GetComponent<Image>().color = new Color32(55, 55, 6,255);
            BoxCover.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("UI/ui_normalBox");
        }
        else
        {
            slotItem.GetComponent<Image>().color = new Color32(52, 14, 77, 255);
            BoxCover.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("UI/ui_specialBox");
        }
        BoxCover.gameObject.SetActive(true);
        // 아이템 등급별 색상
        int itemClass = item.itemClass;

        if (itemClass >= 5)
            itemEffect.Play();
        else
            itemEffect.Stop();

        itemClassText.color = ItemColor.GetItemColor(itemClass);
        itemNameText.color = ItemColor.GetItemColor(itemClass);
        itemContainerImage.color = ItemColor.GetItemColor(itemClass);
        //
        itemCoverImage.sprite = ItemSystem.GetItemClassImage(item.id);
        itemImage.sprite = ItemSystem.GetItemImage(item.id);
        itemClassText.text = Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)item.itemClass - 1);
        itemNameText.text = ItemSystem.GetItemName(item.id);
        if (slotItem.GetComponent<AiryUIAnimatedElement>() != null)
            slotItem.GetComponent<AiryUIAnimatedElement>().ShowElement();
        else
            slotItem.gameObject.SetActive(true);
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        yield return new WaitForSeconds(0.1f);
        float alpha = 1;
        while(BoxCover.GetComponent<Image>().color.a>0)
        {
            BoxCover.GetComponent<Image>().color = new Color(1,1,1, alpha);
            alpha -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        BoxCover.gameObject.SetActive(false);
        GetComponent<Button>().interactable = true;
        tabText.enabled = true;
        yield return null;
    }

    public IEnumerator ShowGetGachas(List<Item> items)
    {
        yield return new WaitForSeconds(1.0f);
        for (var i = 0; i < items.Count; i++)
        {
            GameObject slotItem = Instantiate(slotGachaPrefab, ItemView);
            itemCoverImage = slotItem.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            itemContainerImage = itemCoverImage.transform.GetChild(1).GetComponent<Image>();
            itemImage = itemCoverImage.transform.GetChild(0).GetComponent<Image>();
            itemClassText = slotItem.transform.GetChild(1).GetComponentInChildren<Text>();
            itemNameText = slotItem.transform.GetChild(2).GetComponent<Text>();
            itemEffect = slotItem.transform.GetComponentInChildren<ParticleSystem>();
            BoxCover = slotItem.transform.GetChild(4).gameObject;
            if (gachaType == GachaSystem.GachaType.NormalFive || gachaType == GachaSystem.GachaType.NormalOne)
            {
                slotItem.GetComponent<Image>().color = new Color32(55, 55, 6, 255);
                BoxCover.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("UI/ui_normalBox");
            }
            else
            {
                slotItem.GetComponent<Image>().color = new Color32(52, 14, 77, 255);
                BoxCover.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("UI/ui_specialBox");
            }
            BoxCover.gameObject.SetActive(true);

            // 아이템 등급별 색상
            int itemClass = items[i].itemClass;

            if (itemClass >= 5)
                itemEffect.Play();
            else
                itemEffect.Stop();

            itemClassText.color = ItemColor.GetItemColor(itemClass);
            itemNameText.color = ItemColor.GetItemColor(itemClass);
            itemContainerImage.color = ItemColor.GetItemColor(itemClass);
            //
            itemCoverImage.sprite = ItemSystem.GetItemClassImage(items[i].id);
            itemImage.sprite = ItemSystem.GetItemImage(items[i].id);
            itemClassText.text = Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)items[i].itemClass - 1);
            itemNameText.text = ItemSystem.GetItemName(items[i].id);
            if (slotItem.GetComponent<AiryUIAnimatedElement>() != null)
                slotItem.GetComponent<AiryUIAnimatedElement>().ShowElement();
            else
                slotItem.gameObject.SetActive(true);
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
            yield return new WaitForSeconds(0.1f);
            float alpha = 1;
            while (BoxCover.GetComponent<Image>().color.a > 0)
            {
                BoxCover.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
                alpha -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            BoxCover.gameObject.SetActive(false);
        }

        GetComponent<Button>().interactable = true;
        tabText.enabled = true;
        yield return null;
    }
}
