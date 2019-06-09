using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ItemSystem
{
    private static List<Item> items = new List<Item>();
    public static void LoadItem()
    {
        items.Clear();
        string path = Application.persistentDataPath + "/Xml/Item.Xml";
        ItemDatabase id = null;

        if (System.IO.File.Exists(path))
            id = ItemDatabase.Load();
        else
            id = ItemDatabase.InitSetting();

        if(id!=null)
        {
            foreach (Item item in id.items)
            {
                items.Add(item);
            }
            Debugging.LogSystem("ItemDatabase is loaded Succesfully.");
        }
    }

    public static Item GetItem(int id)
    {
        return items.Find(item => item.id == id || item.id.Equals(id));
    }

    public static Item GetItem(string name)
    {
        return items.Find(item => item.name.Equals(name));
    }

    public static int GetItemCount()
    {
        return items.Count;
    }
    public static List<Item> GetAllItems()
    {
        return items;
    }

    public static List<Item> GetShopItems()
    {
        return items.FindAll(item => item.itemtype==100||item.itemtype.Equals(100));
    }

    public static Item GetRandomItem()
    {
        List<Item> ranitems = items.FindAll(item => item.itemtype != 100);
        return ranitems[Random.Range(0, ranitems.Count)];
    }

    public static List<Item> GetUserItems(Common.OrderByType orderByType = Common.OrderByType.NONE)
    {
        List<Item> itemList = new List<Item>();
        foreach(var item in items.FindAll(item => item.count > 0 && item.enable&&item.itemtype!=100))
        {
            if(item.itemtype==0)
            {
                for (int i = 0; i < item.count; i++)
                {
                    itemList.Add(item);
                }
            }
            else
            {
                itemList.Add(item);
            }
        }
        if(orderByType!=Common.OrderByType.NONE)
        {
            itemList = SetOrderByItemList(itemList, orderByType);
        }
        return itemList;
    }

    public static Item GetUserItem(int id)
    {
        return GetUserItems().Find(item => item.id == id || item.id.Equals(id));
    }

    private static List<Item> SetOrderByItemList(List<Item> itemList, Common.OrderByType orderByType)
    {
        if(itemList!=null)
        {
            switch (orderByType)
            {
                case Common.OrderByType.NAME:
                    itemList.Sort((i1, i2) => i1.name.CompareTo(i2.name));
                    break;
                case Common.OrderByType.VALUE:
                    itemList.Sort((i1, i2) => i1.value.CompareTo(i2.name));
                    break;
            }
            return itemList;
        }
        else
        {
            Debugging.Log("정렬할 아이템 리스트가 null 입니다.");
            return null;
        }
    }

    public static int GetWeaponType(int id)
    {
        return items.Find(item => item.id == id).weapontype;
    }

    public static Sprite GetItemImage(int id)
    {
        return Resources.Load<Sprite>(items.Find(item => item.id == id).image);
    }
    public static void SetObtainMoney(int id)
    {
        Item obtainMoney = items.Find(item => item.id == id || item.id.Equals(id));

        if (obtainMoney != null)
        {
            if(obtainMoney.id>9000&&obtainMoney.id<9010)
                SaveSystem.SetUserCrystal(obtainMoney.count);
            else if(obtainMoney.id>9010&&obtainMoney.id<9020)
                SaveSystem.SetUserEnergy(obtainMoney.count);
            SaveSystem.SavePlayer();
        }
        else
        {
            Debugging.LogError("획득할 재화를 찾지못함 >> " + id);
        }
    }

    public static void SetObtainItem(int id)
    {
        Item obtainItem = items.Find(item => item.id == id || item.id.Equals(id));

        if (obtainItem != null)
        {
            obtainItem.enable = true;
            obtainItem.count += 1;
            ItemDatabase.ItemSave(id);
        }
        else
        {
            Debugging.LogError("획득할 아이템을 찾지못함 >> " + id);
        }
    }

    public static void UseItem(int id, int count)
    {
        Item useItem = items.Find(item => item.id == id || item.id.Equals(id));
        if(useItem!=null)
        {
            useItem.count -= count;
            ItemDatabase.ItemSave(id);
            Debugging.Log(useItem.name + "을 " + count + "개 사용하여 " + useItem.count + "개 남았습니다.");
        }
    }

}
