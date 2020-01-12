using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    #region 변수
    //id
    public int id;
    //string
    public string HeroName;
    public List<string> initChats = new List<string>();
    public List<string> playChats = new List<string>();
    //public List<string> endChats = new List<string>();
    //boolean

    public bool isPlayerHero = false;
    public bool isStage = false;
    public bool isDead = false;
    public bool isUnBeat = false;
    public bool isCriticalAttack = false;
    public bool isFriend = false;
    public bool isStatic = false;
    private bool isAttack = false;
    private bool isStart = false;
    private bool isNeutrality;
    private bool isHold = false;
    private bool isLeftorRight = false;
    private bool isSkillAttack = false;
    private bool isWait = false;
    private bool isTrack = false;
    private bool isJumping = false;
    private bool isDefence = false;
    private bool isStun = false;
    private bool isStunning = false;
    private bool isClimb = false;
    private bool isClimbing = false;
    private bool isAir = false;
    private bool isAirborne = false;
    private bool isReached = false;
    private bool isPvpSetting = false;
    private bool isPersistentHitted = false;

    //Integer,float
    float distanceBetweenTarget;
    float standardTime = 5.0f;
    float animationTime = 0.0f;
    float recoveryHpTime;
    private float attackMinRange = 0f;
    private float attackMaxRange = 1.0f;
    float attackRange;
    int randomStatus = 0;
    int attackNumber = 0;
    int stageModeType = 0;
    //GameObject
    public GameObject target;
    List<GameObject> enemys = new List<GameObject>();
    List<GameObject> allys = new List<GameObject>();
    public GameObject weaponPoint;
    public GameObject weaponPoint2;
    public GameObject attackPoint;
    public GameObject stageHeroProfileUI;
    GameObject transcendenceEffect;
    GameObject shadow;
    GameObject hpUI;
    //Vector3
    Vector3 firstPos = Vector3.zero;
    Vector3 wallPos;
    Vector3 pos;
    //RaycastHit2D
    RaycastHit2D checkJumpHit2D;
    RaycastHit2D checkTurnHit2D;
    //Enum
    public HeroState heroState;
    public WeaponType weaponType;
    public enum HeroState { Normal, Attack  }
    public enum WeaponType { No,Gun,Sword,Knife,Staff,Bow,Heal,Support,Zombie,Raser,DoubleSword,Wand,Shuriken}
    //Rigidbody2D
    Rigidbody2D rigid;
    //Animator
    public Animator animator;
    Animator faceAnimator;
    //InnerClass
    public Status status;
    //HeroData
    public HeroData heroData;
    Skill skillData;
    //EquipItems
    List<Item> equipItems = new List<Item>();
    //Buff boolean
    bool isAttackBuff = false;
    bool isDefenceBuff = false;
    bool isWaveBuff = false;
    #endregion

    #region Awake,Start,Update
    void Start()
    {
        InitHero();
        StartHero();

        if (isPlayerHero)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
            isLeftorRight = false;
        }
        else
        {
            isLeftorRight = true;
        }
        if(transform.Find("shadow")!=null)
            shadow = transform.Find("shadow").gameObject;
        ShadowOnOff(true);
        animator = GetComponent<Animator>();
        if(GetComponentInChildren<faceOff>()!=null)
            faceAnimator = GetComponentInChildren<faceOff>().GetFaceAnimator();
        rigid = GetComponent<Rigidbody2D>();
        if (attackPoint.GetComponent<SpriteRenderer>() != null)
            attackPoint.GetComponent<SpriteRenderer>().enabled = false;
        SetAttackAnimation();
        RemoveWeapon();
        if(isStage)
        {
            if(Common.stageModeType==Common.StageModeType.Battle)
            {
                StartCoroutine("StartPvpMode");
            }
            else
            {
                if (isPlayerHero)
                    StartCoroutine("StartAttackMode");
                else
                    StartCoroutine("StartMonsterAttackMode");
            }
        }
        else
        {
            isStart = true;
        }
    }

    void Update()
    {
        if (isDead)
            return;
        FindEnemys();
        StateChecking();
        RePositioningHero();
        Die();
    }
    private void FixedUpdate()
    {
        if (isDead)
            return;
        Jump();
        SkyCheck();
        Climb();
        Running();
    }
    private void OnDisable()
    {
        MusicEffectOff();
        if(EffectPool.Instance != null&&transcendenceEffect != null)
        {
            EffectPool.Instance.PushToPool("GlowOrbPink", transcendenceEffect);
        }
    }
    private void OnDestroy()
    {
        MusicEffectOff();
        if (EffectPool.Instance != null && transcendenceEffect != null)
        {
            EffectPool.Instance.PushToPool("GlowOrbPink", transcendenceEffect);
        }
    }
    #endregion

    #region 계산함수
    public int ATTACK
    {
        get
        {
            return status.attack + OnReturnHeroAbility(4);
        }
        set { }
    }
    public int GetDefence(float pent)
    {
        int def = status.defence + OnReturnHeroAbility(5);
        return (int)(def - (def * pent));
    }
    public int Damage(bool isCtk = false)
    {
        if (!isCtk)
        {
            if (UnityEngine.Random.Range(1, 100) <= this.status.criticalPercent)
                isCriticalAttack = true;
            else
                isCriticalAttack = false;
        }
        else
            isCriticalAttack = true;

        float dam = ATTACK;
        dam = isSkillAttack ? dam * Mathf.Clamp((SkillSystem.GetUserSkillPower(heroData.skill)*0.01f),0f,100f) : dam;
        dam = UnityEngine.Random.Range(dam * 0.8f, dam * 1.2f);
        dam = isCriticalAttack ? dam * (1.5f+OnReturnHeroAbility(3)*0.01f) : dam;
        return (int)dam;
    }
    public string SetDamageData(int nDam, float nPent)
    {
        return nDam + "/" + nPent;
    }
    public int DamageDataDam(string attackPointText)
    {
        if (attackPointText.Contains("/"))
            return int.Parse(attackPointText.Substring(0, attackPointText.IndexOf('/')));
        else
            return 0;
    }
    public float DamageDataPent(string attackPointText)
    {
        return float.Parse(attackPointText.Substring(attackPointText.IndexOf('/')+1));
    }
    public bool IsReached()
    {
        return isReached;
    }


    public int SkillDamage(bool isCtk = false)
    {
        if (!isCtk)
        {
            if (UnityEngine.Random.Range(1, 100) <= this.status.criticalPercent)
                isCriticalAttack = true;
            else
                isCriticalAttack = false;
        }
        else
            isCriticalAttack = true;

        float dam = ATTACK;
        dam = dam * Mathf.Clamp((SkillSystem.GetUserSkillPower(heroData.skill) * 0.01f), 0f, 100f);
        dam = UnityEngine.Random.Range(dam * 0.8f, dam * 1.2f);
        dam = isCriticalAttack ? dam * 1.5f : dam;
        return (int)dam;
    }
    public bool isHealAble()
    {
        if (status.hp == status.maxHp)
            return false;
        return true;
    }
    void RecoveryHp()
    {
        recoveryHpTime += Time.deltaTime;
        if(!isDead&&status.hp>0&&recoveryHpTime>1.0f)
        {
            status.hp=Common.looHpPlus(status.hp, status.maxHp, HeroSystem.GetRecoveryHp(ref heroData));
            recoveryHpTime = 0;
        }
    }
    void DistanceChecking()
    {
        if (target != null)
            distanceBetweenTarget = Common.GetDistanceBetweenAnother(transform, target.transform);
    }
    void RemoveWeapon()
    {
        if (weaponPoint!=null&&weaponPoint.transform.childCount > 0 && weaponPoint.transform.GetChild(0).gameObject.activeSelf)
        {
            animator.SetInteger("weaponType", -1);
            weaponPoint.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (weaponPoint2!=null&&weaponPoint2.transform.childCount > 0 && weaponPoint2.transform.GetChild(0).gameObject.activeSelf)
        {
            weaponPoint2.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    void InitEnemys()
    {
        Transform Characters = null;
        if (isPlayerHero)
        {
            Characters = GameObject.Find("EnemysHero").transform;
        }
        else
        {
            Characters = GameObject.Find("PlayersHero").transform;
        }

        if (Characters != null)
        {
            foreach (var character in Characters.GetComponentsInChildren<Hero>())
            {
                if (character.gameObject.activeSelf && isPlayerHero != character.isPlayerHero)
                {
                    enemys.Add(character.gameObject);
                }
            }
        }
    }
    IEnumerator FindAllys()
    {
        while(!isDead&&isFriend && isStage)
        {
            if (!isAttack && !isClimb && !isClimbing && !isWait)
            {
                allys = Common.FindAlly(this.isPlayerHero);
                allys.Remove(this.gameObject);
                if (allys != null && allys.Count > 0)
                {
                    float tempHp = (float)allys[0].GetComponent<Hero>().status.hp/(float)allys[0].GetComponent<Hero>().status.maxHp;
                    int targetIndex = 0;
                    for (var i = 1; i < allys.Count; i++)
                    {
                        if (tempHp > (float)allys[i].GetComponent<Hero>().status.hp / (float)allys[i].GetComponent<Hero>().status.maxHp && TargetAliveCheck(allys[i]))
                        {
                            targetIndex = i;
                            tempHp = (float)allys[i].GetComponent<Hero>().status.hp / (float)allys[i].GetComponent<Hero>().status.maxHp;
                        }
                    }
                    target = allys[targetIndex].gameObject;
                }
                else
                {
                    target = null;
                }
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;

    }
    void FindEnemys(bool isForce=false)
    {
        if(!isAttack  && !isClimb && !isClimbing&&isStage&&!isFriend)
        {
            if (!isForce)
            {
                if (target == null && enemys != null && enemys.Count > 0)
                {
                    foreach (var character in enemys.ToArray())
                    {
                        if (!TargetAliveCheck(character))
                        {
                            enemys.Remove(character);
                        }
                    }

                    if (enemys.Count > 0)
                    {
                        int targetIndex = 0;
                        float tempTargetDistance = Common.GetDistanceBetweenAnother(transform, enemys[0].transform);
                        for (int i = 1; i < enemys.Count; i++)
                        {
                            if (tempTargetDistance > Common.GetDistanceBetweenAnother(transform, enemys[i].transform))
                            {
                                targetIndex = i;
                                tempTargetDistance = Common.GetDistanceBetweenAnother(transform, enemys[i].transform);
                            }
                        }
                        target = enemys[targetIndex].gameObject;
                     
                        //Debugging.Log(name + " 의 타겟 >> " + target.name);
                    }
                    else
                    {
                        //Debugging.Log(name + " 의 타겟 찾을 수 없음. ");
                    }
                }
            }
            else
            {
                if (enemys.Count > 0)
                {
                    int targetIndex = 0;
                    float tempTargetDistance = Common.GetDistanceBetweenAnother(transform, enemys[0].transform);
                    for (int i = 1; i < enemys.Count; i++)
                    {
                        if (tempTargetDistance > Common.GetDistanceBetweenAnother(transform, enemys[i].transform))
                        {
                            targetIndex = i;
                        }
                        tempTargetDistance = Common.GetDistanceBetweenAnother(transform, enemys[i - 1].transform);
                    }
                    target = enemys[targetIndex].gameObject;
                    //Debugging.Log(name + " 의 타겟 강제지정 >> " + target.name);
                }
            }
            if (target != null)
            {
                if (TargetDeadCheck(target))
                {
                    target = null;
                }
                else if (IsLimitXOutRange(target.transform))
                {
                    target = null;
                }
            }
            if(isPlayerHero)
            {
                if (target == null && enemys.Count < 1)
                {
                    enemys = Common.FindEnemy(this.isPlayerHero);
                    // 캐슬타겟
                    if (Common.hitTargetObject != null&&enemys.Count<1)
                    {
                        if(Common.hitTargetObject.GetComponent<Castle>()!=null&&!Common.hitTargetObject.GetComponent<Castle>().isDead)
                        {
                            target = Common.hitTargetObject;
                        }
                        else if(Common.hitTargetObject.GetComponent<Boss>()!=null&&!Common.hitTargetObject.GetComponent<Boss>().bossPrefabData.isDead)
                        {
                            target = Common.hitTargetObject;
                        }
                    }
                }
            }
            else
            {
                if (target == null && enemys.Count < 1)
                {
                    enemys = Common.FindEnemy(this.isPlayerHero);
                    // 캐슬타겟
                    if (Common.allyTargetObject != null && enemys.Count < 1)
                    {
                        if (Common.allyTargetObject.GetComponent<Castle>() != null && !Common.allyTargetObject.GetComponent<Castle>().isDead)
                        {
                            target = Common.allyTargetObject;
                        }
                    }
                }
            }
            if(Common.stageModeType==Common.StageModeType.Main)
            {
                if (Common.hitTargetObject == null)
                {
                    if (!isPlayerHero)
                        status.hp = 0;
                    target = null;
                }
                if (StageManagement.instance.stageInfo.stageType == 0)
                {
                    if (Common.allyTargetObject == null)
                    {
                        if (isPlayerHero)
                            status.hp = 0;
                        target = null;
                    }
                }
            }
        }
    }
    public void ResearchingEnemys(GameObject enemy)
    {
        if(TargetAliveCheck(enemy))
        {
            enemys.Add(enemy);
            if (target == Common.hitTargetObject||target==Common.allyTargetObject)
            {
                FindEnemys(true);
            }
        }
    }
    bool IsLimitXOutRange(Transform tran)
    {
        if (stageModeType != (int)Common.SCENE.INFINITE||!isPlayerHero)
            return false;
        else
        {
            if (tran.position.x > 6.5f)
            {
                return true;
            }
        }
        return false;
    }
    bool TargetAliveCheck(GameObject obj)
    {
        if (obj != null && obj.GetComponent<Hero>() != null && !obj.GetComponent<Hero>().isDead)
            return true;
        else
            return false;
    }
    bool TargetDeadCheck(GameObject obj)
    {
        if (obj != null && obj.GetComponent<Hero>() != null && obj.GetComponent<Hero>().isDead)
            return true;
        else
            return false;
    }
    private void SkyCheck()
    {
        if (rigid.velocity.y < -3)
        {
            if (!isUnBeat)
                animator.SetBool("isJumping", true);
        }
        else if (rigid.velocity.y <= 0 && animator.GetBool("isJumping"))
        {
            LandingCheck();
        }
        if (isAir)
        {
            StartCoroutine("Airborne");
        }
        if (isAirborne && isUnBeat && rigid.gravityScale < 1)
        {
            rigid.gravityScale = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
        }
    }
    void CheckHurdle()
    {
        //점프 레이어 (필드 오브젝트)
        int layerMask = 1 << 30;
        checkJumpHit2D = Physics2D.Raycast(transform.position + new Vector3(0, -0.6f, 0), isLeftorRight ? Vector2.left : Vector2.right, 0.2f, layerMask);
        Debug.DrawRay(transform.position + new Vector3(0, 0, 0), isLeftorRight ? Vector2.left : Vector2.right, Color.red, 0.2f);
        if (checkJumpHit2D)
        {
            isJumping = true;
        }
        //방향전환 레이어 (필드)
        layerMask = 1 << 31;
        checkTurnHit2D = Physics2D.Raycast(transform.position, isLeftorRight ? Vector2.left : Vector2.right, 0.2f, layerMask);
        Debug.DrawRay(transform.position, isLeftorRight ? Vector2.left : Vector2.right, Color.blue, 0.2f);
        if (checkTurnHit2D)
        {
            isLeftorRight = !isLeftorRight;
        }
    }
    private void LandingCheck()
    {
        if (rigid.velocity.y <= 0 && animator.GetBool("isJumping"))
        {
            int layerMask = 1 << 31;
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.localPosition, Vector2.down, 1.5f, layerMask);
            Debug.DrawRay(transform.localPosition, Vector3.down * 1.5f, Color.blue, 1f);
            foreach (var i in hit)
            {
                if (i.collider && !i.collider.isTrigger)
                {
                    GroundEffect();
                    animator.SetBool("isJumping", false);
                }
            }
        }
    }
    private bool IsTargetLeft(Transform tran)
    {
        if(tran.transform.position.x > this.transform.position.x)
        {
            return false;
        }
        return true;
    }
    #endregion

    #region 영웅세팅
    void InitHero()
    {
        if (Common.GetSceneCompareTo(Common.SCENE.STAGE) || Common.GetSceneCompareTo(Common.SCENE.INFINITE) || Common.GetSceneCompareTo(Common.SCENE.BOSS) || Common.GetSceneCompareTo(Common.SCENE.BATTLE) || Common.GetSceneCompareTo(Common.SCENE.ATTACK)) 
        {
            isStage = true;
            stageModeType = Common.GetSceneNumber();
        }
        this.tag = isPlayerHero ? "Hero" : "Enemy";
        InitSpriteLayer();
        if (isStage)
        {
            if (isPlayerHero)
            {
                this.transform.parent = GameObject.Find("PlayersHero").transform;
            }
            else
            {
                this.transform.parent = GameObject.Find("EnemysHero").transform;
            }

        }

        SpriteAlphaSetting(0);
    }
    void StartHero()
    {
        // STATUS 설정
        isPvpSetting = Common.stageModeType == Common.StageModeType.Battle;
        if (!isPvpSetting)
        {
            if (isPlayerHero)
                heroData = HeroSystem.GetUserHero(id);
            else
                heroData = HeroSystem.GetHero(id);
        }
        else
        {
            if (isPlayerHero)
                heroData = HeroSystem.GetUserHero(id);
        }
        if (heroData!=null)
        {
            if(isPvpSetting&&!isPlayerHero)
                this.status.SetPvpStatus(ref heroData, this);
            else
                this.status.SetHeroStatus(ref heroData, false, this);
            BossModeDifficultyBuff();
            SetSkin(heroData);

            this.HeroName = HeroSystem.GetHeroName(heroData.id);
            this.name = HeroName;
            initChats = HeroSystem.GetHeroChats(heroData.id);
            skillData = SkillSystem.GetSkill(heroData.skill);
            // 보스의 경우
            if(this.id>1000)
            {
                if (Common.stageModeType != Common.StageModeType.Infinite)
                {
                    this.GetComponent<Boss>().enabled = true;
                    this.GetComponent<Boss>().StartBoss();
                }
                else
                    this.GetComponent<Boss>().enabled = false;
            }
        }
        else
        {
            Debugging.LogWarning(this.name + " 영웅의 Status 데이터 받아오기 실패.");
        }
    }
    void ShadowOnOff(bool on)
    {
        if(shadow!=null)
        {
            if (on)
                shadow.SetActive(true);
            else
                shadow.SetActive(false);
        }
    }
    public void SpriteAlphaSetting(float alpha)
    {
        if(isPlayerHero&&isStage&&Common.stageModeType!=Common.StageModeType.Battle)
        {
            foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            }
        }
    }
    public void SetFalseAllboolean()
    {
        isJumping = false;
        isAttack = false;
        isClimb = false;
        isClimbing = false;
        isUnBeat = false;
        isWait = false;
        isStun = false;
        isStunning = false;

        foreach (var i in animator.parameters)
        {
            if (i.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(i.name, false);
            }
            else if (i.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(i.name);
            }
        }
    }
    void ChangeNpcMode()
    {
        DistanceChecking();
        if (isNeutrality)
            return;
        if (target != null && target.activeSelf)
        {
            ChangingAttackMode();
        }
        else
        {
            ChangingNormalMode();
        }

    }
    void SetAttackAnimation()
    {
        animator.SetInteger("weaponType", (int)weaponType);
        float scale = ((this.transform.localScale.x-1) * 0.5f);
        switch (weaponType)
        {
            case WeaponType.No:
                attackRange = 1.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                break;
            case WeaponType.Gun:
                attackRange = 7.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 3);
                break;
            case WeaponType.Sword:
                attackRange = 1.7f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                break;
            case WeaponType.Knife:
                attackRange = 1.2f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                break;
            case WeaponType.Staff:
                attackRange = 5.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                if(isStage)
                    animator.SetBool("isFly", true);
                break;
            case WeaponType.Bow:
                attackRange = 9.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 3);
                break;
            case WeaponType.Heal:
                attackRange = 1.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                break;
            case WeaponType.Support:
                attackRange = 5.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                break;
            case WeaponType.Zombie:
                if(heroData.attackType==0)
                {
                    attackRange = 1.0f + scale;
                    attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                    attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                }
                else
                {
                    attackRange = 7.0f + scale;
                    attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0);
                    attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 3);
                }
                break;
            case WeaponType.Raser:
                attackRange = 3.5f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(2f + attackRange * 2.5f, 3);
                //if (isStage)
                //    animator.SetBool("isFly", true);
                break;
            case WeaponType.DoubleSword:
                attackRange = 1.7f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(1.5f + attackRange * 2f, 3);
                break;
            case WeaponType.Wand:
                attackRange = 5.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f - attackRange, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(2f + attackRange * 2.5f, 3);
                break;
            case WeaponType.Shuriken:
                attackRange = 7.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 3);
                break;
        }
        if (isStatic)
            attackRange = 100f;
        attackMaxRange = UnityEngine.Random.Range(attackRange * 0.8f, attackRange);
    }
    void HealMode()
    {
        RecoveryHp();
        DistanceChecking();
        if (distanceBetweenTarget < attackMaxRange && distanceBetweenTarget >= attackMinRange && TargetAliveCheck(target))
        {
            if(id==109)
            {
                if (target.GetComponent<Hero>().isHealAble())
                {
                    Heal();
                }
                else
                {
                    animator.SetBool("isMoving", false);
                    animator.SetBool("isRun", false);
                }
            }
            else if(id==107)
            {
                Support();
            }

        }
        else
        {
            if (!isAttack && !isUnBeat && !isStunning && !isClimb && !isWait && !isAirborne && !isClimb && !isClimbing && !isSkillAttack)
            {
                ChangeNpcMode();
                Track();
            }
        }
    }
    void AttackMode()
    {
        RecoveryHp();
        DistanceChecking();
        if (distanceBetweenTarget < attackMaxRange && distanceBetweenTarget >= attackMinRange&&target!=null)
        {
            isReached = true;
            Attack();
        }
        else
        {
            isReached = false;
            if (!isAttack && !isUnBeat && !isStunning && !isClimb && !isWait && !isAirborne&&!isClimb&&!isClimbing&&!isSkillAttack&&target!=null)
            {
                Track();
            }
            else
            {
                ChangeNpcMode();
            }
        }
    }
    void RedirectCharacter()
    {
        if (target != null && target.activeSelf && !isStatic)
        {
            isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
        }
        transform.rotation = isLeftorRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }
    void ChangingAttackMode()
    {
        if (heroState != HeroState.Attack)
        {
            //SetFalseAllboolean();
            RedirectCharacter();
            heroState = HeroState.Attack;
            EquipWeapon();
        }
    }
    void ChangingNormalMode()
    {
        if (heroState != HeroState.Normal)
        {
            heroState = HeroState.Normal;
            Idle();
        }

    }
    void StateChecking()
    {
        if(isStage)
        {
            if (isStart&&StageManagement.instance.isStageStart())
            {
                switch (heroState)
                {
                    case HeroState.Normal:
                        Normal();
                        break;
                    case HeroState.Attack:
                        if (!isFriend)
                            AttackMode();
                        else
                            HealMode();
                        break;
                }
            }
        }
        else
        {
            if (isStart)
            {
                switch (heroState)
                {
                    case HeroState.Normal:
                        Normal();
                        break;
                    case HeroState.Attack:
                        if (!isFriend)
                            AttackMode();
                        else
                            HealMode();
                        break;
                }
            }
        }

    }
    void SetSkin(HeroData data)
    {
        //if(id==101)
        //{
        //    var skins = Resources.LoadAll<Sprite>("Skins/001");
        //    foreach(var skin in skins)
        //    {
        //        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
        //        {
        //            if(sprite.sprite.name.Equals(skin.name))
        //            {
        //                sprite.sprite = skin;
        //            }
        //        }
        //    }
        //}
        if(data.over>0)
        {
            TranscendenceEffect();
        }
    }
    void RePositioningHero()
    {
        if(isStage&&isStart)
        {
            if (this.transform.position.x > 16)
            {
                this.transform.position = new Vector3(14, 0, 0);
            }
            else if(this.transform.position.x < - 16)
            {
                this.transform.position = new Vector3(-14, 0, 0);
            }
        }
    }
    public void SetFirstPos(Vector3 pos)
    {
        this.firstPos = pos;
    }
    #endregion

    #region 영웅행동처리
    void Idle()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }
    void Die()
    {
        if (status.hp <= 0 && !isDead)
        {
            if(!isPlayerHero)
                DeathEffect();
            ShadowOnOff(false);
            isDead = true;
            StopAllCoroutines();
            SetFalseAllboolean();
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.gravityScale = 1;
            if(faceAnimator!=null)
                faceAnimator.SetBool("isDead", true);
            rigid.velocity = Vector3.zero;
            Knockback(20, 30);
            attackPoint.SetActive(false);
            animator.SetTrigger("deading");
            if (!isPlayerHero)
            {
                StageManagement.instance.SetDPoint();
                if(Common.stageModeType==Common.StageModeType.Main||Common.stageModeType==Common.StageModeType.Infinite)
                    StartCoroutine("DroppingItem");
                MissionSystem.AddClearPoint(MissionSystem.ClearType.EnemyKill);
                StageManagement.instance.SetKPoint();
                List<GameObject> allyHeros = Common.FindAlly(true);
                if(allyHeros.Count>0)
                {
                    foreach (var hero in allyHeros)
                    {
                        hero.GetComponent<Hero>().ExpUp(Common.GetHeroExp(status.level));
                    }
                }
            }
            else
            {
                if(stageHeroProfileUI!=null&&Common.stageModeType==Common.StageModeType.Main&&StageManagement.instance.stageInfo.stageType==0&&StageManagement.instance.isStageStart())
                {
                    stageHeroProfileUI.GetComponent<UI_StageHeroProfile>().ShowResurrectionTime();
                }
            }
            if(id<1000)
                StartCoroutine("TransparentSprite");
            else
            {
                if(Common.GetSceneCompareTo(Common.SCENE.INFINITE))
                    StartCoroutine("TransparentSprite");
            }
            isStart = false;
        }
    }
    public bool isSkillAble()
    {
        if (!isDead&&!isSkillAttack&&isStart)
        {
            return true;
        }
        return false;
    }
    public void SkillAttack()
    {
        if (isSkillAble())
        {
            if(!isFriend)
                StopHeroAllCoroutines();
            SetFalseAllboolean();
            ShowSkillCastingUI();
            SkillChat(string.Format("{0} !!", SkillSystem.GetSkillName(this.skillData.id)));
            StartCoroutine("SkillAttacking");
        }
    }
    void StopHeroAllCoroutines()
    {
        StopCoroutine("Shoot");
        StopCoroutine("Bowing");
        StopCoroutine("Tracking");
        StopCoroutine("Attacking");
        StopCoroutine("Supporting");
        StopCoroutine("SkillAttacking");
        StopCoroutine("HealAttacking");
    }
    void Heal()
    {
        isTrack = false;
        animator.SetBool("isMoving", false);
        animator.SetBool("isRun", false);

        if (!isAttack && !isUnBeat && !isWait && !isStunning && !isAirborne && !isClimb && !isClimbing && !isSkillAttack)
        {
            StartCoroutine("HealAttacking");
        }
    }
    void Support()
    {
        isTrack = false;
        animator.SetBool("isMoving", false);
        animator.SetBool("isRun", false);

        if (!isAttack && !isUnBeat && !isWait && !isStunning && !isAirborne && !isClimb && !isClimbing && !isSkillAttack)
        {
            StartCoroutine("Supporting");
        }
    }
    void Attack()
    {
        isTrack = false;
        animator.SetBool("isMoving", false);
        animator.SetBool("isRun", false);


        if (!isAttack && !isWait && !isStunning && !isAirborne&&!isClimb&&!isClimbing&&!isSkillAttack)
        {
            StartCoroutine("Attacking");
        }
        if (isAttack && target != null && TargetAliveCheck(target))
        {
            if (distanceBetweenTarget < 0.5f)
            {
                if (this.transform.position.x > target.transform.position.x)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(target.transform.position.x + 0.5f, this.transform.position.y, 0), 0.1f);
                }
                else
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(target.transform.position.x - 0.5f, this.transform.position.y, 0), 0.1f);
                }
            }
        }
    }
    void Track()
    {
        ChangeNpcMode();
        animator.SetBool("isMoving", true);
        animator.SetBool("isRun", true);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (!isTrack && target != null)
        {
            StartCoroutine(Tracking(target.transform.position));
        }

    }
    void Hold()
    {
        if (Vector2.Distance(firstPos, transform.position) > 4)
        {
            isHold = true;
            animator.SetBool("isMoving", true);
            animator.SetBool("isRun", true);
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (!isTrack)
                StartCoroutine("Holding");
        }
        else
        {
            isHold = false;
            IdleAndRun(1.5f);
        }
        ChangeNpcMode();
    }
    void Run()
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (!isStatic)
            isLeftorRight = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
    }
    void Running()
    {
        if (animator.GetBool("isMoving") && !isStunning && !isClimb && !isSkillAttack&&!isStatic)
        {
            ChangeNpcMode();
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            Vector3 moveVelocity = Vector3.zero;
            CheckHurdle();
            RedirectCharacter();
            moveVelocity = isLeftorRight ? Vector3.left : Vector3.right;
            transform.position += moveVelocity * (animator.GetBool("isRun") ? status.moveSpeed * 1f : status.moveSpeed*0.7f) * Time.deltaTime;
        }
    }
    void StopAttack()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isRun", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isSkill", false);
        animator.SetFloat("speed", 1.0f);
        if (!isWait && heroState != HeroState.Normal && !isDead)
            StartCoroutine("WaitingForReady");
    }
    public void SetWait(bool isWaiting)
    {
        isWait = isWaiting;
    }
    void Jump()
    {
        if (isJumping && !isClimb)
        {
            animator.SetBool("isJumping", true);
            animator.SetTrigger("jumping");
            if (faceAnimator != null)
                faceAnimator.SetTrigger("Do");
            rigid.velocity = Vector2.zero;
            JumpEffect();
            Vector2 jumpVelocity = new Vector2(isLeftorRight ? -1 : 1, 20);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

            isJumping = false;
        }

    }
    void Climb()
    {
        if (isClimb)
        {
            if (animator.GetBool("isJumping"))
                animator.SetBool("isJumping", false);
            if (!isClimbing)
            {
                if (rigid.constraints == RigidbodyConstraints2D.FreezeRotation)
                    animator.SetTrigger("climbing");
                animator.SetBool("isClimbReady", true);
                rigid.velocity = new Vector2(0, 0);
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                rigid.bodyType = RigidbodyType2D.Kinematic;

                this.transform.position = Vector2.Lerp(transform.position, new Vector2(pos.x + ((wallPos.x - pos.x) * 0.2f), wallPos.y + 0.5f), 0.1f);
                if (Math.Abs(transform.position.y - (wallPos.y + 0.5f)) < 0.01f)
                {
                    isClimbing = true;
                }
            }
            //벽오르기
            else
            {
                this.transform.position = Vector2.Lerp(transform.position, new Vector2(wallPos.x, wallPos.y + 1f), 0.1f);
                if (Vector2.Distance(this.transform.position, wallPos + new Vector3(0, 1)) < 0.01f)
                    ClimbEnd();
            }
        }
    }
    void ClimbEnd()
    {
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigid.bodyType = RigidbodyType2D.Dynamic;
        isClimb = false;
        isClimbing = false;
        animator.SetBool("isClimbReady", false);
    }

    public IEnumerator Shoot(int count=1)
    {
        count = Mathf.Clamp(count, 1, 10);
        for(int i =0; i<count; i++)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.shootPistol);
            ShootEffect();
            if (target != null && ObjectPool.Instance != null)
            {
                GameObject bullet = ObjectPool.Instance.PopFromPool("Bullet");
                bullet.GetComponent<bulletController>().penetrateCnt = 2;
                bullet.GetComponent<bulletController>().damage = Damage();
                bullet.GetComponent<bulletController>().pent = this.status.penetration;
                bullet.GetComponent<bulletController>().isAlly = this.isPlayerHero;
                bullet.GetComponent<bulletController>().Target = target.transform;
                bullet.GetComponent<bulletController>().isCritical = isCriticalAttack;
                bullet.transform.position = transform.GetChild(0).position + new Vector3(0, 0.1f) + transform.right * -0.5f;
                bullet.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                bullet.SetActive(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }

    public void Throwing()
    {
        if (target != null && ObjectPool.Instance != null)
        {
            GameObject knife = ObjectPool.Instance.PopFromPool("knife(throw)");
            knife.GetComponent<bulletController>().damage = Damage();
            knife.GetComponent<bulletController>().pent = this.status.penetration;
            knife.GetComponent<bulletController>().isAlly = this.isPlayerHero;
            knife.GetComponent<bulletController>().Target = target.transform;
            knife.GetComponent<bulletController>().isCritical = isCriticalAttack;
            knife.transform.position = transform.GetChild(0).position + new Vector3(0, 0.1f) + transform.right * -1f;
            knife.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            knife.SetActive(true);
        }
    }
    public IEnumerator Bowing(int count=1)
    {
        count = Mathf.Clamp(count, 1, 10);
        for (int i = 0; i < count; i++)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.bow);
            if (target != null && ObjectPool.Instance != null)
            {
                GameObject arrow = ObjectPool.Instance.PopFromPool("Arrow");

                arrow.GetComponent<arrowController>().damage = Damage();
                arrow.GetComponent<arrowController>().pent = this.status.penetration;
                arrow.GetComponent<arrowController>().isAlly = this.isPlayerHero;
                arrow.GetComponent<arrowController>().target = target.transform;
                arrow.GetComponent<arrowController>().isCritical = isCriticalAttack;
                arrow.transform.position = transform.position + new Vector3(0, 0.1f) + transform.right * -1f;
                arrow.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                arrow.SetActive(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
    public IEnumerator ThrowingShuriken(int count = 1)
    {
        count = Mathf.Clamp(count, 1, 10);
        for (int i = 0; i < count; i++)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.bow);
            if (target != null && ObjectPool.Instance != null)
            {
                GameObject arrow = ObjectPool.Instance.PopFromPool("Shuriken");

                arrow.GetComponent<arrowController>().damage = Damage();
                arrow.GetComponent<arrowController>().pent = this.status.penetration;
                arrow.GetComponent<arrowController>().isAlly = this.isPlayerHero;
                arrow.GetComponent<arrowController>().target = target.transform;
                arrow.GetComponent<arrowController>().isCritical = isCriticalAttack;
                arrow.transform.position = transform.position + new Vector3(0, 0.1f) + transform.right * -1f;
                arrow.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                arrow.SetActive(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
    void Normal()
    {
        ChangeNpcMode();
        if(isStage)
        {
            if (stageModeType == (int)Common.SCENE.INFINITE)
                Hold();
            else
                Idle();
        }
        else
        {
            IdleAndRun(UnityEngine.Random.Range(3.0f, 5.0f));
        }
    }
    void IdleAndRun(float time)
    {
        animationTime += Time.unscaledDeltaTime;
        if (animationTime >= standardTime)
        {
            StopAttack();
            animationTime = 0;
            standardTime = time;
            randomStatus = UnityEngine.Random.Range(0, 3);
            if (randomStatus <= 1)
                Idle();
            else
                Run();
        }
    }
    void Knockback(float kx, float ky, Collider2D collision = null)
    {
        Vector2 attackedVelocity = Vector2.zero;
        kx -= status.knockbackResist;
        ky -= status.knockbackResist;
        kx = kx < 0 ? 0 : kx;
        ky = ky < 0 ? 0 : ky;
        if (collision != null)
        {

            if (collision.transform.position.x > transform.position.x)
            {
                if (this.transform.rotation.y == 0)
                    animator.SetBool("isFrontHit", false);
                else
                    animator.SetBool("isFrontHit", true);

                attackedVelocity = new Vector2(-kx, ky);
            }
            else
            {
                if (this.transform.rotation.y == 0)
                    animator.SetBool("isFrontHit", true);
                else
                    animator.SetBool("isFrontHit", false);

                attackedVelocity = new Vector2(kx, ky);
            }
            rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
        }
        else
        {
            if (target != null)
                attackedVelocity = target.transform.position.x > transform.position.x ? new Vector2(-kx, ky) : new Vector2(kx, ky);
            rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
            if ((rigid.velocity.x > 0 && isLeftorRight) || (rigid.velocity.x < 0 && !isLeftorRight))
                animator.SetBool("isFrontHit", true);
            else
                animator.SetBool("isFrontHit", false);

        }

    }
    void Defence(Collider2D collision)
    {
        isUnBeat = true;
        isDefence = true;
        ChangingAttackMode();
        GuardEffect(collision);
        if (!animator.GetBool("isAttack")&&!isSkillAttack)
        {
            animator.SetTrigger("defencing");
            if (faceAnimator != null)
                faceAnimator.SetTrigger("Do");
            Knockback(1, 1, collision);
        }
        DamageCCUIShow(LocalizationManager.GetText("Defence"));

        if (target != null)
        {
            if (Vector3.Distance(Common.GetBottomPosition(collision.transform.parent), Common.GetBottomPosition(this.transform)) < distanceBetweenTarget && collision.GetComponentInParent<Hero>() != null && collision.GetComponentInParent<Hero>().isPlayerHero != isPlayerHero)
            {
                target = collision.GetComponentInParent<Hero>().gameObject;
            }
        }
        StartCoroutine(UnBeatTime(0));

        isWait = false;
        animator.SetBool("isWait", false);
    }
    public void Hitted(Collider2D collision, int dam, float kx, float ky, float pentration = 0.0f)
    {
        if(!isDead && status.hp > 0)
        {
            bool isCritical = collision.GetComponentInParent<Hero>().isCriticalAttack;
            if (isCritical)
                HitCriticalEffect();
            else
                HitEffect();
            dam = isPvpSetting&&isPlayerHero ? Common.GetPvpDamage(dam,GetDefence(pentration),StageBattleManager.instance.GetHeroMaxDamageLevel()):  Common.GetDamage(dam, GetDefence(pentration));
            Common.isHitShake = true;
            ChangingAttackMode();
            isUnBeat = true;
            if (collision.GetComponentInParent<Hero>() != null)
            {
                DamageUIShow(dam.ToString(), isCritical);
                if (collision.GetComponentInParent<Hero>() != null && collision.GetComponentInParent<Hero>().isPlayerHero != isPlayerHero&&!isFriend)
                {
                    if(target==Common.hitTargetObject)
                    {
                        target = collision.GetComponentInParent<Hero>().gameObject;
                    }
                    else if (Common.GetDistanceBetweenAnother(this.transform, collision.transform.parent) < distanceBetweenTarget)
                    {
                        target = collision.GetComponentInParent<Hero>().gameObject;
                    }
                }
            }
            if (isStunning)
            {
                Knockback(0, ky, collision);
            }
            else
            {
                Knockback(kx, ky, collision);
                if (!animator.GetBool("isAttack") && !isSkillAttack)
                    animator.SetTrigger("heating");
            }
            if (faceAnimator != null)
                faceAnimator.SetTrigger("Hit");
            status.hp = Common.looMinus(status.hp, dam);
            StartCoroutine(UnBeatTime(dam));
        }
    }
    public void HittedByObject(int dam,bool isCritical, Vector2 addforce, float pent=0.0f)
    {
        if(!isDead&&status.hp>0)
        {
            if (isCritical)
                HitCriticalEffect();
            else
                HitEffect();
            dam = isPvpSetting && isPlayerHero ? Common.GetPvpDamage(dam, GetDefence(pent), StageBattleManager.instance.GetHeroMaxDamageLevel()) : Common.GetDamage(dam, GetDefence(pent));

            Common.isHitShake = true;
            ChangingAttackMode();
            isUnBeat = true;
            DamageUIShow(dam.ToString(), isCritical);
            if (!isStunning)
            {
                if (!animator.GetBool("isAttack") && !isSkillAttack)
                    animator.SetTrigger("heating");
            }
            if (faceAnimator != null)
                faceAnimator.SetTrigger("Hit");
            status.hp = Common.looMinus(status.hp, dam);
            Vector2 attackedVelocity = isLeftorRight ? new Vector2(addforce.x, addforce.y) : new Vector2(-addforce.x, addforce.y);
            rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
            StartCoroutine(UnBeatTime(dam));
        }
    }
    public void HittedPersistent(int dam, int time)
    {
        if (!isPersistentHitted)
            StartCoroutine(HittedPersistenting(dam, time));
    }
    public IEnumerator HittedPersistenting(int dam, int time)
    {
        if (!isDead && status.hp > 0)
        {
            isPersistentHitted = true;
            dam = dam / time;

            float t = 0.0f;
            while(t<=time)
            {
                status.hp = Common.looMinus(status.hp, dam);
                DamageFixedUIShow("-" + dam.ToString());
                ShowHpBar(dam);
                yield return new WaitForSeconds(1.0f);
                t += 1.0f;
            }
            isPersistentHitted = false;
            yield return null;
        }
        isPersistentHitted = false; 
        yield return null;
    }
    public void Healing(int healAmount=0)
    {
        HealEffect();
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.heal);
        int maxHeal = status.maxHp - status.hp;
        healAmount = Mathf.Clamp(healAmount, 0, maxHeal);
        this.status.hp = Common.looHpPlus(this.status.hp, this.status.maxHp, healAmount);
        DamageCCUIShow("+" + healAmount.ToString());
        ShowHpBar(-healAmount);
    }
    public void Stunned(float time=1.0f,bool isForce=false)
    {
        if (!isStunning&&status.knockbackResist<10|| isForce)
        {
            isStun = true;
            DamageCCUIShow(LocalizationManager.GetText("Stun"));
            StunEffect();
            StartCoroutine(Stunning(time));
        }
    }
    void EquipWeapon()
    {
        if (weaponPoint != null && weaponPoint.transform.childCount > 0 && !weaponPoint.transform.GetChild(0).gameObject.activeSelf)
        {
            animator.SetTrigger("changingWeapon");
            weaponPoint.transform.GetChild(0).gameObject.SetActive(true);
        }
        if (weaponPoint2 != null && weaponPoint2.transform.childCount > 0 && !weaponPoint2.transform.GetChild(0).gameObject.activeSelf)
        {
            weaponPoint2.transform.GetChild(0).gameObject.SetActive(true);
        }
        SetAttackAnimation();
    }
    IEnumerator DroppingItem()
    {
        // 적 영웅 코인획득 파트
        if(!isPlayerHero)
        {
            // 하트
            if(UnityEngine.Random.Range(0,10)<1)
            {
                var findTarget = Common.FindAlly(true);
                if(findTarget!=null&&findTarget.Count>0)
                {
                    GameObject heartPrefab = ObjectPool.Instance.PopFromPool("Heart");
                    heartPrefab.GetComponent<Heart>().SetHeart(10);
                    findTarget.Sort((t1, t2) => t1.GetComponent<Hero>().status.hp.CompareTo(t2.GetComponent<Hero>().status.hp));
                    heartPrefab.GetComponent<Heart>().MagnetTarget = findTarget[0];
                    heartPrefab.transform.position = this.transform.position;
                    heartPrefab.SetActive(true);
                    heartPrefab.GetComponent<Rigidbody2D>().AddForce(new Vector3(UnityEngine.Random.Range(-1, 1), 5, 10), ForceMode2D.Impulse);
                }
            }
            yield return new WaitForSeconds(0.1f);
            // 코인
            if(id>1000)
            {
                int bossCoinCount = UnityEngine.Random.Range(3, 7);
                if (Common.GetSceneCompareTo(Common.SCENE.INFINITE))
                    bossCoinCount = 1;
                for (var i = 0; i < bossCoinCount; i++)
                {
                    int coin = Common.GetMonsterCoin(status.level);
                    GameObject coinPrefab = ObjectPool.Instance.PopFromPool("Coin");
                    coinPrefab.GetComponent<Coin>().SetCoin(coin);
                    coinPrefab.transform.position = transform.position;
                    coinPrefab.SetActive(true);
                    coinPrefab.GetComponent<Rigidbody2D>().AddForce(new Vector3(UnityEngine.Random.Range(-1, 1), 5, 10), ForceMode2D.Impulse);
                    MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalCoinDropCount);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                int coin = Common.GetMonsterCoin(status.level);
                GameObject coinPrefab = ObjectPool.Instance.PopFromPool("Coin");
                coinPrefab.GetComponent<Coin>().SetCoin(coin);
                coinPrefab.transform.position = transform.position;
                coinPrefab.SetActive(true);
                coinPrefab.GetComponent<Rigidbody2D>().AddForce(new Vector3(UnityEngine.Random.Range(-1, 1), 5, 10), ForceMode2D.Impulse);
                MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalCoinDropCount);
                yield return new WaitForSeconds(0.1f);
            }
        }
        // 아이템 획득 파트
        if (UnityEngine.Random.Range(0, 15) < StageManagement.instance.stageInfo.stageNumber+3)
        {
            Item randomItem = ItemSystem.GetRandomItem();
            if (UnityEngine.Random.Range(0, 1000) < ItemSystem.GetDroprate(randomItem) && StageManagement.instance.stageInfo.stageGetItems.Count < 5)
            {
                GameObject dropItem = ObjectPool.Instance.PopFromPool("dropItemPrefab");
                dropItem.transform.position = transform.position;
                dropItem.GetComponent<dropItemInfo>().dropItemID = randomItem.id;
                dropItem.SetActive(true);
                dropItem.GetComponent<dropItemInfo>().DropItem();
                StageManagement.instance.AddGetStageItem(randomItem.id);
                MissionSystem.AddClearPoint(MissionSystem.ClearType.TotalItemDropCount);
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return null;
    }
    #endregion

    #region 영웅버프
    public void AttackBuff(float buffTime,float buffPower)
    {
        if(!isAttackBuff)
        {
            StartCoroutine(AttackBuffing(buffTime, buffPower));
        }
    }
    public void DefenceBuff(float buffTime, float buffPower)
    {
        if(!isDefenceBuff)
        {
            StartCoroutine(DefenceBuffing(buffTime, buffPower));
        }
    }
    public void WaveBuff(int wave)
    {
        if (!isPlayerHero&&!isWaveBuff)
        {
            StartCoroutine(WaveBuffing(wave));
        }
    }
    IEnumerator WaveBuffing(int wave)
    {
        isWaveBuff = true;
        this.status.attack += (int)(wave * 0.1f * this.status.attack);
        this.status.defence += (int)(wave * 0.1f * this.status.defence);
        this.status.maxHp += (int)(wave * 0.1f * this.status.maxHp);
        this.status.hp = this.status.maxHp;
        yield return null;
    }
    void BossModeDifficultyBuff()
    {
        if(this.id>1000&&Common.stageModeType==Common.StageModeType.Boss)
        {
            if(Common.bossModeDifficulty==1)
            {
                this.status.attack += (int)(2 * this.status.attack);
                this.status.defence += (int)(1 * this.status.defence);
                this.status.maxHp += (int)(9 * this.status.maxHp);
                this.status.hp = this.status.maxHp;
            }
            else
            {

            }
            Debugging.Log("보스난이도 세팅");
        }
    }
    IEnumerator AttackBuffing(float buffTime,float buffPower)
    {
        isAttackBuff = true;
        BuffEffectAttack();
        int initAttack = this.status.attack;
        float time = 0.0f;
        this.status.attack += (int)(initAttack * buffPower);
        if(buffPower >= 0)
            DamageCCUIShow(LocalizationManager.GetText("AttackBuffUp"));
        else
            DamageCCUIShow(LocalizationManager.GetText("AttackBuffDown"));

        while (time < buffTime)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isAttackBuff = false;
        this.status.attack = initAttack;
        Debugging.Log(this.HeroName + " 의 공격력 원상태로"  + this.status.attack);
        yield return null;
    }
    IEnumerator DefenceBuffing(float buffTime, float buffPower)
    {
        buffPower = Mathf.Clamp(buffPower, -0.8f, 0.8f);
        isDefenceBuff = true;
        BuffEffectAttack();
        int initDefence = this.status.defence;
        float time = 0.0f;
        this.status.defence += (int)(initDefence * buffPower);
        if (buffPower >= 0)
            DamageCCUIShow(LocalizationManager.GetText("DefenceBuffUp"));
        else
            DamageCCUIShow(LocalizationManager.GetText("DefenceBuffDown"));
        while (time < buffTime)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isDefenceBuff = false;
        this.status.defence = initDefence;
        Debugging.Log(this.HeroName + " 의 방어력 원상태로" + this.status.defence);
        yield return null;
    }
    #endregion

    #region 공통기능
    public void Chat(string chat)
    {
        Common.Chat(chat, transform.GetChild(0).transform);
    }
    public void SkillChat(string chat)
    {
        Common.SkillChat(chat, transform.GetChild(0).transform);
    }
    #endregion

    #region 이펙트
    public void StunEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.stoneCrack);
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("StunEffect");
            if(effect!=null)
            {
                effect.transform.position = transform.GetChild(0).position;
                effect.SetActive(true);
            }
        }
    }
    public void PunchEffect(float distance)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Punch_Hit");
            effect.transform.position = transform.position + new Vector3(isLeftorRight ? -distance : distance, 0, 0);
            effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            effect.SetActive(true);
        }
    }
    public void SpawnEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.teleport);
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SimplePortalRed");
            effect.transform.position = transform.GetChild(0).position;
            effect.transform.localScale = Vector3.one*1.5f;
            effect.SetActive(true);
        }
    }
    public void LevelUpEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.levelup);
        GameObject effect = EffectPool.Instance.PopFromPool("LevelUpEffect");
        effect.transform.position = transform.GetChild(0).position;
        effect.SetActive(true);
    }
    public void MagicEffect(int effectNumber=0)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = null;
            switch(effectNumber)
            {
                case 0:
                    effect = EffectPool.Instance.PopFromPool("MagicCircleYellow");
                    break;
                case 1:
                    effect = EffectPool.Instance.PopFromPool("MagicCircleBlue");
                    break;
            }
            if(effect!=null)
            {
                effect.transform.position = transform.GetChild(0).position + new Vector3(isLeftorRight ? -0.2f : 0.2f, 0, 0);
                effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                effect.transform.localScale = new Vector3(1, 1, 1);
                effect.SetActive(true);
            }
        }
    }
    public void MagicBigEffect(int effectNumber = 0)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("MagicCircleYellow");
            if (effect != null)
            {
                effect.transform.position = transform.GetChild(0).position + new Vector3(isLeftorRight ? -0.2f : 0.2f, 0, 0);
                effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                effect.transform.localScale = new Vector3(2, 2, 2);
                effect.SetActive(true);
            }
        }
    }
    public void MagicSpawnEffect(Vector3 pos)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("MagicCircleYellow");
            if (effect != null)
            {
                effect.transform.position = pos;
                float r = UnityEngine.Random.Range(1f, 1.5f);
                effect.transform.localScale = new Vector3(r, r, r);
                effect.SetActive(true);
            }
        }
    }
    public void RunEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Run_Smoke");
            effect.transform.position = transform.position + new Vector3(isLeftorRight ? 0.3f : -0.3f, -0.7f, 0);
            effect.SetActive(true);
        }
    }
    private void JumpEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject jumpEffect = EffectPool.Instance.PopFromPool("Jump_Smoke");
            jumpEffect.transform.position = transform.position + new Vector3(0, -0.7f);
            jumpEffect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            jumpEffect.SetActive(true);
        }
    }
    public void GroundEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("KnockBack_Smoke");
            effect.transform.position = transform.position + new Vector3(0, -0.8f);
            effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            effect.SetActive(true);
        }
    }
    public void HitEffect(Collider2D collider=null)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Hit_white_Small");
            effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0.2f, 0);
            effect.SetActive(true);
        }
    }
    public void HitCriticalEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoftFightAction");
            effect.transform.position = transform.GetChild(0).position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0.2f, 0);
            effect.SetActive(true);
        }
    }
    public void DeathEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("GenericDeath");
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
        }
    }
    public void SlashHitEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SwordHitBlue");
            effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0.2f, 0);
            effect.SetActive(true);
        }
    }
    public void SlashEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SwordSlashBlue");
            var effectPos = this.transform.position;
            effectPos.z = 100;
            effect.transform.localScale = new Vector3(2, 2, 2);
            effect.transform.position = effectPos;
            effect.transform.rotation = isLeftorRight ? Quaternion.Euler(-100, 180, 0) : Quaternion.Euler(-100, 0, 0);
            effect.SetActive(true);
        }
    }
    private void SwordHitEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Sword_Hit");
            effect.transform.position = transform.position;
            effect.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-0.2f, 0.2f), 0, UnityEngine.Random.Range(-10, 10));
            effect.SetActive(true);
        }
    }
    public void GuardEffect(Collider2D collider)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Guard_hit");
            effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0.2f, 0);
            effect.SetActive(true);
        }
    }
    public void ShootEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Gun_Shoot");
            effect.transform.position = transform.position + new Vector3(0, 0.2f, 0) + transform.right * -1f;
            effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            effect.SetActive(true);
        }
    }
    private void SwingEffect(float z)
    {
        if(EffectPool.Instance!=null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Sword_Cut_Medium");
            effect.transform.position = transform.position + new Vector3(isLeftorRight ? -1f : 1f, 0.2f, 0);
            if (z == 0)
                z = 90;
            effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 90, z) : Quaternion.Euler(0, 270, z);
            effect.SetActive(true);
        }

    }
    public void SwingEffect2()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Sword_Thrust_Small");
            effect.transform.position = transform.position + new Vector3(isLeftorRight ? -0.5f : 0.5f, -0.5f, 0);
            effect.transform.rotation = isLeftorRight ? Quaternion.Euler(120, 225, 90) : Quaternion.Euler(120, 45, 90);
            effect.SetActive(true);
        }
    }
    public void MagicCircleEffect(Vector3 pos, float size = 1)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("fx_Magiccircle_m");
            effect.transform.position = pos;
            effect.transform.localScale = new Vector3(size, size, size);
            effect.SetActive(true);
        }
    }
    void BoomEffect()
    {
        GameObject effect = EffectPool.Instance.PopFromPool("ExplosionRoundFire");
        effect.transform.position = transform.position;
        effect.SetActive(true);
    }
    public void SmokeDarkEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.smoke);
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SmokeExplosionDark");
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
        }

    }
    public void SlamEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.fireShot);
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoftBodySlam");
            effect.transform.position = transform.GetChild(0).position + new Vector3(isLeftorRight ? -1 : 1, 0, 0);
            effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 110, -90) : Quaternion.Euler(0, -110, -90);
            effect.SetActive(true);
        }
    }
    GameObject musicEffect;
    public void MusicEffect()
    {
        if (EffectPool.Instance != null&&musicEffect==null)
        {
            musicEffect = EffectPool.Instance.PopFromPool("MusicalNotes");
        }
        if(musicEffect!=null)
        {
            musicEffect.transform.position = transform.GetChild(0).position;
            musicEffect.SetActive(true);
        }
    }
    public void MusicEffectOff()
    {
        if (EffectPool.Instance != null && musicEffect != null)
        {
            EffectPool.Instance.PushToPool("MusicalNotes", musicEffect);
        }
    }
    public void BuffEffectSpeed()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoulMuzzleGreen");
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
        }

    }
    public void BuffEffectAttack()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoulMuzzlePurple");
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
        }

    }
    public void BuffEffectDefence()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoulMuzzlePurple");
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
        }

    }
    public void RaserEffect(float distance)
    {
        if (EffectPool.Instance != null)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.fireShot);
            GameObject effect = isPlayerHero ? EffectPool.Instance.PopFromPool("TallExplosionBlue") : EffectPool.Instance.PopFromPool("TallExplosionFire");
            effect.transform.position = transform.position + new Vector3(isLeftorRight ? -distance : distance, 0, 0);
            if(target!=null)
            {
                effect.transform.rotation = IsTargetLeft(target.transform) ? Quaternion.Euler(-180, 90, 0) : Quaternion.Euler(-180, -90, 0);
            }
            else
            {
                effect.transform.rotation = isLeftorRight ? Quaternion.Euler(-180, 90, 0) : Quaternion.Euler(-180, -90, 0);
            }
            effect.SetActive(true);
        }
    }
    public void HealEffect()
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("SoulMuzzleOrange");
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
        }
    }
    public void TranscendenceEffect()
    {
        if (EffectPool.Instance != null&& transform!=null)
        {
            transcendenceEffect = EffectPool.Instance.PopFromPool("GlowOrbPink");
            if(transcendenceEffect!=null)
            {
                transcendenceEffect.transform.SetParent(transform.GetChild(0));
                transcendenceEffect.transform.position = transform.GetChild(0).position;
                transcendenceEffect.SetActive(true);
            }
        }
    }
    public void FireEffect()
    {
        StartCoroutine("FireEffecting");
    }
    public IEnumerator FireEffecting()
    {
        GameObject effect = EffectPool.Instance.PopFromPool("SoftFireABRed");
        if(effect!=null)
        {
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
            while (effect.activeSelf && !this.isDead)
            {
                effect.transform.position = transform.GetChild(0).position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f), 0);
                yield return new WaitForSeconds(0.2f);
            }
            EffectPool.Instance.PushToPool("SoftFireABRed", effect);
        }
        yield return null;
    }
    #endregion

    #region 물리처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
            return;
        if(!isDead&&!isUnBeat)
        {
            if (collision.gameObject.CompareTag("Boom"))
            {
                Hitted(collision, status.maxHp, 25f, 15f);
            }
            if (collision.gameObject.CompareTag("bullet") && collision.GetComponent<bulletController>() != null && collision.GetComponent<bulletController>().isAlly != isPlayerHero)
            {
                collision.GetComponent<bulletController>().BulletStand(this.transform.GetChild(0).transform);
            }
            if (collision.gameObject.CompareTag("arrow") && collision.GetComponent<arrowController>() != null && collision.GetComponent<arrowController>().isAlly != isPlayerHero && !collision.GetComponent<arrowController>().isStand)
            {
                collision.GetComponent<arrowController>().ArrowStand(this.transform.GetChild(0).transform);
                //collision.GetComponent<SpriteRenderer>().sortingOrder = (GetSpriteLayer("body")+2);
            }
            if (collision.gameObject.layer == 9 && collision.isTrigger && collision.gameObject != attackPoint.gameObject && collision.GetComponentInParent<Hero>() != null && collision.GetComponentInParent<Hero>().isPlayerHero != isPlayerHero && collision.GetComponentInParent<Hero>().target != null && collision.GetComponentInParent<Hero>().target.gameObject.GetInstanceID() == this.gameObject.GetInstanceID())
            {
                if (UnityEngine.Random.Range(0, 1000) <= Mathf.Clamp(status.defence*0.1f,0,200)+OnReturnHeroAbility(8) && !isStunning && !isAirborne)
                {
                    Defence(collision);
                }
                else
                {
                    Hitted(collision, DamageDataDam(collision.name), 4f, 2f, DamageDataPent(collision.name));
                }
            }
        }

        // 벽
        if (collision.CompareTag("Wall") && !isClimb && !isClimbing)
        {
            wallPos = collision.transform.position;
            pos = transform.position;
            if (wallPos.y > transform.position.y)
            {
                if (collision.name.Contains("left"))
                {
                    if (transform.rotation.y != 0)
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    if (transform.rotation.y != -1)
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                isClimb = true;
                rigid.gravityScale = 0;
            }
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead)
            return;

        if (collision.CompareTag("obstacle") && (Math.Abs(collision.GetComponent<Rigidbody2D>().angularVelocity) > 120 || Math.Abs(collision.GetComponent<Rigidbody2D>().velocity.y) > 2 || collision.transform.position.y > transform.position.y + 0.5f))
        {
            Hitted(collision, (int)collision.GetComponent<Rigidbody2D>().mass, 0f, 0f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.layer == 0)
        {
            GetComponent<Rigidbody2D>().simulated = true;
        }
        if (collision.CompareTag("Wall"))
        {
            rigid.gravityScale = 1;
        }
    }
    #endregion

    #region 코루틴
    IEnumerator UnBeatTime(int dam)
    {
        ShowHpBar(dam);
        Sound_Damage();
        while (isUnBeat)
        {
            yield return new WaitForFixedUpdate();
            isUnBeat = false;
        }
        isUnBeat = false;
        if (status.hp > 0 && !isStunning)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        else
            yield return null;
        if (isDefence)
            isDefence = false;
        yield return null;
    }
    IEnumerator StartAttackMode()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
        if(isPlayerHero)
            SpawnEffect();
        RedirectCharacter();
        yield return new WaitForSeconds(3);
        EquipWeapon();
        SpriteAlphaSetting(1);
        OpenHpBar(isPlayerHero);
        yield return new WaitForSeconds(1);
        if (initChats.Count > 0)
            Common.Chat(initChats[UnityEngine.Random.Range(0, initChats.Count)], transform.GetChild(0),UnityEngine.Random.Range(0,3));
        yield return new WaitForSeconds(1);
        InitEnemys();
        heroState = HeroState.Attack;
        isStart = true;
        yield return new WaitForSeconds(2.0f);
        FindEnemys(this.isPlayerHero);
        StartCoroutine("FindAllys");
        yield return null;
    }
    IEnumerator StartMonsterAttackMode()
    {
        RedirectCharacter();
        OpenHpBar(isPlayerHero);
        yield return new WaitForSeconds(0.5f);
        EquipWeapon();
        if (initChats.Count > 0 && UnityEngine.Random.Range(0, 3) < 1)
            Common.Chat(initChats[UnityEngine.Random.Range(0, initChats.Count)], transform.GetChild(0));
        yield return new WaitForSeconds(1);
        InitEnemys();
        heroState = HeroState.Attack;
        isStart = true;
        FindEnemys(true);
        yield return null;
    }
    IEnumerator StartPvpMode()
    {
        while(!StageBattleManager.instance.isStartGame)
        {
            yield return null;
        }
        RedirectCharacter();
        yield return new WaitForSeconds(3);
        EquipWeapon();
        SpriteAlphaSetting(1);
        OpenHpBar(isPlayerHero);
        yield return new WaitForSeconds(2);
        InitEnemys();
        heroState = HeroState.Attack;
        isStart = true;
        yield return new WaitForSeconds(2.0f);
        FindEnemys(this.isPlayerHero);
        StartCoroutine("FindAllys");
        yield return null;
    }
    IEnumerator OnAttackPoint(int totalCount=1)
    {
        attackPoint.name = this.SetDamageData(this.Damage(), this.status.penetration);
        totalCount = Mathf.Clamp(totalCount, 1, 10);
        int cnt = 0;
        while(cnt<totalCount&&!isFriend)
        {
            attackPoint.gameObject.SetActive(true);
            if (attackPoint && attackPoint.GetComponent<AudioSource>() != null)
            {
                attackPoint.GetComponent<AudioSource>().Play();
            }
            yield return new WaitForFixedUpdate();
            attackPoint.gameObject.SetActive(false);
            cnt++;
        }
        yield return null;
    }
    IEnumerator OnMagicPoint(int magicType=0)
    {
        if (target != null)
        {
            if(id==103||id==511)
            {
                switch (magicType)
                {
                    case 0:
                        GameObject magic = ObjectPool.Instance.PopFromPool("MagicStone");
                        magic.transform.position = target.transform.position + new Vector3(0, 5);
                        MagicCircleEffect(magic.transform.position + new Vector3(0, 1), 3);
                        if (magic.GetComponent<DecompositionObject>() != null)
                        {
                            magic.GetComponent<DecompositionObject>().TargetTag = isPlayerHero ? "Enemy" : "Hero";
                            magic.GetComponent<DecompositionObject>().Damage = Damage();
                            magic.GetComponent<DecompositionObject>().pent = this.status.penetration;
                            magic.GetComponent<DecompositionObject>().isCritical = isCriticalAttack;
                            magic.GetComponent<DecompositionObject>().isRolling = true;
                        }
                        magic.SetActive(true);
                        break;
                    case 1:
                        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.smoke);
                        List<GameObject> targetList = enemys;
                        targetList = targetList.FindAll(x => Common.GetDistanceBetweenAnother(this.transform, x.transform) < 5);
                        if (targetList.Count > 0)
                        {
                            for (int i = 0; i < targetList.Count; i++)
                            {
                                yield return new WaitForEndOfFrame();
                                if (i > 6)
                                    break;
                                if (TargetAliveCheck(targetList[i]))
                                {
                                    GameObject skillEffect = EffectPool.Instance.PopFromPool("LightningStrikeTall", this.transform);
                                    skillEffect.transform.position = targetList[i].transform.position;
                                    skillEffect.SetActive(true);
                                    skillEffect.GetComponent<ParticleSystem>().Play();
                                    targetList[i].GetComponent<Hero>().HittedByObject((Damage() * 2 / targetList.Count), isCriticalAttack, new Vector2(5f, 5f), this.status.penetration);
                                }
                            }
                        }
                        else if (Common.hitTargetObject != null && isPlayerHero)
                        {
                            yield return new WaitForEndOfFrame();
                            GameObject skillEffect = EffectPool.Instance.PopFromPool("LightningStrikeTall", this.transform);
                            skillEffect.transform.position = Common.hitTargetObject.transform.position;
                            skillEffect.SetActive(true);
                            skillEffect.GetComponent<ParticleSystem>().Play();
                            if (Common.hitTargetObject.GetComponent<Castle>() != null)
                                Common.hitTargetObject.GetComponent<Castle>().HittedByObject((Damage()), isCriticalAttack, new Vector2(5f, 5f));
                        }
                        else if (Common.allyTargetObject != null && !isPlayerHero)
                        {
                            yield return new WaitForEndOfFrame();
                            GameObject skillEffect = EffectPool.Instance.PopFromPool("LightningStrikeTall", this.transform);
                            skillEffect.transform.position = Common.hitTargetObject.transform.position;
                            skillEffect.SetActive(true);
                            skillEffect.GetComponent<ParticleSystem>().Play();
                            if (Common.hitTargetObject.GetComponent<Castle>() != null)
                                Common.allyTargetObject.GetComponent<Castle>().HittedByObject((Damage()), isCriticalAttack, new Vector2(5f, 5f));
                        }
                        break;
                }
            }
            else if(id==524)
            {
                Vector3 pos = transform.GetChild(0).position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-1f, 1f));
                MagicSpawnEffect(pos);
                yield return new WaitForSeconds(0.2f);
                if (target != null)
                {
                    GameObject bullet = ObjectPool.Instance.PopFromPool("FlamethrowerCartoonyFire");
                    bullet.transform.position = pos;
                    bullet.transform.rotation = IsTargetLeft(target.transform) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                    bullet.GetComponent<bulletController>().damage = this.Damage();
                    bullet.GetComponent<bulletController>().pent = this.status.penetration;
                    bullet.GetComponent<bulletController>().isAlly = this.isPlayerHero;
                    bullet.GetComponent<bulletController>().Target = target.transform;
                    bullet.GetComponent<bulletController>().isCritical = isCriticalAttack;
                    bullet.SetActive(true);
                }
            }
            else if(id==1008)
            {
                List<GameObject> targetList = Common.FindEnemy(isPlayerHero);
                if(targetList!=null&&targetList.Count>0)
                for(var i = 0; i < targetList.Count; i++)
                {
                        GameObject magic = ObjectPool.Instance.PopFromPool("MagicSpier");
                        magic.transform.position = targetList[i].transform.position + new Vector3(0, 5);
                        MagicCircleEffect(magic.transform.position + new Vector3(0, 1), 3);
                        if (magic.GetComponent<DecompositionObject>() != null)
                        {
                            magic.GetComponent<DecompositionObject>().TargetTag = isPlayerHero ? "Enemy" : "Hero";
                            magic.GetComponent<DecompositionObject>().Damage = Damage();
                            magic.GetComponent<DecompositionObject>().pent = this.status.penetration;
                            magic.GetComponent<DecompositionObject>().isCritical = isCriticalAttack;
                            magic.GetComponent<DecompositionObject>().isRolling = false;
                        }
                        magic.SetActive(true);
                    }
            }
        }
        yield return null;
    }
    IEnumerator WaitingForReady()
    {
        int ct = 0;
        isWait = true;
        animator.SetTrigger("waiting");
        animator.SetBool("isWait", isWait);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        while (ct < 1)
        {
            yield return new WaitForSeconds(1.2f-(Mathf.Clamp(this.status.attackSpeed,0,1) *1.2f));
            if (isAttack)
                isAttack = false;
            ct++;
        }
        isWait = false;
        animator.SetBool("isWait", isWait);
    }
    IEnumerator TransparentSprite()
    {
        yield return new WaitForSeconds(2.0f);
        var sprites = GetComponentsInChildren<SpriteRenderer>();

        float alpha = 1.0f;
        while (alpha >= 0)
        {
            foreach (var a in sprites)
            {
                if (!a.name.Contains("hair"))
                    a.color = new Color(1, 1, 1, alpha);
            }

            alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var a in sprites)
            a.color = new Color(1, 1, 1, 0);
        this.gameObject.SetActive(false);
        yield return null;

    }
    IEnumerator Airborne()
    {
        isAirborne = true;
        isAir = false;
        rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
        while (isAirborne)
        {
            yield return new WaitForSeconds(0.35f);
            rigid.gravityScale = 0.1f;
            yield return new WaitForSeconds(0.1f);
            rigid.gravityScale = 0.2f;
            yield return new WaitForSeconds(0.2f);
            rigid.gravityScale = 0.4f;
            yield return new WaitForSeconds(0.2f);
            rigid.gravityScale = 0.7f;
            yield return new WaitForSeconds(0.2f);
            rigid.gravityScale = 0.8f;
            yield return new WaitForSeconds(0.5f);
            isAirborne = false;
        }
        rigid.gravityScale = 1;
        yield return null;
    }
    IEnumerator Tracking(Vector3 tPos)
    {
        isTrack = true;
        float cnt = 0;
        while (cnt < 1&&target!=null)
        {
            if (!isStatic)
                isLeftorRight = tPos.x < transform.position.x ? true : false;
            yield return new WaitForFixedUpdate();
            cnt += Time.deltaTime;
        }
        isTrack = false;
        yield return null;
    }
    IEnumerator Holding()
    {
        isTrack = true;
        float cnt = 0;
        while (cnt < 1)
        {
            if (!isStatic)
                isLeftorRight = firstPos.x < transform.position.x ? true : false;
            yield return new WaitForFixedUpdate();
            cnt += Time.deltaTime;
        }
        isTrack = false;
        yield return null;
    }
    IEnumerator Stunning(float stunTime)
    {
        isStunning = true;
        Knockback(10, 1);
        animator.SetTrigger("stunning");
        animator.SetBool("isStun", true);
        if (faceAnimator != null)
            faceAnimator.SetTrigger("Hit");
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        while (isStunning)
        {
            yield return new WaitForSeconds(stunTime);
            isStun = false;
            isStunning = false;
        }
        animator.SetBool("isStun", false);
        yield return null;
    }
    IEnumerator HealAttacking()
    {
        isAttack = true;
        attackNumber = UnityEngine.Random.Range(0, 5);
        isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
        RedirectCharacter();
        animator.SetInteger("attackNumber", attackNumber);
        animator.SetFloat("speed", 0.8f + this.status.attackSpeed * 2f);
        if (faceAnimator != null)
            faceAnimator.SetTrigger("Do");
        animator.SetBool("isAttack", true);
        animator.SetTrigger("attacking");
        if (playChats.Count > 0 && UnityEngine.Random.Range(0, 5) < 1)
            Common.Chat(playChats[UnityEngine.Random.Range(0, playChats.Count)], transform.GetChild(0));
        if (target != null && target.GetComponent<Hero>() != null)
            target.GetComponent<Hero>().Healing(this.Damage());
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        StopAttack();
        yield return null;
    }
    IEnumerator Supporting()
    {
        isAttack = true;
        attackNumber = 0;
        isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
        RedirectCharacter();
        animator.SetInteger("attackNumber", attackNumber);
        animator.SetFloat("speed", 0.8f + this.status.attackSpeed * 2f);
        if (faceAnimator != null)
            faceAnimator.SetTrigger("Do");
        animator.SetBool("isAttack", true);
        animator.SetTrigger("attacking");
        MusicEffect();
        var enemyList = Common.FindEnemy(this.isPlayerHero);
        foreach(var enemy in enemyList)
        {
            enemy.GetComponent<Hero>().DefenceBuff(20,-HeroSystem.GetHeroStatusSkillEnergy(ref heroData)*0.01f);
        }
        for(var i =0;i<10;i++)
        {
            if (distanceBetweenTarget > attackMaxRange+2||!TargetAliveCheck(target)||!isAttack)
                break;
            yield return new WaitForSeconds(1.0f);
        }
        target = null;
        MusicEffectOff();
        StopAttack();
        yield return null;
    }
    IEnumerator Attacking()
    {
        isAttack = true;
        if(id>500)
        {
            attackNumber = heroData.attackType;
        }
        else
        {
            attackNumber = UnityEngine.Random.Range(0, 5);
        }
        if(!isStatic)
            isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
        RedirectCharacter();
        animator.SetInteger("attackNumber", attackNumber);
        animator.SetFloat("speed", 0.8f + this.status.attackSpeed*2f);
        if (faceAnimator != null)
            faceAnimator.SetTrigger("Do");
        animator.SetBool("isAttack", true);
        animator.SetTrigger("attacking");
        if (playChats.Count > 0 && UnityEngine.Random.Range(0, 5)<1)
            Common.Chat(playChats[UnityEngine.Random.Range(0, playChats.Count)], transform.GetChild(0));

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if (target.GetComponent<Hero>() != null)
            OnAttackHeroAbility();
        StopAttack();
        yield return null;
    }
    IEnumerator SkillAttacking()
    {
        isSkillAttack = true;
        animator.SetFloat("speed", 1f);
        animator.SetBool("isSkill", true);
        animator.SetTrigger("skillAttacking");
        animator.SetInteger("skillType", skillData.id);
        float time = 0.0f;
        while(time<2.2f)
        {
            isSkillAttack = true;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StopAttack();
        isSkillAttack = false;
        yield return null;
    }
    public IEnumerator LobbyAttack()
    {
        EquipWeapon();
        animationTime = 0;
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.5f);
        if (initChats.Count > 0)
            LobbyChat(initChats[UnityEngine.Random.Range(0, initChats.Count)], transform, UnityEngine.Random.Range(0, 2));
        attackNumber = UnityEngine.Random.Range(0, 5);
        animator.SetInteger("attackNumber", attackNumber);
        animator.SetFloat("speed", 1.0f);
        if (faceAnimator != null)
            faceAnimator.SetTrigger("Do");
        animator.SetTrigger("attacking");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * 1.1f);
        Idle();
        RemoveWeapon();
    }

    IEnumerator SmokingTeleport(Transform trans)
    {
        isTrack = true;
        SmokeDarkEffect();
        yield return new WaitForSeconds(0.5f);
        if(trans.position.x>this.transform.position.x)
            this.transform.position = trans.position + new Vector3(-0.7f,0);
        else
            this.transform.position = trans.position + new Vector3(0.7f, 0);
        SmokeDarkEffect();

        yield return null;
    }
    #endregion

    #region Sprite관리
    void InitSpriteLayer()
    {
        foreach (var i in GetComponentsInChildren<SpriteRenderer>())
        {
            i.sortingLayerName = "Default";
            i.sortingOrder -= GetInstanceID();
        }
    }
    int GetSpriteLayer(string name)
    {
        int sortingOrder = 0;
        foreach (var i in GetComponentsInChildren<SpriteRenderer>())
        {
            if (i.name == "body")
            {
                sortingOrder = i.sortingOrder;
                break;
            }
        }
        return sortingOrder;
    }
    void ColorChangeSprites(Color color)
    {
        if (!isDefence)
        {
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sp in sprites)
            {
                sp.color = color;
            }
        }
    }
    void LobbyChat(string chat, Transform tran = null, int posY = 0, int leftOrRight = 0)
    {
        GameObject chatObj = ObjectPool.Instance.PopFromPool("chatBox");
        chatObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        posY = posY == 0 ? 1 : posY;
        if (leftOrRight == 0)
        {
            if (tran.rotation.y == 0)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

            }
            else
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            }
        }
        else
        {
            if (leftOrRight == 1)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (leftOrRight == 2)
            {
                chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        chatObj.GetComponent<UI_chatBox>().correctionY = posY;
        chatObj.GetComponent<UI_chatBox>().Target = tran;
        chatObj.GetComponent<UI_chatBox>().chatText = chat;
        chatObj.SetActive(true);
    }

    #endregion

    #region 유저인터페이스
    public void DamageUIShow(string font, bool isCritical = false)
    {
        if (ObjectPool.Instance != null)
        {
            GameObject damageUIprefab = ObjectPool.Instance.PopFromPool("damageUI", GameObject.Find("CanvasUI").transform) as GameObject;
            damageUIprefab.GetComponentInChildren<Text>().text = font.ToString();
            damageUIprefab.GetComponent<TextDamageController>().isLeft = !isLeftorRight;
            damageUIprefab.GetComponent<TextDamageController>().isCritical = isCritical;
            damageUIprefab.GetComponent<TextDamageController>().isCC = false;
            damageUIprefab.GetComponent<TextDamageController>().isFixed = false;
            if(isCritical)
                damageUIprefab.transform.position = transform.GetChild(0).position + new Vector3(0, 1.5f);
            else
                damageUIprefab.transform.position = transform.GetChild(0).position + new Vector3(0, 1.2f);
            damageUIprefab.SetActive(true);
        }
    }
    public void DamageFixedUIShow(string font)
    {
        if (ObjectPool.Instance != null)
        {
            GameObject damageUIprefab = ObjectPool.Instance.PopFromPool("damageUI", GameObject.Find("CanvasUI").transform) as GameObject;
            damageUIprefab.GetComponentInChildren<Text>().text = font.ToString();
            damageUIprefab.GetComponent<TextDamageController>().isLeft = !isLeftorRight;
            damageUIprefab.GetComponent<TextDamageController>().isCritical = false;
            damageUIprefab.GetComponent<TextDamageController>().isCC = false;
            damageUIprefab.GetComponent<TextDamageController>().isFixed = true;
            damageUIprefab.transform.position = transform.GetChild(0).position + new Vector3(0, 1.2f);
            damageUIprefab.SetActive(true);
        }
    }
    public void DamageCCUIShow(string font)
    {
        if(ObjectPool.Instance!=null)
        {
            GameObject damageUIprefab = ObjectPool.Instance.PopFromPool("damageUI", GameObject.Find("CanvasUI").transform) as GameObject;
            damageUIprefab.GetComponentInChildren<Text>().text = string.Format("<size='27'>{0}</size>",font.ToString());
            damageUIprefab.GetComponent<TextDamageController>().isLeft = !isLeftorRight;
            damageUIprefab.GetComponent<TextDamageController>().isCritical = false;
            damageUIprefab.GetComponent<TextDamageController>().isFixed = false;
            damageUIprefab.GetComponent<TextDamageController>().isCC = true;
            damageUIprefab.transform.position = transform.GetChild(0).position;
            damageUIprefab.SetActive(true);
        }

    }
    private void OpenHpBar(bool isBlue = false)
    {
        if(id<1000)
        {
            hpUI = ObjectPool.Instance.PopFromPool("hpEnemyUI");
            hpUI.GetComponent<UI_hp>().OpenHpUI(this.gameObject, isBlue);
            hpUI.gameObject.SetActive(true);
        }
        else
        {
            if(Common.stageModeType!=Common.StageModeType.Boss)
            {
                hpUI = ObjectPool.Instance.PopFromPool("hpCastleUI");
                hpUI.GetComponent<UI_castleHp>().OpenHpUI(this.gameObject, isBlue);
                hpUI.gameObject.SetActive(true);
            }
        }
    }
    private void ShowHpBar(int dam=0)
    {
        if (!isDead && status.hp > 0&&hpUI!=null)
        {
            if (!hpUI.gameObject.activeSelf)
            {
                if(id>1000)
                    hpUI.GetComponent<UI_castleHp>().panelHpTime = 0;
                else
                    hpUI.GetComponent<UI_hp>().panelHpTime = 0;
                hpUI.gameObject.SetActive(true);
            }
            if(dam>0)
            {
                if(id>1000)
                    hpUI.GetComponent<UI_castleHp>().GetDamage(dam);
                else
                    hpUI.GetComponent<UI_hp>().GetDamage(dam);
            }

        }
    }
    private void ShowSkillCastingUI()
    {
        if(isPlayerHero)
        {
            GameObject castingUI = ObjectPool.Instance.PopFromPool("SkillCastingUI");
            castingUI.SetActive(true);
            castingUI.GetComponent<UI_SkillCasting>().ShowCastingUI(HeroSystem.GetHeroThumbnail(this.id), SkillSystem.GetSkillName(this.skillData.id),false,SkillSystem.GetSkillImage(this.skillData.id));
        }
        else
        {
            if(id<1000)
            {
                GameObject castingUI = ObjectPool.Instance.PopFromPool("SkillCastingReverseUI");
                castingUI.SetActive(true);
                castingUI.GetComponent<UI_SkillCasting>().ShowCastingUI(HeroSystem.GetHeroThumbnail(this.id), SkillSystem.GetSkillName(this.skillData.id), true, SkillSystem.GetSkillImage(this.skillData.id));
            }
        }
    }
    #endregion

    #region 사운드 매니저
    public void Sound_HeartBeat()
    {
        SoundManager.instance.PlaySingleLoop(AudioClipManager.instance.heartBeat);
    }
    public void Sound_ReloadPistol()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.reloadPistol);
    }
    public void Sound_FootStep()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.jump);
    }
    public void Sound_Jump()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.jump);
    }
    public void Sound_Damage()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.damage1, AudioClipManager.instance.damage2);
    }
    public void Sound_Dead()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.dead);
    }
    public void Sound_Punch()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.punchHit);
    }
    public void Sound_Sword()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.swingSword);
    }
    public void Sound_Knife()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.swingKnife);
    }
    public void Sound_Spell()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.spell);
    }
    public void Sound_Magic()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.magic);
    }
    #endregion

    #region 영웅레벨매니저
    [Serializable]
    public class Status
    {
        [HideInInspector]
        public int level;
        [HideInInspector]
        public int exp;
        [HideInInspector]
        public int attack;
        [HideInInspector]
        public int defence;
        [HideInInspector]
        public int hp;
        [HideInInspector]
        public int skillEnegry;
        [HideInInspector]
        public float penetration;
        [HideInInspector]
        public int maxHp;
        [Range(0, 100)]
        public int criticalPercent;
        [Range(0.05f,2)]
        public float attackSpeed;
        [Range(0.1f, 3)]
        public float moveSpeed;
        [Range(0, 15)]
        public float knockbackResist;
        [Range(10, 120)]
        public float resurrectionTime;

        public Status() { level = 1; exp = 0;}

        public void SetHeroStatus(ref HeroData data, bool isLevelUp, Hero prefabData = null)
        {
            this.level = data.level;
            this.exp = data.exp;
            this.attack = HeroSystem.GetHeroStatusAttack(ref data);
            this.defence = HeroSystem.GetHeroStatusDefence(ref data);
            this.maxHp = HeroSystem.GetHeroStatusMaxHp(ref data)+ prefabData.OnReturnHeroAbility(9);
            this.hp = isLevelUp?hp : this.maxHp;
            this.criticalPercent = HeroSystem.GetHeroStatusCriticalPercent(ref data);
            this.attackSpeed = HeroSystem.GetHeroStatusAttackSpeed(ref data)*0.005f;
            this.moveSpeed = Mathf.Clamp(HeroSystem.GetHeroStatusMoveSpeed(ref data)*0.01f,0.5f,3.5f);
            this.knockbackResist = HeroSystem.GetHeroStatusKnockbackResist(ref data);
            this.skillEnegry = 20 - HeroSystem.GetHeroStatusSkillEnergy(ref data);
            this.penetration = HeroSystem.GetHeroStatusPenetration(ref data)*0.01f;
            this.resurrectionTime = HeroSystem.GetHeroResurrectionTime(data.id, 0);
        }

        public void SetPvpStatus(ref HeroData data, Hero prefabData = null)
        {
            this.level = data.level;
            this.exp = data.exp;
            this.attack = StageBattleManager.instance.GetHeroStatusAttack(ref data);
            this.defence = StageBattleManager.instance.GetHeroStatusDefence(ref data);
            this.maxHp = StageBattleManager.instance.GetHeroStatusMaxHp(ref data) + prefabData.OnReturnHeroAbility(9);
            this.hp = this.maxHp;
            this.criticalPercent = StageBattleManager.instance.GetHeroStatusCriticalPercent(ref data);
            this.attackSpeed = StageBattleManager.instance.GetHeroStatusAttackSpeed(ref data) * 0.005f;
            this.moveSpeed = Mathf.Clamp(StageBattleManager.instance.GetHeroStatusMoveSpeed(ref data) * 0.01f, 0.45f, 3.5f);
            this.knockbackResist = StageBattleManager.instance.GetHeroStatusKnockbackResist(ref data);
            this.skillEnegry = 20 - StageBattleManager.instance.GetHeroStatusSkillEnergy(ref data);
            this.penetration = HeroSystem.GetHeroStatusPenetration(ref data) * 0.01f;
            this.resurrectionTime = HeroSystem.GetHeroResurrectionTime(data.id, 0);
        }

        public void SetTutorialHeroStatus(bool isPlayer)
        {
            this.level = 1;
            this.exp = 0;
            this.attack = !isPlayer ? 5 : 60;
            this.defence = 30;
            this.maxHp = !isPlayer ? 100 : 300;
            this.hp = this.maxHp;
            this.criticalPercent = 0;
            this.attackSpeed = 1.0f;
            this.moveSpeed = 1.5f;
            this.knockbackResist = 0;
            this.skillEnegry = 0;
            this.resurrectionTime = 0;
        }
    }

    int GetHeroMaxLevel()
    {
        return this.heroData.over * 10 + 100;
    }
   
    public void LevelUp()
    {
        if (status.exp >= Common.GetHeroNeedExp(status.level)&& status.level < GetHeroMaxLevel() && !isDead)
        {
            LevelUpEffect();
            HeroSystem.LevelUpStatusSet(this.id,this);
            if (id < 500&&hpUI!=null)
            {
                hpUI.GetComponent<UI_hp>().SetLevelUI(this.status.level);
            }
            Debugging.Log(name + " 의 레벨업  >> " + status.level);
        }
    }

    public void ExpUp(int nExp)
    {
        if (status!=null&& !isDead)
        {
            if(status.level< GetHeroMaxLevel())
            {
                status.exp += nExp;
                HeroSystem.SetHero(this);
                LevelUp();
            }
        }
    }
    #endregion

    #region 영웅스킬모음
    public IEnumerator Skill001()
    {
        SlashEffect();
        var targetList = Common.FindEnemysByDistance(this.isPlayerHero,isLeftorRight,this.transform,4.0f);
        foreach(var target in targetList)
        {
            target.GetComponent<Hero>().HittedByObject(this.Damage(), true, new Vector2(10, 10),this.status.penetration);
            target.GetComponent<Hero>().SlashHitEffect();
            yield return new WaitForEndOfFrame();
        }
        if(Common.hitTargetObject!=null&&Common.hitTargetObject.GetComponent<Castle>()!=null&&target!=null)
        {
            if(Common.hitTargetObject==target&&Common.GetDistanceBetweenAnother(this.transform,Common.hitTargetObject.transform)<4.0f)
                Common.hitTargetObject.GetComponent<Castle>().HittedByObject(this.Damage(), true, Vector2.zero);
        }
        yield return null;
    }
    public IEnumerator Skill002()
    {
        if (target != null && ObjectPool.Instance != null)
        {
            int totalCount = Mathf.Clamp(SkillSystem.GetUserSkillLevel(skillData.id) / 5 + 1,1,10);
            GameObject bomb = ObjectPool.Instance.PopFromPool("Bomb");
            bomb.transform.position = this.transform.position;
            bomb.gameObject.SetActive(true);
            bomb.GetComponent<Boom>().StartBomb(totalCount,isPlayerHero, Damage(), isCriticalAttack,target);
        }
        yield return null;
    }
    public IEnumerator Skill003()
    {
        int totalCount = Mathf.Clamp(SkillSystem.GetUserSkillLevel(skillData.id)/5 + 1,1,8);
        for (int i = 0; i < totalCount; i++)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.magic);
            if (target != null && ObjectPool.Instance != null)
            {
                Vector3 pos = transform.GetChild(0).position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-1f, 1f));
                MagicSpawnEffect(pos);
                yield return new WaitForSeconds(0.2f);
                if (target != null)
                {
                    GameObject bullet = ObjectPool.Instance.PopFromPool("FlamethrowerCartoonyFire");
                    bullet.transform.position = pos;
                    bullet.transform.rotation = IsTargetLeft(target.transform) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                    bullet.GetComponent<bulletController>().damage = this.SkillDamage()/totalCount;
                    bullet.GetComponent<bulletController>().pent = this.status.penetration;
                    bullet.GetComponent<bulletController>().isAlly = this.isPlayerHero;
                    bullet.GetComponent<bulletController>().Target = target.transform;
                    bullet.GetComponent<bulletController>().isCritical = isCriticalAttack;

                    bullet.SetActive(true);
                }
            }
            yield return new WaitForSeconds(0.03f);
        }
        yield return null;
    }
    public IEnumerator Skill004()
    {
        int totalCount = Mathf.Clamp(SkillSystem.GetUserSkillLevel(skillData.id) / 5 + 1,1,8);
        for (int i = 0; i < totalCount; i++)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.bow);
            if (target != null && ObjectPool.Instance != null)
            {
                GameObject arrow = ObjectPool.Instance.PopFromPool("Arrow");

                arrow.GetComponent<arrowController>().damage = this.SkillDamage();
                arrow.GetComponent<arrowController>().pent = this.status.penetration;
                arrow.GetComponent<arrowController>().isAlly = this.isPlayerHero;
                arrow.GetComponent<arrowController>().target = target.transform;
                arrow.GetComponent<arrowController>().isCritical = isCriticalAttack;
                arrow.transform.position = transform.position + new Vector3(0, 0.1f) + transform.right * -1f;
                arrow.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                arrow.SetActive(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
    public IEnumerator Skill005()
    {
        SmokeDarkEffect();
        bool flag = false;
        yield return new WaitForSeconds(0.5f);
        if (target != null)
        {
            if (target.transform.position.x > this.transform.position.x)
            {
                this.transform.position = Common.GetBottomPosition(target.transform) + new Vector3(-0.7f, 0);
                flag = true;
            }

            else
            {
                this.transform.position = Common.GetBottomPosition(target.transform) + new Vector3(0.7f, 0);
                flag = false;
            }

        }
        SmokeDarkEffect();
        attackPoint.name = this.SetDamageData(this.SkillDamage(), this.status.penetration);
        int totalCount = Mathf.Clamp(SkillSystem.GetUserSkillLevel(skillData.id) / 5 + 1,1,10);
        for (int i = 0; i < totalCount; i++)
        {

            attackPoint.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            attackPoint.gameObject.SetActive(false);
            i++;
        }

        if(target!=null)
        {
            if (!flag)
                this.transform.position = Common.GetBottomPosition(target.transform) + new Vector3(-0.7f, 0);
            else
                this.transform.position = Common.GetBottomPosition(target.transform) + new Vector3(0.7f, 0);
            SmokeDarkEffect();
        }

        yield return null;
    }
    public IEnumerator Skill006()
    {
        JumpEffect();
        SlamEffect();
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.smoke);
        var targetList = Common.FindEnemysByDistance(this.isPlayerHero,isLeftorRight, this.transform, 5.0f);
        yield return new WaitForSeconds(0.2f);
        int totalCount = Mathf.Clamp(SkillSystem.GetUserSkillLevel(skillData.id) / 3 + 1,1,10);
        for(var i = 0; i<totalCount&&i<targetList.Count; i++)
        {
            targetList[i].GetComponent<Hero>().HittedByObject(this.Damage(), true, new Vector2(20, 20),this.status.penetration);
            yield return new WaitForEndOfFrame();
        }
        attackPoint.gameObject.SetActive(false);
        yield return null;
    }
    public IEnumerator Skill007()
    {
        GroundEffect();
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.spell007);
        foreach(var ally in Common.FindAlly(this.isPlayerHero))
        {
            if(TargetAliveCheck(ally))
            {
                ally.GetComponent<Hero>().AttackBuff(2.3f, SkillSystem.GetUserSkillPower(skillData.id)*0.01f);
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public IEnumerator Skill008()
    {
        GameObject missile = EffectPool.Instance.PopFromPool("RocketMissileBlue");
        missile.transform.position = this.transform.GetChild(0).position;
        missile.gameObject.SetActive(true);
        
        while (missile.transform.position.y>this.transform.GetChild(0).position.y+5)
        {
            missile.transform.Translate(new Vector3(0, 2 * Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
        SlamEffect();
        float distance = 1.0f;
        float delay = 0.0f;
        while(distance>0.5f&&target!=null)
        {
            distance = Vector2.Distance(missile.transform.position, target.transform.position);
            missile.transform.position = Vector2.Lerp(missile.transform.position, target.transform.position, 0.07f);
            delay += Time.deltaTime;
            if (delay > 3.0f)
                break;
            yield return new WaitForEndOfFrame();
        }
        EffectPool.Instance.PushToPool("RocketMissileBlue", missile);
        if(target!=null&&target.GetComponent<Hero>()!=null)
        {
            target.GetComponent<Hero>().HittedByObject(this.SkillDamage(), true, new Vector2(40, 20),this.status.penetration);
            target.GetComponent<Hero>().BoomEffect();
        }
        else if(target != null&&target.GetComponent<Castle>()!=null)
        {
            target.GetComponent<Castle>().HittedByObject(this.SkillDamage(), true, Vector2.zero);
            target.GetComponent<Castle>().BoomEffect();
        }
        yield return null;
    }
    public IEnumerator Skill009()
    {
        GameObject embulance = ObjectPool.Instance.PopFromPool("Embulance");
        embulance.transform.position = this.transform.position;
        if(isLeftorRight)
            embulance.transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            embulance.transform.rotation = Quaternion.Euler(0, 0, 0);
        embulance.SetActive(true);
        float timeCheck = 0.0f;
        while (timeCheck<2)
        {
            embulance.transform.Translate(new Vector2(5 * Time.deltaTime, 0));
            timeCheck += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        var allyList = Common.FindAlly(this.isPlayerHero);
        foreach (var ally in allyList)
        {
            if(TargetAliveCheck(ally))
                ally.GetComponent<Hero>().Healing(this.Damage());
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public IEnumerator Skill010()
    {
        int dam = this.Damage();
        attackPoint.name = this.SetDamageData(this.Damage(), this.status.penetration);
        for (int i = 0; i < 4; i++)
        {
            attackPoint.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            attackPoint.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
        var targetList = Common.FindEnemysByDistance(this.isPlayerHero,isLeftorRight, this.transform, 4.0f);
        foreach (var target in targetList)
        {
            if(TargetAliveCheck(target))
            {
                target.GetComponent<Hero>().HittedByObject(this.SkillDamage(), true, new Vector2(20, 20), this.status.penetration);
                target.GetComponent<Hero>().SlashHitEffect();
            }
            break;
        }
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator Skill011()
    {
        var targetList = Common.FindEnemysByDistance(this.isPlayerHero, isLeftorRight, this.transform, 8.0f);
        Vector3 effectPos = Vector3.zero;
        List<GameObject> effectTransfoms = new List<GameObject>();
        float effectY = 5;
        for(var i =0; i<6; i++)
        {
            if (EffectPool.Instance != null)
            {
                GameObject effect = EffectPool.Instance.PopFromPool("SpikyFireAdditivePurple");
                if (effect != null)
                {
                    if(isLeftorRight)
                    {
                        effectPos = new Vector3(this.transform.position.x-(i + 1), effectY);
                        effect.transform.position = effectPos;
                    }
                    else
                    {
                        effectPos = new Vector3(this.transform.position.x+(i + 1), effectY);
                        effect.transform.position = effectPos;
                    }
                    effect.SetActive(true);
                    effectTransfoms.Add(effect);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.fireShot);
        while (effectY>0)
        {
            effectY -= 8 * Time.deltaTime;
            foreach (var efx in effectTransfoms)
            {
                efx.transform.position = new Vector3(efx.transform.position.x, effectY, 0);
            }

            yield return new WaitForEndOfFrame();
        }

        for (var i = 0; i < 3; i++)
        {
            if (EffectPool.Instance != null)
            {
                GameObject effect = EffectPool.Instance.PopFromPool("SoulExplosionPurple");
                if (effect != null)
                {
                    if (isLeftorRight)
                    {
                        effectPos = new Vector3(this.transform.position.x-(i + 1)*1.5f, 0);
                        effect.transform.position = effectPos;
                    }
                    else
                    {
                        effectPos = new Vector3(this.transform.position.x+(i + 1)*1.5f, 0);
                        effect.transform.position = effectPos;
                    }
                    effect.SetActive(true);
                }
            }
        }
        foreach (var target in targetList)
        {
            if (TargetAliveCheck(target))
            {
                target.GetComponent<Hero>().HittedByObject(this.SkillDamage(), true, new Vector2(5, 5), this.status.penetration);
                yield return new WaitForEndOfFrame();
                target.GetComponent<Hero>().Stunned(0.5f,true);
                Debugging.Log(target.name);
                yield return new WaitForEndOfFrame();
            }
        }
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.smoke);
        yield return new WaitForEndOfFrame();
    }
    public IEnumerator Skill012()
    {
        int totalCount = Mathf.Clamp(SkillSystem.GetUserSkillLevel(skillData.id) / 5 + 1, 1, 7);
        for (int i = 0; i < totalCount; i++)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.bow);
            if (target != null && ObjectPool.Instance != null)
            {
                GameObject arrow = ObjectPool.Instance.PopFromPool("Shuriken");

                arrow.GetComponent<arrowController>().pent = this.status.penetration;
                arrow.GetComponent<arrowController>().isAlly = this.isPlayerHero;
                arrow.GetComponent<arrowController>().target = target.transform;
                arrow.GetComponent<arrowController>().isCritical = isCriticalAttack;
                arrow.GetComponent<arrowController>().damage = isCriticalAttack ? (int)(this.SkillDamage()*1.1f) : this.SkillDamage();

                arrow.transform.position = transform.position + new Vector3(0, 0.1f) + transform.right * -1f;
                arrow.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                arrow.SetActive(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
    #endregion

    #region 보스스킬모음
    public IEnumerator BSkill004()
    {
        if(UnityEngine.Random.Range(0,10)<3)
        {
            var targetList = Common.FindAlly(true);
            if(targetList!=null&&targetList.Count>0)
            {
                foreach (var target in targetList)
                {
                    target.GetComponent<Hero>().HittedByObject(this.Damage() / Mathf.Clamp((targetList.Count/2),1,4), true, new Vector2(20, 20));
                    target.GetComponent<Hero>().SlamEffect();
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        yield return null;
    }
    public IEnumerator BSkill1006()
    {
        GameObject spawnEnemy = PrefabsDatabaseManager.instance.GetMonsterPrefab(516);
        for (int i = 0; i < 5; i++)
        {
            GameObject e = Instantiate(spawnEnemy);
            e.SetActive(false);
            e.GetComponent<Hero>().isPlayerHero = false;
            e.GetComponent<Hero>().SpriteAlphaSetting(0);
            e.transform.position = this.transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0);
            e.GetComponent<Hero>().SpawnEffect();
            yield return new WaitForSeconds(0.5f);
            e.SetActive(true);
            e.GetComponent<Hero>().SpriteAlphaSetting(1);
            StageManagement.instance.AddMonsterCount();
            List<GameObject> allys = Common.FindAlly(true);
            foreach (var ally in allys)
            {
                if (ally.GetComponent<Hero>() != null)
                {
                    ally.GetComponent<Hero>().ResearchingEnemys(e);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public IEnumerator BSkill1007()
    {
        SlashEffect();
        var targetList = Common.FindAlly(true);
        foreach (var target in targetList)
        {
            target.GetComponent<Hero>().HittedByObject(this.SkillDamage(), true, new Vector2(10, 15), this.status.penetration);
            target.GetComponent<Hero>().SlamEffect();
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public IEnumerator BSkill1008()
    {
        JumpEffect();
        SlamEffect();
        var targetList = Common.FindAlly(true);
        if(targetList!=null&&targetList.Count>0)
        {
            for (var i = 0; i < targetList.Count; i++)
            {
                if (TargetAliveCheck(targetList[i]))
                {
                    targetList[i].GetComponent<Hero>().HittedByObject(this.SkillDamage(), true, new Vector2(30, 35), 0.5f);
                    targetList[i].GetComponent<Hero>().SlamEffect();
                    yield return new WaitForEndOfFrame();
                    targetList[i].GetComponent<Hero>().Stunned(3.0f, true);
                    yield return null;
                }
            }
        }
    }
    #endregion

    #region 잠재능력
    public void OnAttackHeroAbility()
    {
        switch(heroData.ability)
        {
            case 1:
                HeroAbility001();
                break;
            case 2:
                HeroAbility002();
                break;
            case 6:
                HeroAbility006(target);
                break;
            case 7:
                HeroAbility007(target);
                break;
        }
    }
    public int OnReturnHeroAbility(int type)
    {
        switch(type)
        {
            case 3:
                return HeroAbility003();
            case 4:
                return HeroAbility004();
            case 5:
                return HeroAbility005();
            case 8:
                return HeroAbility008();
            case 9:
                return HeroAbility009();
        }
        return 0;
    }
    public void HeroAbility001()
    {
        if(heroData.ability==1)
        {
            int drainAmount = Mathf.Clamp((HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel)*status.maxHp)/100, 0, this.status.maxHp);
            this.status.hp = Common.looHpPlus(this.status.hp, this.status.maxHp, drainAmount);
            DamageCCUIShow("+" + drainAmount.ToString());
            ShowHpBar(-drainAmount);
        }
    }
    public void HeroAbility002()
    {
        if(heroData.ability==2)
        {
            int drainAmount = Mathf.Clamp(HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel), 0, 100);
            if (StageManagement.instance != null)
            {
                StageManagement.instance.stageInfo.stageEnergy += drainAmount;
            }
        }
    }
    public int HeroAbility003()
    {
        if(heroData.ability==3)
        {
            int ctkDmg = Mathf.Clamp(HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel), 0, 100);
            return ctkDmg;
        }
        return 0;
    }
    public int HeroAbility004()
    {
        if (heroData.ability == 4)
        {
            int atk = Mathf.Clamp(HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel), 0, 300);
            return atk;
        }
        return 0;
    }
    public int HeroAbility005()
    {
        if (heroData.ability == 5)
        {
            int def = Mathf.Clamp(HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel), 0, 300);
            return def;
        }
        return 0;
    }
    public void HeroAbility006(GameObject target)
    {
        if (heroData.ability == 6)
        {
            int percent = HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel);
            if (target != null && target.GetComponent<Hero>()!=null&& !target.GetComponent<Hero>().isDead)
            {
                if (UnityEngine.Random.Range(0, 100) < percent)
                    target.GetComponent<Hero>().Stunned(2.0f);
            }
        }
    }
    public void HeroAbility007(GameObject target)
    {
        if (heroData.ability == 7)
        {
            if (target != null && target.GetComponent<Hero>() != null && !target.GetComponent<Hero>().isDead)
            {
                target.GetComponent<Hero>().FireEffect();
                target.GetComponent<Hero>().HittedPersistent(HeroAbilitySystem.GetAbilityPower(heroData.ability,heroData.abilityLevel), 5);
            }
        }
    }
    public int HeroAbility008()
    {
        if (heroData.ability == 8)
        {
            return HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel);
        }
        return 0;
    }
    public int HeroAbility009()
    {
        if (heroData.ability == 9)
        {
            return HeroAbilitySystem.GetAbilityPower(heroData.ability, heroData.abilityLevel);
        }
        return 0;
    }

    #endregion
}
