using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System;

public class ChatRoomManager : MonoBehaviour, IChatClientListener
{
    public InputField inputTextField;
    public GameObject ChatBoxSlot;
    public GameObject MyChatBoxSlot;
    public GameObject SysChatBoxSlot;
    public GameObject ChatUserSlot;
    public Transform ChatUserListTransform;
    public Transform ChatViewParentTransform;
    public Transform PreviewTransform;

    private ChatClient chatClient;
    private string currentChannelName;
    private Dictionary<string, GameObject> userList = new Dictionary<string, GameObject>();

    private float previewTime = 3.0f;
    private float previewingTime = 0.0f;
    private bool isPreview = false;

    void Start()
    {
        Application.runInBackground = true;

        currentChannelName = "Channel 001";

        chatClient = new ChatClient(this);
        chatClient.Connect(ChatSettings.Instance.AppId, "1.0", new AuthenticationValues(User.name+","+User.profileHero+","+User.level));
    }

    private void OnApplicationQuit()
    {
        if (chatClient != null)
        {
            chatClient.PublishMessage(currentChannelName, "o|" + User.name);
            chatClient.Disconnect();
        }
    }
    private void OnDisable()
    {
        if (chatClient != null)
        {
            try
            {
                chatClient.PublishMessage(currentChannelName, "o|" + User.name);
            }
            catch
            {

            }
            chatClient.Disconnect();
        }
    }
    private void OnDestroy()
    {
        if (chatClient != null)
        {
            try
            {
                chatClient.PublishMessage(currentChannelName, "o|" + User.name);
            }
            catch
            {

            }

            chatClient.Disconnect();
        }
    }
    void AddUser(string name, string profile)
    {
        if(!userList.ContainsKey(name))
        {
            GameObject chatUser = Instantiate(ChatUserSlot, ChatUserListTransform);
            chatUser.GetComponentInChildren<Text>().text = name;
            chatUser.transform.GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroThumbnail(int.Parse(profile));
            chatUser.gameObject.SetActive(true);
            userList.Add(name, chatUser);
            Debugging.Log(name+" 목록 추가");
        }
            
    }
    void DeleteUser(string name)
    {
        if (userList.ContainsKey(name))
        {
            Debugging.Log(name + " 목록 제거");
            Destroy(userList[name]);
            userList.Remove(name);
        }
    }

    void AddLine(string msg)
    {
        GameObject chatbox = Instantiate(SysChatBoxSlot, ChatViewParentTransform);
        chatbox.GetComponentInChildren<Text>().text = msg;
        chatbox.gameObject.SetActive(true);
    }
    void AddChatLine(string msg, string name, string profile, string lv)
    {
        GameObject chatbox = Instantiate(ChatBoxSlot, ChatViewParentTransform);
        chatbox.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroThumbnail(int.Parse(profile));
        chatbox.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = string.Format("{0} (Lv {1})   <color='gray'><size='20'>{2}</size></color>", name, lv, DateTime.Now.ToShortTimeString());
        chatbox.transform.GetChild(0).GetChild(1).GetChild(1).GetComponentInChildren<Text>().text = msg;
        chatbox.gameObject.SetActive(true);
        PreviewLine(name, msg);
    }
    void AddChatLineMe(string msg, string name, string profile, string lv)
    {
        GameObject chatbox = Instantiate(MyChatBoxSlot, ChatViewParentTransform);
        chatbox.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = HeroSystem.GetHeroThumbnail(int.Parse(profile));
        chatbox.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format("<color='gray'><size='20'>{0}</size></color>   {1} (Lv {2})", DateTime.Now.ToShortTimeString(), name,lv);
        chatbox.transform.GetChild(0).GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = msg;
        chatbox.gameObject.SetActive(true);
        PreviewLine(name, msg);
    }

    void DeleteLine()
    {
        if(ChatViewParentTransform.childCount>100)
        {
            Destroy(ChatViewParentTransform.GetChild(0).gameObject);
        }
    }

    #region IChatClientListenr 인터페이스 구현구분
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogWarning(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnConnected()
    {
        AddLine("채팅서버에 연결되었습니다.");

        //chatClient.Subscribe(new string[]{currentChannelName}, 10);
        chatClient.Subscribe(new string[] { currentChannelName, "Channel 002" }, 10);
        chatClient.SetOnlineStatus(ChatUserStatus.Offline, null);
        chatClient.PublishMessage(currentChannelName, "j|"+User.name);
    }

    public void OnDisconnected()
    {
        AddLine("채팅서버에 연결이 끊어졌습니다.");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //AddLine(string.Format("채널 입장 ({0})", string.Join(",", channels)));
    }

    public void OnUnsubscribed(string[] channels)
    {
        chatClient.PublishMessage(currentChannelName, "o|" + User.name);
        //AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            if(messages[i].ToString().Contains("|"))
            {
                if(messages[i].ToString().Split('|')[0]=="j")
                {
                    AddUser(senders[i].Split(',')[0], senders[i].Split(',')[1]);
                }
                else if(messages[i].ToString().Split('|')[0]=="o")
                {
                    DeleteUser(senders[i].Split(',')[0]);
                }
                else
                {
                    if (senders[i].Split(',')[0].Equals(User.name) || senders[i].Split(',')[0] == User.name)
                        AddChatLineMe(messages[i].ToString().Split('|')[1], senders[i].Split(',')[0], senders[i].Split(',')[1], senders[i].Split(',')[2]);
                    else
                    {
                        AddUser(senders[i].Split(',')[0], senders[i].Split(',')[1]);
                        AddChatLine(messages[i].ToString().Split('|')[1], senders[i].Split(',')[0], senders[i].Split(',')[1], senders[i].Split(',')[2]);
                    }

                }
            }
            DeleteLine();
            //AddLine(string.Format("{0} : {1}", senders[i].Split(',')[0], messages[i].ToString()));

        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    void Update()
    {
        chatClient.Service();
        if(chatClient!=null&&isPreview)
        {
            previewingTime += Time.deltaTime;
            if(previewingTime>=previewTime)
            {
                isPreview = false;
                previewingTime = 0.0f;
            }
        }
        if (!isPreview)
            PreviewTransform.gameObject.SetActive(false);

    }

    void PreviewLine(string name, string txt)
    {
        previewingTime = 0.0f;
        isPreview = true;
        PreviewTransform.gameObject.SetActive(true);
        PreviewTransform.GetComponentInChildren<Text>().text = string.Format("{0} : {1}", name, txt);
    }

    public void Input_OnEndEdit(string text)
    {
        if (chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            // public
            if(!string.IsNullOrEmpty(inputTextField.text)&&!inputTextField.text.Contains("|"))
                chatClient.PublishMessage(currentChannelName, "n|"+inputTextField.text);

            // private
            //chatClient.SendPrivateMessage("ethan", inputField.text);


            inputTextField.text = "";
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        AddLine(string.Format("{0} : {1}", channel, user));
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        AddLine(string.Format("{0} : {1}", channel, user));
    }

    #endregion
}
