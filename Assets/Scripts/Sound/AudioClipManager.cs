using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    public AudioClip coin;
    public AudioClip coinGet;
    public AudioClip dropItem;
    public AudioClip reloadPistol;
    public AudioClip shootPistol;
    public AudioClip swingSword;
    public AudioClip swingKnife;
    public AudioClip punchHit;
    public AudioClip jump;
    public AudioClip heartBeat;
    public AudioClip knife1;
    public AudioClip knife2;
    public AudioClip bow;
    public AudioClip damage1;
    public AudioClip damage2;
    public AudioClip pickup;
    public AudioClip equip;
    public AudioClip stoneCrack;
    public AudioClip stoneRolling;
    public AudioClip rumble;
    public AudioClip grasscut;
    public AudioClip dead;
    public AudioClip levelup;
    //UI
    public AudioClip ui_shop;
    public AudioClip ui_button_default;
    public AudioClip ui_pop;
    public AudioClip ui_button_skill;
    //Skill
    public AudioClip spell;
    public AudioClip heal;
    public AudioClip teleport;
    public AudioClip smoke;
    public AudioClip fireShot;
    public AudioClip shootPistolSilence;
    public AudioClip magic;
    

    public AudioClip Bgm1;
    public AudioClip Bgm2;

    public static AudioClipManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
