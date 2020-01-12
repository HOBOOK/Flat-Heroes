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
    public AudioClip victory;
    //UI
    public AudioClip ui_shop;
    public AudioClip ui_button_default;
    public AudioClip ui_pop;
    public AudioClip ui_button_skill;
    public AudioClip ui_button_cancel;
    public AudioClip ui_roulette;
    //Skill
    public AudioClip spell;
    public AudioClip heal;
    public AudioClip teleport;
    public AudioClip smoke;
    public AudioClip fireShot;
    public AudioClip shootPistolSilence;
    public AudioClip magic;
    public AudioClip spell007;


    public AudioClip LobbyBgm;
    public AudioClip Stage_001;
    public AudioClip Stage_002;
    public AudioClip Stage_003;
    public AudioClip Stage_004;
    public AudioClip Stage_005;
    public AudioClip Stage_006;
    public AudioClip Stage_007;
    public AudioClip Stage_008;
    public AudioClip Stage_Attack;
    public AudioClip Stage_Battle;
    public AudioClip Stage_Raid;
    public AudioClip Stage_Infinity;
    public AudioClip Intro;

    public static AudioClipManager instance;

    public AudioClip StageBgm(int stageNumber)
    {
        if (stageNumber == 0 || stageNumber == 1)
            return Stage_001;
        else if (stageNumber == 2)
            return Stage_002;
        else if (stageNumber == 3)
            return Stage_003;
        else if (stageNumber == 4)
            return Stage_004;
        else if (stageNumber == 5)
            return Stage_005;
        else if (stageNumber == 6)
            return Stage_006;
        else if (stageNumber == 7)
            return Stage_007;
        else
            return Stage_008;
    }
    public AudioClip StageBattleBgm()
    {
        return Stage_Battle;
    }
    public AudioClip StageAttackBgm()
    {
        return Stage_Attack;
    }
    public AudioClip StageRaidBgm()
    {
        return Stage_Raid;
    }
    public AudioClip StageInfinityBgm()
    {
        return Stage_Infinity;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
