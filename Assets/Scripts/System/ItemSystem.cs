using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class ItemSystem
{
    /// <summary>
    /// 장비강화타입 0:없음 1:공격력 2:방어력 3:체력 4:치명타 5:공속 6:이속 7:기술력 8:관통력
    /// </summary>
    // 전체아이템데이터
    private static List<Item> items = new List<Item>();
    // 유저아이템데이터
    private static List<Item> userItems = new List<Item>();
    // 장비강화능력치상승비
    private static float[] reinforcementStatRate = { 0, 0.07f, 0.14f, 0.21f, 0.28f, 0.4f, 0.55f, 0.75f };

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
        foreach (var item in userItems.FindAll(item => item.count > 0 && IsEquipmentItem(item)))
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
    public static Item GetUserChest(int type)
    {
        Item item = null;
        if (type == 2)
        {
            item = GetUserItem(8005);
        }
        else if (type == 1)
        {
            item = GetUserItem(8004);
        }
        else
        {
            item = GetUserItem(8003);
        }
        return item;
    }
    public static int GetUserChestCount(int type)
    {
        int count = 0;
        Item item = GetUserChest(type);
        if (item != null)
            count = item.count;
        return count;
    }
    public static int GetUserMedalCount()
    {
        Item medal = GetUserMedal();
        if (medal == null)
            return 0;
        else
            return medal.count;
    }
    public static Item GetUserMedal()
    {
        return GetUserItem(8002);
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
            // 코인
            else if (obtainMoney.id > 9030 && obtainMoney.id < 9040)
                SaveSystem.AddUserCoin(obtainMoney.count);
            SaveSystem.SavePlayer();
        }
        else
        {
            Debugging.LogError("획득할 재화를 찾지못함 >> " + id);
        }
    }
    public static void SetObtainItem(int id, int count=1)
    {
        Item referenceItem = items.Find(item => item.id == id || item.id.Equals(id));
        Item obtainItem = userItems.Find(item => item.id == id || item.id.Equals(id));

        if (obtainItem != null&& referenceItem.itemtype!=0)
        {
            obtainItem.enable = true;
            obtainItem.count += count;
            ItemDatabase.ItemSave(id);
        }
        else
        {
            if(referenceItem != null)
            {
                if(referenceItem.itemtype==0)
                {
                    Item[] newItems = new Item[count];
                    for(var i = 0; i < count; i++)
                    {
                        Item newCopyItem = referenceItem.Clone() as Item;
                        newCopyItem.customId = Common.GetRandomItemId(userItems);
                        newCopyItem.enable = true;
                        newCopyItem.count = 1;
                        userItems.Add(newCopyItem);
                        newItems[i] = newCopyItem;
                    }
                    MissionSystem.AddClearPoint(MissionSystem.ClearType.CollectEquipment, count);
                    ItemDatabase.AddItemListSave(newItems);
                }
                else
                {
                    referenceItem.customId = Common.GetRandomItemId(userItems);
                    referenceItem.enable = true;
                    referenceItem.count = count;
                    userItems.Add(referenceItem);
                    Debugging.Log(referenceItem.customId);
                    ItemDatabase.AddItemSave(referenceItem);
                }
            }
        }
    }
    public static bool IsSetObtainItem(int id, int count = 1)
    {
        if(id>10000)
        {
            return IsSetObtainMoney(id, count);
        }
        else
        {
            Item referenceItem = items.Find(item => item.id == id || item.id.Equals(id));
            Item obtainItem = userItems.Find(item => item.id == id || item.id.Equals(id));

            if (obtainItem != null && referenceItem.itemtype != 0)
            {
                obtainItem.enable = true;
                obtainItem.count += count;
                ItemDatabase.ItemSave(id);
                return true;
            }
            else
            {
                if (referenceItem != null)
                {
                    if (referenceItem.itemtype == 0)
                    {
                        Item[] newItems = new Item[count];
                        for (var i = 0; i < count; i++)
                        {
                            Item newCopyItem = referenceItem.Clone() as Item;
                            newCopyItem.customId = Common.GetRandomItemId(userItems);
                            newCopyItem.enable = true;
                            newCopyItem.count = 1;
                            userItems.Add(newCopyItem);
                            newItems[i] = newCopyItem;
                        }
                        MissionSystem.AddClearPoint(MissionSystem.ClearType.CollectEquipment, count);
                        ItemDatabase.AddItemListSave(newItems);
                        return true;
                    }
                    else
                    {
                        referenceItem.customId = Common.GetRandomItemId(userItems);
                        referenceItem.enable = true;
                        referenceItem.count = count;
                        userItems.Add(referenceItem);
                        Debugging.Log(referenceItem.customId);
                        ItemDatabase.AddItemSave(referenceItem);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public static bool IsSetObtainMoney(int id, int count)
    {
        if(id==10001)
        {
            SaveSystem.AddUserCoin(count);
            return true;
        }
        else if(id==10002)
        {
            SaveSystem.AddUserCrystal(count);
            return true;
        }
        else if(id==10003)
        {
            SaveSystem.AddUserEnergy(count);
            return true;
        }
        return false;
    }
    public static void SetObtainPackageItem(int id)
    {
        if(id==9041) // 광고
        {
            User.isAdsRemove = true;
            User.isAdsSkip = true;
            SaveSystem.AddUserCrystal(50);
            GoogleSignManager.SaveData();
            Debugging.Log("광고제거 패키지 적용 성공");
        }
        else if(id==9042) // 자동 패키지
        {
            User.premiumPassDate = DateTime.Now.AddDays(31).ToShortDateString();
            GoogleSignManager.SaveData();
            Debugging.Log("자동 패키지 구매");
        }
        else if(id==9048) // 준비패키지1
        {
            if(User.isAdsRemove||User.isAdsSkip)
            {
                SaveSystem.AddUserCrystal(450);
            }
            else
            {
                User.isAdsRemove = true;
                User.isAdsSkip = true;
                SaveSystem.AddUserCrystal(400);
            }
            SaveSystem.AddUserEnergy(100);
            SaveSystem.AddUserCoin(100000);
            GoogleSignManager.SaveData();
            Debugging.Log("9048 패키지 적용 성공");
        }
        else if(id==9049) // 준비패키지2
        {
            if (User.isAdsRemove || User.isAdsSkip)
            {
                SaveSystem.AddUserCrystal(1050);
            }
            else
            {
                User.isAdsRemove = true;
                User.isAdsSkip = true;
                SaveSystem.AddUserCrystal(1000);
            }
            SaveSystem.AddUserEnergy(200);
            SaveSystem.AddUserCoin(200000);
            GoogleSignManager.SaveData();
            Debugging.Log("9049 패키지 적용 성공");
        }
        else if(id==9050) // 준비패키지3
        {
            if (User.isAdsRemove || User.isAdsSkip)
            {
                SaveSystem.AddUserCrystal(2450);
            }
            else
            {
                User.isAdsRemove = true;
                User.isAdsSkip = true;
                SaveSystem.AddUserCrystal(2400);
            }
            SetObtainItem(128);
            SaveSystem.AddUserEnergy(500);
            SaveSystem.AddUserCoin(400000);
            GoogleSignManager.SaveData();
            Debugging.Log("9050 패키지 적용 성공");
        }
    }
    public static bool IsGetAbleItem(int plus=1)
    {
        if (GetUserEquipmentItems().Count+plus <= User.inventoryCount)
            return true;
        else
            return false;
    }
    public static bool IsEquipmentItem(Item item)
    {
        if(item!=null)
        {
            if (item.itemtype == 0)
                return true;
        }
        return false;
    }
    
    public static bool UseItem(int id, int count)
    {
        Item useItem = userItems.Find(item => item.customId == id || item.customId.Equals(id));
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
        foreach (var item in userItems.FindAll(x => x.count > 0 && IsEquipmentItem(x)&&x.equipCharacterId==0))
        {
            itemList.Add(item);
        }
        itemList = SetOrderByItemList(itemList, orderByType);
        return itemList;
    }
    public static List<Item> GetUserOnEquipmentItems(Common.OrderByType orderByType = Common.OrderByType.NONE)
    {
        List<Item> itemList = new List<Item>();
        foreach (var item in userItems.FindAll(x => x.count > 0 && IsEquipmentItem(x) && x.equipCharacterId != 0))
        {
            itemList.Add(item);
        }
        itemList = SetOrderByItemList(itemList, orderByType);
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
    public static int GetHeroEquipmentItemPenetration(ref HeroData heroData)
    {
        int pent = 0;
        int[] heroItems = HeroSystem.GetHeroEquipmentItems(heroData.id);
        for (int i = 0; i < heroItems.Length; i++)
        {
            if (heroItems[i] != 0)
            {
                Item item = ItemSystem.GetUserEquipmentItem(heroItems[i]);
                pent += item != null ? ItemPenetration(item) : 0;
            }
        }
        return pent;
    }
    public static string GetEquipmentItemDescription(Item item)
    {
        StringBuilder sb = new StringBuilder();
        string[] itemAbilitiesTxt = { LocalizationManager.GetText("heroInfoAttack"), LocalizationManager.GetText("heroInfoDefence"), LocalizationManager.GetText("heroInfoHp"), LocalizationManager.GetText("heroInfoCritical"), LocalizationManager.GetText("heroInfoAttackSpeed"), LocalizationManager.GetText("heroInfoMoveSpeed"), LocalizationManager.GetText("heroInfoSkillEnergy"), LocalizationManager.GetText("heroInfoPenetration") };
        string[] itemAbilities = { ItemAttack(item).ToString(), ItemDefence(item).ToString(),ItemHp(item).ToString(), ItemCritical(item).ToString(), ItemAttackSpeed(item).ToString(), ItemMoveSpeed(item).ToString(), ItemSkillEnergy(item).ToString(), ItemPenetration(item).ToString() };

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
                    if(i==GetEnhancementType(item)-1&&item.enhancement>0)
                    {
                        sb.Append(string.Format("<color='yellow'>{0} + {1}</color>\r\n", itemAbilitiesTxt[i], itemAbilities[i]));
                    }
                    else
                    {
                        sb.Append(string.Format("<color='white'>{0} + {1}</color>\r\n", itemAbilitiesTxt[i], itemAbilities[i]));
                    }
                }
            }
        }
        sb.Append(string.Format("\r\n<color='cyan'>{0} : {1}</color>\r\n",LocalizationManager.GetText("ReinforcementRemainCount"),7-item.enhancementCount));
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
            if(item.enhancement>0&&GetEnhancementType(item)==1)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
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
            if (item.enhancement > 0 && GetEnhancementType(item) == 2)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
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
            if (item.enhancement > 0 && GetEnhancementType(item) == 3)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
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
            if (item.enhancement > 0 && GetEnhancementType(item) == 4)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
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
            if (item.enhancement > 0 && GetEnhancementType(item) == 5)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
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
            if (item.enhancement > 0 && GetEnhancementType(item) == 6)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
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
            if (item.enhancement > 0 && GetEnhancementType(item) == 7)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
            return val;
        }
        return 0;
    }
    public static int ItemPenetration(Item item)
    {
        Item primeItem = GetPrimeItem(item);
        if (primeItem != null)
        {
            int val = (int)(primeItem.penetration + ((primeItem.penetration * item.itemClass * item.itemClass) * 0.2f))/10;
            if (item.enhancement > 0 && GetEnhancementType(item) == 8)
            {
                val += (int)(val * reinforcementStatRate[item.enhancement]);
            }
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
    public static int GetEnhancementType(Item item)
    {
        Item refItem = items.Find(x => x.id.Equals(item.id) || x.id == item.id);
        if(refItem!=null)
        {
            return refItem.enhancementType;
        }
        return 0;
    }
    public static int GetDroprate(Item item)
    {
        int[] DroprateByClass = { 0, 300, 200, 50, 7, 3, 1, 0, 0 };
        if (item.id > 120 && item.id < 129)
            return -1;
        return DroprateByClass[Mathf.Clamp(item.itemClass,0,DroprateByClass.Length-1)];
    }
    public static int GetReinforceCurrentValue(Item item)
    {
        switch(GetEnhancementType(item))
        {
            case 1:
                return ItemAttack(item);
            case 2:
                return ItemDefence(item);
            case 3:
                return ItemHp(item);
            case 4:
                return ItemCritical(item);
            case 5:
                return ItemAttackSpeed(item);
            case 6:
                return ItemMoveSpeed(item);
            case 7:
                return ItemSkillEnergy(item);
            case 8:
                return ItemPenetration(item);
        }
        return 0;
    }
    public static int GetReinforceAfterValue(Item item)
    {
        Item refItem = items.Find(x => x.id == item.id || x.id.Equals(item.id)).Clone() as Item;
        if(refItem!=null)
        {
            refItem.enhancement = Mathf.Clamp(item.enhancement+1, 0, 7);
            switch (GetEnhancementType(item))
            {
                case 1:
                    return ItemAttack(refItem);
                case 2:
                    return ItemDefence(refItem);
                case 3:
                    return ItemHp(refItem);
                case 4:
                    return ItemCritical(refItem);
                case 5:
                    return ItemAttackSpeed(refItem);
                case 6:
                    return ItemMoveSpeed(refItem);
                case 7:
                    return ItemSkillEnergy(refItem);
                case 8:
                    return ItemPenetration(refItem);
            }
        }
        return 0;
    }
    public static void ReinforceItem(int customId, bool isSuccess)
    {
        Item targetItem = userItems.Find(item => (item.customId == customId || item.customId.Equals(customId)) && item.itemtype == 0);
        if (targetItem != null)
        {
            if(isSuccess)
            {
                targetItem.enhancement += 1;
            }
            targetItem.enhancementCount += 1;
            ItemDatabase.SaveEnhancement(targetItem.customId);
            Debugging.Log(targetItem.name + " 아이템 강화완료 > " + isSuccess.ToString());
        }
    }
    public static void ReinforceReinitItem(int customId)
    {
        Item targetItem = userItems.Find(item => (item.customId == customId || item.customId.Equals(customId)) && item.itemtype == 0);
        if (targetItem != null)
        {
            targetItem.enhancementCount = 0;
            targetItem.enhancement = 0;
            ItemDatabase.SaveEnhancement(targetItem.customId);
            Debugging.Log(targetItem.name + " 아이템 강화초기화 완료");
        }
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
        return items.FindAll(item => (item.itemtype == 100 || item.itemtype.Equals(100))&&item.id<10000);
    }
    public static List<Item> GetPackageItems()
    {
        return items.FindAll(item => item.itemtype == 102 || item.itemtype.Equals(102));
    }
    public static List<Item> GetCrystalShopItems(int type=3)
    {
        if(type ==0)
        {
            return items.FindAll(item => (item.itemtype == 101 || item.itemtype.Equals(101))&&item.id>9030&&item.id<=9040);
        }
        else if(type ==1)
        {
            return items.FindAll(item => (item.itemtype == 101 || item.itemtype.Equals(101)) && item.id > 9010 && item.id <= 9020);
        }
        else
        {
            return items.FindAll(item => item.itemtype == 101 || item.itemtype.Equals(101));
        }
    }
    public static Item GetRandomItem(int itemClass=0)
    {
        List<Item> ranitems = items.FindAll(item => item.itemtype < 100&&item.itemClass>itemClass);
        return ranitems[UnityEngine.Random.Range(0, ranitems.Count)];
    }
    private static List<Item> SetOrderByItemList(List<Item> itemList, Common.OrderByType orderByType)
    {
        if (itemList != null)
        {
            switch (orderByType)
            {
                case Common.OrderByType.NONE:
                    itemList.Sort((i1, i2) => i2.itemClass.CompareTo(i1.itemClass));
                    break;
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
    public static string GetItemEnhancementText(int customId)
    {
        Item data = userItems.Find(x => x.customId == customId || x.customId.Equals(customId));
        if(data != null)
        {
            if (data.enhancement > 0)
                return "+ " + data.enhancement;
        }
        return "";
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
                return "(L)";
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
    public static int GetItemReinforceSuccessRate(Item item)
    {
        if(item!=null)
        {
            int[] rate = { 80, 70, 60, 40, 20, 5, 1,0 };
            return Mathf.Clamp(rate[item.enhancement]+20-(item.itemClass*2),0,100);
        }
        return 0;
    }
    public static int GetItemReinforceNeedCost(Item item, bool scroll)
    {
        int cost = 0;
        if (item != null)
        {
            if(scroll)
            {
                cost = (item.itemClass * item.itemClass) + item.enhancement*item.itemClass; 
            }
            else
            {
                cost = ((item.itemClass * item.itemClass*item.itemClass) + item.enhancement * item.itemClass) *500;
            }
        }
        return cost;
    }
    #endregion
}
