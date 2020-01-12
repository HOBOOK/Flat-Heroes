using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image coverImage;
    [SerializeField] Text nameText;
    [SerializeField] Text countText;

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
                countText.enabled = false;
                coverImage.sprite = ItemSystem.GetItemNoneImage();
            }
            else
            {
                image.sprite = ItemSystem.GetItemImage(_item.id);
                coverImage.sprite = ItemSystem.GetItemClassImage(_item.id);
                nameText.text = ItemSystem.GetItemName(_item.id);
                image.enabled = true;
                nameText.enabled = true;
                if(_item.itemtype==1)
                {
                    countText.enabled = true;
                    countText.text = _item.count.ToString();
                }
            }
        }
    }
    private void OnValidate()
    {
        if(image==null)
            image = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        if (coverImage == null)
            coverImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        if (nameText == null||countText==null)
        {
            foreach(var txt in transform.GetComponentsInChildren<Text>())
            {
                if (txt.name.Equals("ItemName"))
                    nameText = txt;
                else if (txt.name.Equals("ItemCount"))
                    countText = txt;
            }
        }
    }
}
