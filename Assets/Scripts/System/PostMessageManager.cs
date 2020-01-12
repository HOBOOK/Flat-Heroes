using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PostMessageManager : MonoBehaviour
{
    private static PostMessageManager instance = null;
    public static PostMessageManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<PostMessageManager>();
            }
            if(instance==null)
            {
                Debugging.LogWarning("포스트매니저 오류");
                return instance;
            }
            return instance;
        }
    }
    public int GetRemainDay(string startDate)
    {
        DateTime endDate = DateTime.Parse(startDate).AddDays(31);
        TimeSpan diffDay = endDate - DateTime.Now;
        return diffDay.Days;
    }

    public void AddPostMessage(string msg)
    {
        var postMessageDatas = new List<PostMessage>();

        int itemId = 0;
        int itemCount = 0;
        string message = "";
        string receiveDate = "";
        if(!string.IsNullOrEmpty(User.postItems))
        {
            string[] postMessages = User.postItems.Split(':');
            foreach (var item in postMessages)
            {
                string[] postMessage = item.Split(',');
                itemId = int.Parse(postMessage[0].Replace("(", ""));
                itemCount = int.Parse(postMessage[1]);
                message = postMessage[2];
                receiveDate = postMessage[3].Replace(")", "");
                if (IsEnablePostMessage(receiveDate, itemId, itemCount))
                {
                    PostMessage postData = new PostMessage(DataSecurityManager.EncryptData(item), receiveDate, itemId, itemCount, message);
                    postMessageDatas.Add(postData);
                }
            }
        }
        if(!string.IsNullOrEmpty(msg))
        {
            string[] addPostMessages = msg.Split(':');
            foreach (var item in addPostMessages)
            {
                string[] postMessage = item.Split(',');
                itemId = int.Parse(postMessage[0].Replace("(", ""));
                itemCount = int.Parse(postMessage[1]);
                message = postMessage[2];
                receiveDate = postMessage[3].Replace(")", "");
                Debugging.Log(receiveDate);
                if (IsEnablePostMessage(receiveDate, itemId, itemCount))
                {
                    Debugging.Log(DataSecurityManager.EncryptData(item) + " 추가");
                    PostMessage postData = new PostMessage(DataSecurityManager.EncryptData(item), receiveDate, itemId, itemCount, message);
                    postMessageDatas.Add(postData);
                }
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

    bool IsEnablePostMessage(string sDate, int itemId, int itemCount)
    {
        int remainDay = GetRemainDay(sDate);
        Debugging.Log(remainDay);
        if (ItemSystem.GetItem(itemId) != null && itemCount > 0 && remainDay <= 31 && remainDay >= 0)
            return true;
        else
            return false;
    }
}
