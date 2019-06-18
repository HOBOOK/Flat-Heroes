using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ItemSystem
{
    // 전체아이템데이터
    private static List<Item> items = new List<Item>();
    // 유저아이템데이터
    private static List<Item> userItems = new List<Item>();
    public static void LoadItem()
    {
        items.Clear();
        userItems.Clear();
        string path = Application.persistentDataPath + "/Xml/Item.Xml";
        ItemDatabase id = null;
        ItemDatabase userId = null;

        if (System.IO.File.Exists(path))
        {
            id = ItemDatabase.Load();
            userId = ItemDatabase.LoadUser();
        }
        else
        {
            id = ItemDatabase.InitSetting();
            userId = ItemDatabase.LoadUser();
        }
        if(id!=null)
        {
            foreach (Item item in id.items)
            {
                items.Add(item);
            }
        }
        if(userId!=null)
        {
            foreach (Item item in userId.items)
            {
                userItems.Add(item);
            }
        }
        if(items!=null&&userItems!=null)
        {
            Debugging.LogSystem("ItemDatabase is loaded Succesfully.");
        }
    }

    #region 유저아이템정보
    public static List<Item> GetUserItems(Common.OrderByType orderByType = Common.OrderByType.NONE)
    {
        List<Item> itemList = new List<Item>();
        foreach (var item in userItems.FindAll(item => item.count > 0 && item.itemtype != 100))
        {
            if (item.itemtype == 0)
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
        if (orderByType != Common.OrderByType.NONE)
        {
            itemList = SetOrderByItemList(itemList, orderByType);
        }
        return itemList;
    }
    public static Item GetUserItem(int id)
    {
        return userItems.Find(item => item.id == id || item.id.Equals(id));
    }
    public static int GetUserScrollCount()
    {
        Item scroll = GetUserItem(8001);
        if (scroll == null)
            return 0;
        else
            return scroll.count;
    }
    public static void SetObtainMoney(int id)
    {
        Item obtainMoney = items.Find(item => item.id == id || item.id.Equals(id));

        if (obtainMoney != null)
        {
            if (obtainMoney.id > 9000 && obtainMoney.id < 9010)
                SaveSystem.AddUserCrystal(obtainMoney.count);
            else if (obtainMoney.id > 9010 && obtainMoney.id < 9020)
                SaveSystem.AddUserEnergy(obtainMoney.count);
            SaveSystem.SavePlayer();
        }
        else
        {
            Debugging.LogError("획득할 재화를 찾지못함 >> " + id);
        }
    }
    public static void SetObtainItem(int id, int count=1)
    {
        Item obtainItem = userItems.Find(item => item.id == id || item.id.Equals(id));

        if (obtainItem != null)
        {
            obtainItem.enable = true;
            obtainItem.count += count;
            ItemDatabase.ItemSave(id);
        }
        else
        {
            Item newItem = items.Find(item => item.id == id || item.id.Equals(id));
            if(newItem!=null)
            {
                newItem.enable = true;
                newItem.count = count;
                userItems.Add(newItem);
                ItemDatabase.AddItemSave(id);
            }
        }
    }
    public static void UseItem(int id, int count)
    {
        Item useItem = userItems.Find(item => item.id == id || item.id.Equals(id)&&item.itemtype==1);
        if (useItem != null)
        {
            if(useItem.count-count<=0)
            {
                //xml삭제
                userItems.Remove(useItem);
                ItemDatabase.DeleteItemSave(id);
                Debugging.Log(useItem.name + "을 " + count + "개 사용하여 0개가 남아서 XML에서 삭제되었습니다.");
            }
            else
            {
                useItem.count -= count;
                ItemDatabase.ItemSave(id);
                Debugging.Log(useItem.name + "을 " + count + "개 사용하여 " + useItem.count + "개 남았습니다.");
            }

        }
    }
    public static void EquipItem(int dismountId, int equipId, ref HeroData heroData)
    {
        Item equipItem = userItems.Find(item => item.id == equipId || item.id.Equals(equipId) && item.itemtype == 0);
        Item dismountItem = userItems.Find(item => item.id == dismountId || item.id.Equals(dismountId) && item.itemtype == 0);
        if (equipItem != null && heroData != null)
        {
            equipItem.equipCharacterId = heroData.id;
            if(dismountItem!=null)
                dismountItem.equipCharacterId = 0;
            ItemDatabase.EquipItemSave(dismountId, equipId, heroData.id);
            Debugging.Log(equipItem.name + " 아이템이 " + heroData.name + "에게 장착되었습니다.");
        }
    }
    public static void DismountItem(int dismountId)
    {
        Item dismountItem = userItems.Find(item => item.id == dismountId || item.id.Equals(dismountId) && item.itemtype == 0);
        if (dismountItem != null)
        {
            dismountItem.equipCharacterId = 0;
            ItemDatabase.DismountItemSave(dismountId);
            Debugging.Log(dismountItem.name + " 아이템이 해제되었습니다.");
        }
    }
    public static List<Item> GetUserEquipmentItems(Common.OrderByType orderByType = Common.OrderByType.NONE)
    {
        List<Item> itemList = new List<Item>();
        foreach (var item in userItems.FindAll(item => item.count > 0 && item.itemtype==0))
        {
            if (item.itemtype == 0)
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
        if (orderByType != Common.OrderByType.NONE)
        {
            itemList = SetOrderByItemList(itemList, orderByType);
        }
        return itemList;
    }
    public static List<Item> GetUserUnEquipmentItems(Common.OrderByType orderByType = Common.OrderByType.NONE)
    {
        List<Item> itemList = new List<Item>();
        foreach (var item in userItems.FindAll(item => item.count > 0 && item.itemtype == 0&&item.equipCharacterId==0))
        {
            if (item.itemtype == 0)
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
        if (orderByType != Common.OrderByType.NONE)
        {
            itemList = SetOrderByItemList(itemList, orderByType);
        }
        return itemList;
    }
    public static int GetHeroEquipmentItemAttack(ref HeroData heroData)
    {
        int attack = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for(int i = 0; i < heroItems.Length; i++)
        {
            if(heroItems[i]!=0)
            {
                attack += ItemSystem.GetUserItem(heroItems[i]).attack;
            }
        }
        return attack;
    }

    #endregion

    #region 전체아이템정보
    public static Item GetItem(int id)
    {
        return items.Find(item => item.id == id || item.id.Equals(id));
    }
    public static int GetItemCount()
    {
        return items.Count;
    }
    public static List<Item> GetShopItems()
    {
        return items.FindAll(item => item.itemtype == 100 || item.itemtype.Equals(100));
    }
    public static Item GetRandomItem()
    {
        List<Item> ranitems = items.FindAll(item => item.itemtype != 100);
        return ranitems[Random.Range(0, ranitems.Count)];
    }
    private static List<Item> SetOrderByItemList(List<Item> itemList, Common.OrderByType orderByType)
    {
        if (itemList != null)
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
    public static Sprite GetItemNoneImage()
    {
        return Resources.Load<Sprite>("Items/none");
    }
    #endregion


}
