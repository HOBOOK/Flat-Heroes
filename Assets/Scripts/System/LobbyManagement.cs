using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManagement : MonoBehaviour
{
    public static LobbyManagement instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        CharactersManager.instance.SetLobbyPositionHeros();
        SoundManager.instance.BgmSourceChange(AudioClipManager.instance.LobbyBgm);
    }
}
