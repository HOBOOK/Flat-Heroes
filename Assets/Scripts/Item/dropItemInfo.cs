using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dropItemInfo : MonoBehaviour
{
    public int dropItemID;
    GameObject dropItemUI;
    GameObject dropItem;
    ParticleSystem dropEffect;
    private void Awake()
    {
        if (dropItem == null)
            dropItem = transform.GetChild(0).gameObject;
        if (dropEffect == null)
            dropEffect = this.GetComponentInChildren<ParticleSystem>();
    }
    private void OnEnable()
    {
        dropEffect.gameObject.SetActive(false);
    }
    public void DropItem()
    {
        if (dropItem != null)
        {
            dropItemUI = Instantiate(ObjectPool.Instance.PopFromPool("dropItemUI"), GameObject.Find("CanvasUI").transform);
            dropItemUI.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            dropItemUI.SetActive(true);
            dropItem.GetComponent<SpriteRenderer>().sprite = ItemSystem.GetItemImage(dropItemID);
            dropItemUI.GetComponentInChildren<Text>().text = ItemSystem.GetItem(dropItemID).name;
            Invoke("ShowDropEffect", 2);
        }
    }
    public void ShowDropEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.dropItem);
        dropEffect.gameObject.SetActive(true);
        dropEffect.Play();
    }
    private void Update()
    {
        if (dropItemUI != null)
            dropItemUI.transform.position = this.transform.position;
    }

}
