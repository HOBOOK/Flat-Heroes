using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        foreach (var item in userItems.FindAll(item => item.count > 0 && item.itemtype ==0))
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
    public static Item GetUserItemByCustomId(int id)
    {
        return userItems.Find(item => item.customId == id || item.customId.Equals(id));
    }
    public static Item GetUserEquipmentItem(int id)
    {
        return userItems.Find(item => (item.customId == id || item.customId.Equals(id))&&item.itemtype==0);
    }
    public static int GetUserScrollCount()
    {
        Item scroll = GetUserScroll();
        if (scroll == null)
            return 0;
        else
            return scroll.count;
    }
    public static Item GetUserScroll()
    {
        return GetUserItem(8001);
    }
    public static void SetObtainMoney(int id)
    {
        Item obtainMoney = items.Find(item => item.id == id || item.id.Equals(id));

        if (obtainMoney != null)
        {
            // 수정
            if (obtainMoney.id > 9000 && obtainMoney.id < 9010)
                SaveSystem.AddUserCrystal(obtainMoney.count);
            // 에너지
            else if (obtainMoney.id > 9010 && obtainMoney.id < 9020)
                SaveSystem.AddUserEnergy(obtainMoney.count);
            // 주문서
            else if (obtainMoney.id > 9020 && obtainMoney.id < 9030)
                SetObtainItem(8001, obtainMoney.count);
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

        if (obtainItem != null&&obtainItem.itemtype!=0)
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
                if(newItem.itemtype==0)
                {
                    Item[] newItems = new Item[count];
                    for(var i = 0; i < count; i++)
                    {
                        Item newCopyItem = newItem.Clone() as Item;
                        newCopyItem.customId = Common.GetRandomItemId(userItems);
                        newCopyItem.enable = true;
                        newCopyItem.count = 1;
                        userItems.Add(newCopyItem);
                        newItems[i] = newCopyItem;
                    }
                    MissionSystem.AddClearPoint(MissionSystem.ClearType.CollectEquipment, count);
                    foreach (var item in userItems)
                    {
                        Debugging.Log(item.customId);
                    }
                    ItemDatabase.AddItemListSave(newItems);
                }
                else
                {
                    newItem.customId = Common.GetRandomItemId(userItems);
                    newItem.enable = true;
                    newItem.count = count;
                    userItems.Add(newItem);
                    Debugging.Log(newItem.customId);
                    ItemDatabase.AddItemSave(newItem);
                }
            }
        }
    }
    
    public static bool UseItem(int id, int count)
    {
        Item useItem = userItems.Find(item => item.customId == id || item.customId.Equals(id));
        Debugging.Log("#2 >>" + useItem.name + "#" + useItem.customId + "#" + useItem.id);
        if (useItem != null)
        {
            if(useItem.count-count<=0)
            {
                //xml삭제
                userItems.Remove(useItem);
                if(useItem.itemtype==0)
                {
                    ItemDatabase.DeleteEquipItemSave(useItem.customId);
                }
                else
                {
                    ItemDatabase.DeleteItemSave(useItem.id);
                }

                Debugging.Log(useItem.name + "을 " + count + "개 사용하여 0개가 남아서 XML에서 삭제되었습니다.");
                return true;
            }
            else
            {
                useItem.count -= count;
                ItemDatabase.ItemSave(useItem.id);
                Debugging.Log(useItem.name + "을 " + count + "개 사용하여 " + useItem.count + "개 남았습니다.");
                return true;
            }

        }
        else
        {
            Debugging.LogWarning(id+" 아이템이 NULL 입니다.");
            return false;
        }
    }
    public static void EquipItem(int dismountId, int equipId, ref HeroData heroData)
    {
        Item equipItem = userItems.Find(item => (item.customId == equipId || item.customId.Equals(equipId)) && item.itemtype == 0);
        Item dismountItem = userItems.Find(item => (item.customId == dismountId || item.customId.Equals(dismountId)) && item.itemtype == 0);
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
        Item dismountItem = userItems.Find(item => (item.customId == dismountId || item.customId.Equals(dismountId)) && item.itemtype == 0);
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
            itemList.Add(item);
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
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);

                attack += item != null ? ItemAttack(item) : 0;
            }
        }
        return attack;
    }
    public static int GetHeroEquipmentItemDefence(ref HeroData heroData)
    {
        int defence = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);
                defence += item != null ? ItemDefence(item) : 0;
            }
        }
        return defence;
    }
    public static int GetHeroEquipmentItemHp(ref HeroData heroData)
    {
        int hp = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);

                hp += item != null ? ItemHp(item) : 0;
            }
        }
        return hp;
    }
    public static int GetHeroEquipmentItemCritical(ref HeroData heroData)
    {
        int cri = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);
                cri += item != null ? ItemCritical(item): 0;
            }
        }
        return cri;
    }
    public static int GetHeroEquipmentItemAttackSpeed(ref HeroData heroData)
    {
        int aSpeed = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);
                aSpeed += item != null ? ItemAttackSpeed(item) : 0;
            }
        }
        return aSpeed;
    }
    public static int GetHeroEquipmentItemMoveSpeed(ref HeroData heroData)
    {
        int mSpeed = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);
                mSpeed += item != null ? ItemMoveSpeed(item) : 0;
            }
        }
        return mSpeed;
    }
    public static int GetHeroEquipmentItemSkillEnergy(ref HeroData heroData)
    {
        int energy = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);
                energy += item != null ? ItemSkillEnergy(item) : 0;
            }
        }
        return energy;
    }
    public static string GetEquipmentItemDescription(Item item)
    {
        StringBuilder sb = new StringBuilder();
        string[] itemAbilitiesTxt = { LocalizationManager.GetText("heroInfoAttack"), LocalizationManager.GetText("heroInfoDefence"), LocalizationManager.GetText("heroInfoHp"), LocalizationManager.GetText("heroInfoCritical"), LocalizationManager.GetText("heroInfoAttackSpeed"), LocalizationManager.GetText("heroInfoMoveSpeed"), LocalizationManager.GetText("heroInfoSkillEnergy") };
        string[] itemAbilities = { ItemAttack(item).ToString(), ItemDefence(item).ToString(),ItemHp(item).ToString(), ItemCritical(item).ToString(), ItemAttackSpeed(item).ToString(), ItemMoveSpeed(item).ToString(), ItemSkillEnergy(item).ToString() };

        for(int i = 0; i < itemAbilitiesTxt.Length; i++)
        {
            if(itemAbilities[i]!="0"||!itemAbilities[i].Equals("0"))
            {
                if(int.Parse(itemAbilities[i])<0)
                {
                    sb.Append(string.Format("<color='red'>{0} {1}</color>\r\n", itemAbilitiesTxt[i], itemAbilities[i]));
                }
                else
                {
                    sb.Append(string.Format("<color='white'>{0} + {1}</color>\r\n", itemAbilitiesTxt[i], itemAbilities[i]));
                }
            }
        }
        sb.Append("\r\n"+GetItemDescription(item.id));
        return sb.ToString();
    }
    public static int GetNextClassItemId(Item item)
    {
        if(items.Find(x=>x.id==(item.id+1)||x.id.Equals((item.id+1)))!=null&&item.itemClass<8)
        {
            return item.id + 1;
        }
        return -1;
    }
    public static Item GetPrimeItem(Item item)
    {
        if(item.itemtype==0)
        {
            int primeItemId = item.id - item.itemClass+1;
            Item primeItem = items.Find(x => x.id == primeItemId || x.id.Equals(primeItemId));
            if (primeItem != null)
                return primeItem;
        }
        return null;
    }
    public static int ItemAttack(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if(primeItem!=null)
        {
            int val = primeItem.attack + (int)((primeItem.attack * item.itemClass * item.itemClass) * 0.2f);
            return val;
        }
        return 0;
    }
    public static int ItemDefence(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if (primeItem != null)
        {
            int val = primeItem.defence + (int)((primeItem.defence * item.itemClass * item.itemClass) * 0.15f);
            return val;
        }
        return 0;
    }
    public static int ItemHp(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if (primeItem != null)
        {
            int val = primeItem.hp + (int)((primeItem.hp * item.itemClass * item.itemClass) * 0.2f);
            return val;
        }
        return 0;
    }
    public static int ItemCritical(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if (primeItem != null)
        {
            int val = primeItem.critical + (int)((primeItem.critical * item.itemClass * item.itemClass) * 0.07f);
            return val;
        }
        return 0;
    }
    public static int ItemAttackSpeed(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if (primeItem != null)
        {
            int val = primeItem.attackSpeed + (int)((primeItem.attackSpeed * item.itemClass * item.itemClass) * 0.1f);
            return val;
        }
        return 0;
    }
    public static int ItemMoveSpeed(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if (primeItem != null)
        {
            int val = primeItem.moveSpeed + (int)((primeItem.moveSpeed * item.itemClass * item.itemClass) * 0.09f);
            return val;
        }
        return 0;
    }
    public static int ItemSkillEnergy(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if (primeItem != null)
        {
            int val = primeItem.skillEnergy + (int)((primeItem.skillEnergy * item.itemClass * item.itemClass) * 0.2f);
            return val;
        }
        return 0;
    }
    public static int ItemValue(Item item)
    {
        if (item.itemtype == 0)
        {
            int val = 0;
            if (GetPrimeItem(item)!=null)
                val = GetPrimeItem(item).value;
            int finalVal = val + (int)((val * item.itemClass * item.itemClass * item.itemClass * item.itemClass) * 0.1f);
            return finalVal;
        }
        else
            return item.value;
    }
    public static int GetDroprate(Item item)
    {
        int itemClass = item.itemClass;
        return (int)(200 * (128 - (itemClass * itemClass * 3)) * 0.005f);
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
    public static List<Item> GetEquipmentItems()
    {
        return items.FindAll(item => (item.itemtype == 0 || item.itemtype.Equals(0)) &&item.itemClass>0);
    }
    public static List<Item> GetShopItems()
    {
        return items.FindAll(item => item.itemtype == 100 || item.itemtype.Equals(100));
    }
    public static List<Item> GetCrystalShopItems()
    {
        return items.FindAll(item => item.itemtype == 101 || item.itemtype.Equals(101));
    }
    public static Item GetRandomItem()
    {
        List<Item> ranitems = items.FindAll(item => item.itemtype < 100);
        return ranitems[UnityEngine.Random.Range(0, ranitems.Count)];
    }
    private static List<Item> SetOrderByItemList(List<Item> itemList, Common.OrderByType orderByType)
    {
        if (itemList != null)
        {
            switch (orderByType)
            {
                case Common.OrderByType.NAME:
                    itemList.Sort((i1, i2) => GetItemName(i1.id).CompareTo(GetItemName(i2.id)));
                    break;
                case Common.OrderByType.VALUE:
                    itemList.Sort((i1, i2) => i2.itemClass.CompareTo(i1.itemClass));
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
    public static Sprite GetItemImage(int id, bool isCustomId = false)
    {
        Item item;
        if (isCustomId)
        {
            id = userItems.Find(x => x.customId == id).id;
        }
        item = items.Find(x => x.id == id);
        if (item == null)
            return GetItemNoneImage();
        else
            return Resources.Load<Sprite>(item.image);
    }
    public static Sprite GetItemNoneImage()
    {
        return Resources.Load<Sprite>("UI/ui_none2");
    }
    public static string GetItemName(int id)
    {
        string name = null;
        Item data = items.Find(x => x.id == id || x.id.Equals(id));
        if (data != null)
        {
            if(data.itemtype==0)
            {
                if (GetPrimeItem(data) != null)
                    name = string.Format("{0} {1}", LocalizationManager.GetText("ItemClassName" + data.itemClass), LocalizationManager.GetText("ItemName" + GetPrimeItem(data).id));
                else
                    name = string.Format("{0}", LocalizationManager.GetText("ItemNameNull"));
            }
            else
                name = string.Format("{0}", LocalizationManager.GetText("ItemName"+data.id));

        }
        else
            name = string.Format("{0}", LocalizationManager.GetText("ItemNameNull"));
        return name;
    }
    public static string GetItemDescription(int id)
    {
        string des = null;
        Item data = items.Find(x => x.id == id || x.id.Equals(id));
        if (data != null)
        {
            des = LocalizationManager.GetText("ItemDescription" + id);
        }
        return des;
    }
    public static string GetIemClassName(int classNumber)
    {
        switch(classNumber)
        {
            case 1:
                return "(D)";
            case 2:
                return "(C)";
            case 3:
                return "(B)";
            case 4:
                return "(A)";
            case 5:
                return "(S)";
            case 6:
                return "(SS)";
            case 7:
                return "(SSS)";
            case 8:
                return "(Lengedary)";
        }
        return "";
    }
    public static Sprite GetItemClassImage(int id, bool isCustomId=false)
    {
        Item item;
        if (isCustomId)
        {
            id = userItems.Find(x => x.customId == id).id;
        }
        item = items.Find(x => x.id == id);
        if (item!=null&&item.itemtype==0)
        {
            switch(item.itemClass)
            {
                case 8:
                    return Resources.Load<Sprite>("Class/Legend");
                case 7:
                    return Resources.Load<Sprite>("Class/SSS");
                case 6:
                    return Resources.Load<Sprite>("Class/SS");
                case 5:
                    return Resources.Load<Sprite>("Class/S");
            }
            return GetItemNoneImage();
        }
        return GetItemNoneImage();
    }
    #endregion


}
