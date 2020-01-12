// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatAppIdCheckerUI.cs" company="Exit Games GmbH">
//   Part of: PhotonChat demo, 
// </copyright>                                                                                             
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

#if PHOTON_UNITY_NETWORKING

using Photon.Pun;


/// <summary>
/// This is used in the Editor Splash to properly inform the developer about the chat AppId requirement.
/// </summary>
[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
    public Text Description;

    public void Update()
    {
		if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
        {
            Description.text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
        }
        else
        {
            Description.text = string.Empty;
        }
    }
}
#else

[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
    public Text Description;

    public void Update()
    {
        if (ChatSettings.Instance== null || string.IsNullOrEmpty(ChatSettings.Instance.AppId))
        {
            Description.text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the ChatSettings file.";
        }
        else
        {
            Description.text = string.Empty;
        }
    }
}

#endif