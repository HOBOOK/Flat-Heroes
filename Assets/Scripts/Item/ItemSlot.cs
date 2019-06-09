using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Text nameText;

    private Item _item;
    public Item Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
            {
                image.enabled = false;
                nameText.enabled = false;
            }
            else
            {
                image.sprite = ItemSystem.GetItemImage(_item.id);
                nameText.text = _item.name;
                image.enabled = true;
                nameText.enabled = true;
            }
        }
    }
    private void OnValidate()
    {
        if(image==null)
            image = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (nameText == null)
            nameText = transform.GetComponentInChildren<Text>();
    }
}
