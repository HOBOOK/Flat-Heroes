using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UI_Post : MonoBehaviour
{
    public GameObject SlotMessage;
    public GameObject MessageTabContentView;
    public GameObject postNotation;

    Text MessageContentsText;
    Text ItemNameText;
    Transform ItemParentTransform;
    Image ItemCoverImage;
    Image ItemContainerImage;
    Image ItemImage;
    Button ReceiveButton;

    private void Start()
    {
        RefreshUI(false);
    }

    private void OnEnable()
    {
        RefreshUI(true);
    }

    void RefreshUI(bool enable)
    {
        foreach (Transform child in MessageTabContentView.transform)
        {
            Destroy(child.gameObject);
        }
        if(enable)
        {
            GetPostItems();
        }
        else
        {
            StartCoroutine("GettingPostItem");
        }
    }

    IEnumerator GettingPostItem()
    {
        GoogleSignManager.Instance.GetPostMessage();
        while (string.IsNullOrEmpty(Common.postItemDatas))
        {
            Debugging.Log("nulll......");
            yield return null;
        }
        if(!Common.postItemDatas.Equals("-1"))
            PostMessageManager.Instance.AddPostMessage(Common.postItemDatas);
        GoogleSignManager.Instance.ResetPostMessage();
        GetPostItems();
        yield return null;
    }


    public void GetPostItems()
    {
        if(!string.IsNullOrEmpty(User.postItems))
        {
            try
            {
                Debugging.Log("우편메시지 >>>>> " + User.postItems);

                foreach (var item in RefreshPostMessageData())
                {
                    Debugging.Log("우편 살피는중 > " + item.ToSerialize());
                    ShowPostItemView(item);
                }
            }
            catch(System.NullReferenceException e)
            {
                Debugging.LogError(e.StackTrace);
            }

        }
    }
    public void ShowPostItemView(PostMessage post)
    {
        GameObject slotPost = Instantiate(SlotMessage, MessageTabContentView.transform);
        MessageContentsText = slotPost.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        MessageContentsText.text = string.Format("{0} <color='grey'>({1})</color>", post.message, RemainDayText(post.receiveDate));
        Item item = ItemSystem.GetItem(post.itemId);
        if(item!=null)
        {
            ReceiveButton = slotPost.transform.GetChild(2).GetChild(0).GetComponent<Button>();
            ItemParentTransform = slotPost.transform.GetChild(2).GetChild(1).transform;
            ItemNameText = ItemParentTransform.GetComponentInChildren<Text>();
            ItemCoverImage = ItemParentTransform.GetChild(0).GetChild(0).GetComponent<Image>();
            ItemContainerImage = ItemParentTransform.GetChild(0).GetChild(2).GetComponent<Image>();
            ItemImage = ItemParentTransform.GetChild(0).GetChild(1).GetComponent<Image>();

            ItemNameText.text = string.Format("{0}\r\n<size='20'>x {1}</size>",ItemSystem.GetItemName(item.id), post.itemCount);
            ItemCoverImage.sprite = ItemSystem.GetItemClassImage(item.id);
            ItemContainerImage.color = ItemColor.GetItemColor(item.itemClass);
            ItemImage.sprite = ItemSystem.GetItemImage(item.id);

            if(ReceiveButton!=null)
            {
                ReceiveButton.onClick.RemoveAllListeners();
                ReceiveButton.onClick.AddListener(delegate
                {
                    ReceivePostItem(post);
                });
            }
        }
    }

    public int GetRemainDay(string startDate)
    {
        DateTime endDate = DateTime.Parse(startDate).AddDays(30);
        TimeSpan diffDay = endDate - DateTime.Now;
        return diffDay.Days;
    }

    bool IsEnablePostMessage(string sDate, int itemId, int itemCount)
    {
        int remainDay = GetRemainDay(sDate);
        if (ItemSystem.GetItem(itemId) != null && itemCount > 0 && remainDay < 31&&remainDay>=0)
            return true;
        else
            return false;
    }
    string RemainDayText(string sDate)
    {
        return string.Format("{0}일 남음", GetRemainDay(sDate));
    }
    List<PostMessage> RefreshPostMessageData()
    {
        var postMessageDatas = new List<PostMessage>();
        string[] postMessages = User.postItems.Split(':');
        int itemId = 0;
        int itemCount = 0;
        string message = "";
        string receiveDate = "";

        foreach (var item in postMessages)
        {
            string[] postMessage = item.Split(',');
            itemId = int.Parse(postMessage[0].Replace("(", ""));
            itemCount = int.Parse(postMessage[1]);
            message = postMessage[2];
            receiveDate = postMessage[3].Replace(")", "");
            if(IsEnablePostMessage(receiveDate,itemId,itemCount))
            {
                Debugging.Log(DataSecurityManager.EncryptData(item) + " 추가");
                PostMessage postData = new PostMessage(DataSecurityManager.EncryptData(item),receiveDate, itemId, itemCount, message);
                postMessageDatas.Add(postData);
            }
        }

        StringBuilder refreshSerializeDatas = new StringBuilder();
        for(var i =0; i<postMessageDatas.Count; i++)
        {
            if(i==postMessageDatas.Count-1)
            {
                refreshSerializeDatas.Append(postMessageDatas[i].ToSerialize());
            }
            else
            {
                refreshSerializeDatas.Append(postMessageDatas[i].ToSerialize() + ":");
            }
        }
        Debugging.Log(refreshSerializeDatas.ToString());
        User.postItems = refreshSerializeDatas.ToString();
        return postMessageDatas;
    }
    public void ReceivePostItem(PostMessage post)
    {
        if(ItemSystem.IsSetObtainItem(post.itemId,post.itemCount))
        {
            RemovePostMessage(post.id);
            GoogleSignManager.SaveData();
            RefreshUI(true);
            UI_Manager.instance.ShowGetAlert(ItemSystem.GetItemImage(post.itemId), string.Format("<color='yellow'>{0}</color> x {1} {2}", ItemSystem.GetItemName(post.itemId),post.itemCount, LocalizationManager.GetText("alertGetMessage1")));
            PostCheck();
        }
    }
    void PostCheck()
    {
        if (!string.IsNullOrEmpty(User.postItems))
        {
            postNotation.SetActive(true);
        }
        else
        {
            postNotation.SetActive(false);
        }
    }
    void RemovePostMessage(string id)
    {
        var postMessageDatas = RefreshPostMessageData();
        foreach(var data in postMessageDatas)
        {
            if(data.id.Equals(id)||data.id==id)
            {
                postMessageDatas.Remove(data);
                break;
            }
        }

        StringBuilder refreshSerializeDatas = new StringBuilder();
        for (var i = 0; i < postMessageDatas.Count; i++)
        {
            if (i == postMessageDatas.Count - 1)
            {
                refreshSerializeDatas.Append(postMessageDatas[i].ToSerialize());
            }
            else
            {
                refreshSerializeDatas.Append(postMessageDatas[i].ToSerialize() + ":");
            }
        }
        Debugging.Log(refreshSerializeDatas.ToString());
        User.postItems = refreshSerializeDatas.ToString();
    }
}

[System.Serializable]
public class PostMessage
{
    [SerializeField]
    public string id;
    [SerializeField]
    public string receiveDate;
    [SerializeField]
    public int itemId;
    [SerializeField]
    public int itemCount;
    [SerializeField]
    public string message;

    PostMessage() { }

    public PostMessage(string nId,string strDate, int nItemId, int nItemCount, string strMsg)
    {
        id = nId;
        receiveDate = strDate;
        itemId = nItemId;
        itemCount = nItemCount;
        message = strMsg;
    }

    public string ToSerialize()
    {
        return string.Format("({0},{1},{2},{3})", itemId, itemCount, message,receiveDate);
    }
}
