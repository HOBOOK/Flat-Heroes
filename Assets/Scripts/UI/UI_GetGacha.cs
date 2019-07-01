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

    Image itemImage;
    Text itemClassText;
    Text itemNameText;
    ParticleSystem itemEffect;


    public void GachaStart(GachaSystem.GachaType gachaType)
    {
        GetComponent<Button>().interactable = false;
        tabText.enabled = false;
        foreach (Transform child in ItemView.transform)
        {
            Destroy(child.gameObject);
        }

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
        itemImage = slotItem.transform.GetChild(0).GetComponent<Image>();
        itemClassText = slotItem.transform.GetChild(1).GetComponentInChildren<Text>();
        itemNameText = slotItem.transform.GetChild(2).GetComponent<Text>();
        itemEffect = slotItem.transform.GetComponentInChildren<ParticleSystem>();

        // 아이템 등급별 색상
        int itemClass = item.itemClass;

        if (itemClass >= 5)
            itemEffect.Play();
        else
            itemEffect.Stop();

        itemClassText.color = ItemColor.GetItemColor(itemClass);
        itemNameText.color = ItemColor.GetItemColor(itemClass);
        //

        itemImage.sprite = ItemSystem.GetItemImage(item.id);
        itemClassText.text = Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)item.itemClass - 1);
        itemNameText.text = ItemSystem.GetItemName(item.id);
        if (slotItem.GetComponent<AiryUIAnimatedElement>() != null)
            slotItem.GetComponent<AiryUIAnimatedElement>().ShowElement();
        else
            slotItem.gameObject.SetActive(true);
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
        yield return new WaitForSeconds(0.1f);
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
            itemImage = slotItem.transform.GetChild(0).GetComponent<Image>();
            itemClassText = slotItem.transform.GetChild(1).GetComponentInChildren<Text>();
            itemNameText = slotItem.transform.GetChild(2).GetComponent<Text>();
            itemEffect = slotItem.transform.GetComponentInChildren<ParticleSystem>();

            // 아이템 등급별 색상
            int itemClass = items[i].itemClass;

            if (itemClass >= 5)
                itemEffect.Play();
            else
                itemEffect.Stop();

            itemClassText.color = ItemColor.GetItemColor(itemClass);
            itemNameText.color = ItemColor.GetItemColor(itemClass);
            //

            itemImage.sprite = ItemSystem.GetItemImage(items[i].id);
            itemClassText.text = Enum.GetName(typeof(GachaSystem.GachaClass), (GachaSystem.GachaClass)items[i].itemClass - 1);
            itemNameText.text = ItemSystem.GetItemName(items[i].id);
            if (slotItem.GetComponent<AiryUIAnimatedElement>() != null)
                slotItem.GetComponent<AiryUIAnimatedElement>().ShowElement();
            else
                slotItem.gameObject.SetActive(true);
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.ui_pop);
            yield return new WaitForSeconds(0.1f);
        }
        GetComponent<Button>().interactable = true;
        tabText.enabled = true;
        yield return null;
    }
}
