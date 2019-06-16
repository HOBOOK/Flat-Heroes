using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Hero : MonoBehaviour
{
    #region 변수
    //id
    public int id;
    //string
    public string HeroName;
    public List<string> initChats = new List<string>();
    public List<string> playChats = new List<string>();
    public List<string> endChats = new List<string>();
    //boolean
    public bool isPlayerHero = false;
    public bool isStage = false;
    public bool isCriticalAttack = false;
    bool isStart = false;
    public bool isNeutrality;
    public bool isHold = false;
    public bool isLeftorRight = false;
    public bool isFriend = false;
    public bool isDead = false;
    public bool isUnBeat = false;
    private bool isAttack = false;
    public bool isSkillAttack = false;
    private bool isWait = false;
    private bool isTrack = false;
    private bool isJumping = false;
    public bool isDefence = false;
    public bool isStun = false;
    private bool isStunning = false;
    private bool isClimb = false;
    private bool isClimbing = false;
    public bool isAir = false;
    private bool isAirborne = false;
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
    //GameObject
    public GameObject target;
    List<GameObject> enemys = new List<GameObject>();
    public GameObject weaponPoint;
    public GameObject attackPoint;
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
    public enum WeaponType { No,Gun,Sword,Knife,Staff,Bow}
    //Rigidbody2D
    Rigidbody2D rigid;
    //Animator
    public Animator animator;
    Animator faceAnimator;
    //InnerClass
    public Status status;
    //HeroData
    HeroData heroData;
    #endregion

    #region Awake,Start,Update
    private void Awake()
    {

    }
    void Start()
    {
        InitHero();
        StartHero();
        firstPos = this.transform.position;

        if (isPlayerHero)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
            isLeftorRight = false;
        }
        else
        {
            isLeftorRight = true;
        }
        animator = GetComponent<Animator>();
        faceAnimator = GetComponentInChildren<faceOff>().GetFaceAnimator();
        rigid = GetComponent<Rigidbody2D>();
        if (attackPoint.GetComponent<SpriteRenderer>() != null)
            attackPoint.GetComponent<SpriteRenderer>().enabled = false;
        SetAttackAnimation();
        RemoveWeapon();
        if(isStage)
        {
            OpenHpBar(isPlayerHero);
            InitEnemys();
            StartCoroutine("StartAttackMode");
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
    #endregion

    #region 계산함수
    public int ATTACK
    {
        get
        {
            return status.attack;
        }
        set { }
    }
    public int DEFENCE
    {
        get
        {
            return status.defence;
        }
        set { }
    }
    public int Damage()
    {
        if (UnityEngine.Random.Range(1, 100) <= this.status.criticalPercent)
            isCriticalAttack = true;
        else
            isCriticalAttack = false;
        float dam = isCriticalAttack ? ATTACK * 1.5f: ATTACK;
        dam = isSkillAttack ? dam * 2 : dam;
        dam = UnityEngine.Random.Range(dam * 0.8f, dam * 1.2f);
        return (int)dam;
    }
    void RecoveryHp()
    {
        recoveryHpTime += Time.deltaTime;
        if(!isDead&&status.hp>0&&recoveryHpTime>1.0f)
        {
            status.hp=Common.looHpPlus(status.hp, status.maxHp, HeroSystem.GetRecoveryHp(heroData));
            recoveryHpTime = 0;
        }
    }
    void DistanceChecking()
    {
        if (target != null)
            distanceBetweenTarget = Vector2.Distance(Common.GetBottomPosition(target.transform), Common.GetBottomPosition(transform));
    }
    void RemoveWeapon()
    {
        if (weaponPoint.transform.childCount > 0 && weaponPoint.transform.GetChild(0).gameObject.activeSelf)
        {
            animator.SetInteger("weaponType", -1);
            weaponPoint.transform.GetChild(0).gameObject.SetActive(false);
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
    void FindEnemys(bool isForce=false)
    {
        if(!isAttack  && !isClimb && !isClimbing&&isStage)
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
                            }
                            tempTargetDistance = Common.GetDistanceBetweenAnother(transform, enemys[i-1].transform);
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
            }
            if(isPlayerHero)
            {
                if (target == null && enemys.Count < 1)
                {
                    // 캐슬타겟
                    if (Common.hitTargetObject != null && !Common.hitTargetObject.GetComponent<Castle>().isDead)
                    {
                        target = Common.hitTargetObject;
                    }
                }
                //else if (target == Common.hitTargetObject && enemys.Count > 0)
                //{
                //    target = null;
                //}
            }
            if(Common.hitTargetObject==null)
            {
                if (!isPlayerHero)
                    status.hp = 0;
                target = null;
            }
        }
    }
    public void ResearchingEnemys(GameObject enemy)
    {
        if(!TargetDeadCheck(enemy))
            enemys.Add(enemy);
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
        Debug.DrawRay(transform.position + new Vector3(0, -0.6f, 0), isLeftorRight ? Vector2.left : Vector2.right, Color.red, 0.2f);
        if (checkJumpHit2D)
        {
            isJumping = true;
        }
        //방향전환 레이어 (필드)
        layerMask = 1 << 31;
        checkTurnHit2D = Physics2D.Raycast(transform.position + new Vector3(0, -0.5f, 0), isLeftorRight ? Vector2.left : Vector2.right, 0.2f, layerMask);
        Debug.DrawRay(transform.position + new Vector3(0, -0.5f, 0), isLeftorRight ? Vector2.left : Vector2.right, Color.blue, 0.2f);
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
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.localPosition, Vector2.down, 1.3f, layerMask);
            Debug.DrawRay(transform.localPosition, Vector3.down * 1.3f, Color.blue, 1f);
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
    #endregion

    #region 영웅세팅
    void InitHero()
    {
        if (Common.GetSceneCompareTo(Common.SCENE.STAGE))
        {
            isStage = true;
        }
        this.tag = isPlayerHero ? "Hero" : "Enemy";
        InitSpriteLayer();
        if(isStage)
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
        heroData = null;
        if (isPlayerHero)
            heroData = HeroSystem.GetUserHero(id);
        else
            heroData = HeroSystem.GetHero(id);
        if (heroData!=null)
        {
            this.status.SetHeroStatus(heroData);
            this.HeroName = heroData.name;
            this.name = HeroName;
            initChats = HeroSystem.GetHeroChats(heroData.id);
        }
        else
        {
            Debugging.LogWarning(this.name + " 영웅의 Status 데이터 받아오기 실패.");
        }
    }

    void SpriteAlphaSetting(float alpha)
    {
        if(isPlayerHero&&isStage)
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
                break;
            case WeaponType.Bow:
                attackRange = 10.0f + scale;
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 3);
                break;
        }
        attackMaxRange = UnityEngine.Random.Range(attackRange * 0.8f, attackRange);
    }
    void AttackMode()
    {
        RecoveryHp();
        DistanceChecking();
        if (distanceBetweenTarget < attackMaxRange && distanceBetweenTarget >= attackMinRange&&target!=null)
        {
            Attack();
        }
        else
        {
            if (!isAttack && !isUnBeat && !isStunning && !isClimb && !isWait && !isAirborne&&!isClimb&&!isClimbing&&!isSkillAttack)
            {
                ChangeNpcMode();
                Track();
            }
        }
    }
    void RedirectCharacter()
    {
        if (target != null && target.activeSelf)
        {
            isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
        }
        transform.rotation = isLeftorRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }
    void ChangingAttackMode()
    {
        if (heroState != HeroState.Attack)
        {
            SetFalseAllboolean();
            RedirectCharacter();
            heroState = HeroState.Attack;
            EquipWeapon();
        }
    }
    void ChangingNormalMode()
    {
        if (heroState != HeroState.Normal)
            heroState = HeroState.Normal;
    }
    void StateChecking()
    {
        if (isStart)
        {
            switch (heroState)
            {
                case HeroState.Normal:
                    Normal();
                    break;
                case HeroState.Attack:
                    AttackMode();
                    break;
            }

        }
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
            isDead = true;
            if (endChats.Count > 0)
                Common.Chat(endChats[UnityEngine.Random.Range(0, endChats.Count)], transform);
            StopAllCoroutines();
            SetFalseAllboolean();
            StartCoroutine("DroppingItem");
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.gravityScale = 1;
            faceAnimator.SetBool("isDead", true);
            rigid.velocity = Vector3.zero;
            Knockback(12, 23);
            attackPoint.SetActive(false);
            animator.SetTrigger("deading");
            if (!isPlayerHero)
            {
                MissionSystem.AddClearPoint(MissionSystem.ClearType.EnemyKill);
                StageManagement.instance.AddExp(20);
                StageManagement.instance.SetKPoint();
                List<GameObject> allyHeros = Common.FindAlly();
                if(allyHeros.Count>0)
                {
                    string debugStr = "";
                    foreach (var hero in allyHeros)
                    {
                        int exp = (int)((status.level * 0.25f) * (status.attack + status.defence));
                        debugStr += string.Format("<{0} + {1}>, ", hero.name, exp);
                        hero.GetComponent<Hero>().ExpUp(exp);
                    }
                    Debugging.Log(debugStr);
                }
            }
            else
            {
                StageManagement.instance.SetDPoint();
            }
            StartCoroutine("TransparentSprite");
        }
    }
    public bool isSkillAble()
    {
        if (!isSkillAttack && !isDead)
        {
            return true;
        }
        return false;
    }
    public void SkillAttack()
    {
        if (isSkillAble())
        {
            StartCoroutine("SkillAttacking");
        }
    }
    void Attack()
    {
        isTrack = false;
        animator.SetBool("isMoving", false);
        animator.SetBool("isRun", false);


        if (!isAttack && !isUnBeat && !isWait && !isStunning && !isAirborne&&!isClimb&&!isClimbing&&!isSkillAttack)
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
        if(!isTrack && target !=null&&weaponType==WeaponType.Knife&&distanceBetweenTarget>attackMaxRange+0.5f && distanceBetweenTarget < 5)
        {
            StartCoroutine(SmokingTeleport(target.transform));
        }
        else if (!isTrack && target != null)
        {
            StartCoroutine(Tracking(target.transform.position));
        }

    }
    void Hold()
    {
        if (Vector3.Distance(firstPos, transform.position) > 2)
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isRun", true);
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (!isTrack)
                StartCoroutine(Tracking(firstPos));
        }
        else
        {
            Idle();
        }
        ChangeNpcMode();
    }
    void Run()
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        isLeftorRight = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
    }
    void Running()
    {
        ChangeNpcMode();

        if (animator.GetBool("isMoving") && !isStunning && !isClimb && !isSkillAttack)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            Vector3 moveVelocity = Vector3.zero;
            CheckHurdle();
            RedirectCharacter();
            moveVelocity = isLeftorRight ? Vector3.left : Vector3.right;
            transform.position += moveVelocity * (animator.GetBool("isRun") ? status.moveSpeed * 1.7f : status.moveSpeed) * Time.deltaTime;
        }
    }
    void StopAttack()
    {
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
    public IEnumerator ShootFire(int count = 1)
    {
        count = Mathf.Clamp(count, 1, 10);
        for (int i = 0; i < count; i++)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.magic);
            if (target != null && ObjectPool.Instance != null)
            {
                Vector3 pos = transform.GetChild(0).position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-1f, 1f));
                MagicSpawnEffect(pos);
                yield return new WaitForSeconds(0.2f);
                if(target!=null)
                {
                    GameObject bullet = ObjectPool.Instance.PopFromPool("FlamethrowerCartoonyFire");
                    bullet.GetComponent<bulletController>().damage = Damage();
                    bullet.GetComponent<bulletController>().isAlly = this.isPlayerHero;
                    bullet.GetComponent<bulletController>().Target = target.transform;
                    bullet.GetComponent<bulletController>().isCritical = isCriticalAttack;
                    bullet.transform.position = pos;
                    bullet.SetActive(true);
                }
            }
            yield return new WaitForSeconds(0.03f);
        }
        yield return null;
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
                bullet.GetComponent<bulletController>().damage = Damage()/ 2;
                bullet.GetComponent<bulletController>().isAlly = this.isPlayerHero;
                bullet.GetComponent<bulletController>().Target = target.transform;
                bullet.GetComponent<bulletController>().isCritical = isCriticalAttack;
                bullet.transform.position = transform.GetChild(0).position + new Vector3(0, 0.1f) + transform.right * -0.5f;
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
        animationTime += Time.deltaTime;
        if (animationTime >= standardTime)
        {
            StopAttack();
            animationTime = 0;
            standardTime = UnityEngine.Random.Range(3.0f, 5.0f);
            randomStatus = UnityEngine.Random.Range(0, 3);
            if (isHold)
                randomStatus = 3;
            switch (randomStatus)
            {
                case 0:
                    Idle();
                    break;
                case 1:
                    Run();
                    break;
                case 2:
                    Idle();
                    break;
                case 3:
                    Hold();
                    break;
            }
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

        if (!animator.GetBool("isAttack")&&!animator.GetBool("isSkill"))
        {
            animator.SetTrigger("defencing");
            faceAnimator.SetTrigger("Do");
            Knockback(1, 1, collision);
        }
        DamageCCUIShow("막음");

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
    public void Hitted(Collider2D collision, int dam, float kx, float ky)
    {
        if(!isDead && status.hp > 0)
        {
            dam = Common.GetDamage(dam, DEFENCE);
            Common.isHitShake = true;
            ChangingAttackMode();
            isUnBeat = true;
            if (collision.GetComponentInParent<Hero>() != null)
            {
                DamageUIShow(dam.ToString(), collision.GetComponentInParent<Hero>().isCriticalAttack);
                if (collision.GetComponentInParent<Hero>() != null && collision.GetComponentInParent<Hero>().isPlayerHero != isPlayerHero)
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
                if (!animator.GetBool("isAttack") && !animator.GetBool("isSkill"))
                    animator.SetTrigger("heating");
            }
            faceAnimator.SetTrigger("Hit");
            status.hp = Common.looMinus(status.hp, dam);
            StartCoroutine(UnBeatTime(dam));
        }
    }
    public void HittedByObject(int dam,bool isCritical, Vector2 addforce)
    {
        if(!isDead&&status.hp>0)
        {
            dam = Common.GetDamage(dam, DEFENCE);
            Common.isHitShake = true;
            ChangingAttackMode();
            isUnBeat = true;
            DamageUIShow(dam.ToString(), isCritical);
            if (!isStunning)
            {
                if (!animator.GetBool("isAttack") && !animator.GetBool("isSkill"))
                    animator.SetTrigger("heating");
            }
            faceAnimator.SetTrigger("Hit");
            status.hp = Common.looMinus(status.hp, dam);
            Vector2 attackedVelocity = isLeftorRight ? new Vector2(addforce.x, addforce.y) : new Vector2(-addforce.x, addforce.y);
            rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
            StartCoroutine(UnBeatTime(dam));
        }
    }
    public void Healing(int healAmount)
    {
        int maxHeal = status.maxHp - status.hp;
        healAmount = Mathf.Clamp(healAmount, 0, maxHeal);
        this.status.hp = Common.looHpPlus(this.status.hp, this.status.maxHp, healAmount);
        DamageCCUIShow("+" + healAmount.ToString());
        ShowHpBar(-healAmount);
    }
    public void Stunned()
    {
        if (!isStunning&&status.knockbackResist<10)
        {
            isStun = true;
            DamageCCUIShow("기절");
            StartCoroutine("Stunning");
        }
    }
    void EquipWeapon()
    {
        if (weaponPoint != null && weaponPoint.transform.childCount > 0 && !weaponPoint.transform.GetChild(0).gameObject.activeSelf)
        {
            animator.SetTrigger("changingWeapon");
            weaponPoint.transform.GetChild(0).gameObject.SetActive(true);
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
                GameObject heartPrefab = ObjectPool.Instance.PopFromPool("Heart");
                heartPrefab.GetComponent<Heart>().SetHeart(10);
                var findTarget = Common.FindAlly();
                findTarget.Sort((t1, t2) => t1.GetComponent<Hero>().status.hp.CompareTo(t2.GetComponent<Hero>().status.hp));
                heartPrefab.GetComponent<Heart>().MagnetTarget = findTarget[0];
                heartPrefab.transform.position = this.transform.position;
                heartPrefab.SetActive(true);
                heartPrefab.GetComponent<Rigidbody2D>().AddForce(new Vector3(UnityEngine.Random.Range(-1, 1), 5, 10), ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(0.1f);
            // 코인
            int coin = UnityEngine.Random.Range(15, 30);
            int coinRemain = coin % 100; // 80
            int coinCount = (coin-coinRemain) / 100; // 11
            int coinPart = 0;
            if (coinCount>0)
            {
                coinPart = (coin - coinRemain) / coinCount; // 100
                for (int i = 0; i < coinCount; i++)
                {
                    GameObject coinPrefab = ObjectPool.Instance.PopFromPool("Coin");
                    coinPrefab.GetComponent<Coin>().SetCoin(coinPart);
                    coinPrefab.transform.position = transform.position;
                    coinPrefab.SetActive(true);
                    coinPrefab.GetComponent<Rigidbody2D>().AddForce(new Vector3(UnityEngine.Random.Range(-1, 1), 5, 10), ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if(coinRemain>0)
            {
                GameObject coinRemainPrefab = ObjectPool.Instance.PopFromPool("Coin");
                coinRemainPrefab.GetComponent<Coin>().SetCoin(coinRemain);
                coinRemainPrefab.transform.position = transform.position;
                coinRemainPrefab.SetActive(true);
                coinRemainPrefab.GetComponent<Rigidbody2D>().AddForce(new Vector3(UnityEngine.Random.Range(-1, 1), 5, 10), ForceMode2D.Impulse);
                yield return new WaitForSeconds(0.1f);
            }
        }
        // 아이템 획득 파트
        Item randomItem = ItemSystem.GetRandomItem();
        if(UnityEngine.Random.Range(0,1000)<randomItem.droprate)
        {
            Debugging.Log(randomItem.droprate + " 의 확률수치의 " + randomItem.name + "의 아이템이 드랍되었습니다.");
            GameObject dropItem = ObjectPool.Instance.PopFromPool("dropItemPrefab");
            dropItem.transform.position = transform.position;
            dropItem.GetComponent<dropItemInfo>().dropItemID = randomItem.id;
            dropItem.SetActive(true);
            dropItem.GetComponent<dropItemInfo>().DropItem();
            StageManagement.instance.AddGetStageItem(randomItem.id);
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    #endregion

    #region 공통기능
    public void Chat(string chat)
    {
        Common.Chat(chat, transform);
    }
    #endregion

    #region 이펙트
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
        if(isPlayerHero)
        {
            SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.teleport);
            if (EffectPool.Instance != null)
            {
                GameObject effect = EffectPool.Instance.PopFromPool("SimplePortalRed");
                effect.transform.position = transform.GetChild(0).position;
                effect.SetActive(true);
            }
        }
    }
    public void LevelUpEffect()
    {
        SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.levelup);
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("LevelUpEffect");
            effect.transform.position = transform.GetChild(0).position;
            effect.SetActive(true);
        }
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
    public void HitEffect(Collider2D collider)
    {
        if (EffectPool.Instance != null)
        {
            GameObject effect = EffectPool.Instance.PopFromPool("Hit_white_Small");
            effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0.2f, 0);
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
                Hitted(collision, status.maxHp, 25f, 25f);
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
                if (UnityEngine.Random.Range(0, 10) >= 1 || isStunning || isAirborne)
                {
                    Hitted(collision, Convert.ToInt32(collision.name), 3f, 3f);
                    HitEffect(collision);

                }
                else
                {
                    Defence(collision);
                    GuardEffect(collision);
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
        SpawnEffect();
        RedirectCharacter();
        if (initChats.Count > 0)
            Common.Chat(initChats[UnityEngine.Random.Range(0, initChats.Count)], transform);
        yield return new WaitForSeconds(2);
        SpriteAlphaSetting(1);
        yield return new WaitForSeconds(1);
        EquipWeapon();
        yield return new WaitForSeconds(2);
        heroState = HeroState.Attack;
        isStart = true;
        yield return new WaitForSeconds(3.0f);
        FindEnemys(true);
        yield return null;
    }
    IEnumerator OnAttackPoint(int totalCount=1)
    {
        attackPoint.name = this.Damage().ToString();
        totalCount = Mathf.Clamp(totalCount, 1, 10);
        int cnt = 0;
        while(cnt<totalCount)
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
            switch(magicType)
            {
                case 0:
                    GameObject magic = ObjectPool.Instance.PopFromPool("MagicStone");
                    magic.transform.position = target.transform.position + new Vector3(0, 5);
                    MagicCircleEffect(magic.transform.position + new Vector3(0, 1), 3);
                    if (magic.GetComponent<DecompositionObject>() != null)
                    {
                        magic.GetComponent<DecompositionObject>().TargetTag = isPlayerHero ? "Enemy" : "Hero";
                        magic.GetComponent<DecompositionObject>().Damage = Damage() * 2;
                        magic.GetComponent<DecompositionObject>().isCritical = isCriticalAttack;
                        magic.GetComponent<DecompositionObject>().isRolling = true;
                    }
                    magic.SetActive(true);
                    break;
                case 1:
                    SoundManager.instance.EffectSourcePlay(AudioClipManager.instance.smoke);
                    List<GameObject> targetList = enemys;
                    targetList  = targetList.FindAll(x=> Common.GetDistanceBetweenAnother(this.transform, x.transform) < 5);
                    if(targetList.Count>0)
                    {
                        for(int i = 0; i < targetList.Count; i++)
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
                                targetList[i].GetComponent<Hero>().HittedByObject((Damage()*2/targetList.Count), isCriticalAttack, new Vector2(5f, 5f));
                            }
                        }
                    }
                    else if(Common.hitTargetObject!=null)
                    {
                        yield return new WaitForEndOfFrame();
                        GameObject skillEffect = EffectPool.Instance.PopFromPool("LightningStrikeTall", this.transform);
                        skillEffect.transform.position = Common.hitTargetObject.transform.position;
                        skillEffect.SetActive(true);
                        skillEffect.GetComponent<ParticleSystem>().Play();
                        Common.hitTargetObject.GetComponent<Castle>().HittedByObject((Damage()), isCriticalAttack, new Vector2(5f, 5f));
                    }
                    break;
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
        yield return new WaitForSeconds(3.0f);
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
        int cnt = 0;
        while (cnt < 1)
        {
            isLeftorRight = tPos.x < transform.position.x ? true : false;
            yield return new WaitForSeconds(0.5f);
            cnt++;
        }
        isTrack = false;
        yield return null;
    }
    IEnumerator Stunning()
    {
        isStunning = true;
        Knockback(10, 1);
        animator.SetTrigger("stunning");
        animator.SetBool("isStun", true);
        faceAnimator.SetTrigger("Hit");
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        while (isStunning)
        {
            yield return new WaitForSeconds(3.0f);
            isStun = false;
            isStunning = false;
        }
        animator.SetBool("isStun", false);
        yield return null;
    }
    IEnumerator Attacking()
    {
        isAttack = true;
        attackNumber = UnityEngine.Random.Range(0, 5);
        isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
        RedirectCharacter();
        animator.SetInteger("attackNumber", attackNumber);
        animator.SetFloat("speed", 0.8f + this.status.attackSpeed*2f);
        faceAnimator.SetTrigger("Do");
        animator.SetBool("isAttack", true);
        animator.SetTrigger("attacking");
        if (playChats.Count > 0 && UnityEngine.Random.Range(0, 5)<1)
            Common.Chat(playChats[UnityEngine.Random.Range(0, playChats.Count)], transform);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        StopAttack();
        yield return null;
    }
    IEnumerator SkillAttacking()
    {
        isSkillAttack = true;
        animator.SetBool("isSkill", true);
        animator.SetTrigger("skillAttacking");
        animator.SetInteger("skillType", 0);
        yield return new WaitForSeconds(2.0f);
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
        attackNumber = UnityEngine.Random.Range(0, 5);
        animator.SetInteger("attackNumber", attackNumber);
        animator.SetFloat("speed", 1.0f);
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
    #endregion

    #region 유저인터페이스
    public void DamageUIShow(string font, bool isCritical = false)
    {
        GameObject damageUIprefab = ObjectPool.Instance.PopFromPool("damageUI", GameObject.Find("CanvasUI").transform) as GameObject;
        damageUIprefab.GetComponentInChildren<Text>().text = font.ToString();
        damageUIprefab.GetComponent<TextDamageController>().isLeft = !isLeftorRight;
        damageUIprefab.GetComponent<TextDamageController>().isCritical = isCritical;
        damageUIprefab.GetComponent<TextDamageController>().isCC = false;
        damageUIprefab.transform.position = transform.position + new Vector3(0, 1);
        damageUIprefab.SetActive(true);
    }
    public void DamageCCUIShow(string font)
    {
        GameObject damageUIprefab = ObjectPool.Instance.PopFromPool("damageUI", GameObject.Find("CanvasUI").transform) as GameObject;
        damageUIprefab.GetComponentInChildren<Text>().text = font.ToString();
        damageUIprefab.GetComponent<TextDamageController>().isLeft = !isLeftorRight;
        damageUIprefab.GetComponent<TextDamageController>().isCritical = false;
        damageUIprefab.GetComponent<TextDamageController>().isCC = true;
        damageUIprefab.transform.position = transform.position;
        damageUIprefab.SetActive(true);
    }
    private void OpenHpBar(bool isBlue = false)
    {
        hpUI = ObjectPool.Instance.PopFromPool("hpEnemyUI");
        hpUI.GetComponent<UI_hp>().OpenHpUI(this.gameObject, isBlue);
        hpUI.gameObject.SetActive(true);
    }
    private void ShowHpBar(int dam=0)
    {
        if (!isDead && status.hp > 0)
        {
            if (!hpUI.gameObject.activeSelf)
            {
                hpUI.GetComponent<UI_hp>().panelHpTime = 0;
                hpUI.gameObject.SetActive(true);
            }
            if(dam>0)
                hpUI.GetComponent<UI_hp>().GetDamage(dam);
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
        [Range(1, 9999)]
        public int attack;
        [Range(0, 1000)]
        public int defence;
        [Range(0, 9999)]
        public int hp;
        [Range(10, 1000)]
        public int skillEnegry;
        [HideInInspector]
        [Range(0, 9999)]
        public int maxHp;
        [Range(0, 100)]
        public int criticalPercent;
        [Range(0.05f,2)]
        public float attackSpeed;
        [Range(0.1f, 3)]
        public float moveSpeed;
        [Range(0, 10)]
        public float knockbackResist;
        public Status() { level = 1; exp = 0;}

        public void SetHeroStatus(HeroData data)
        {
            this.level = data.level;
            this.exp = data.exp;
            this.attack = HeroSystem.GetHeroStatusAttack(data);
            this.defence = HeroSystem.GetHeroStatusDefence(data);
            this.maxHp = HeroSystem.GetHeroStatusMaxHp(data);
            this.hp = this.maxHp;
            this.criticalPercent = HeroSystem.GetHeroStatusCriticalPercent(data);
            this.attackSpeed = HeroSystem.GetHeroStatusAttackSpeed(data);
            this.moveSpeed = HeroSystem.GetHeroStatusMoveSpeed(data);
            this.knockbackResist = HeroSystem.GetHeroStatusKnockbackResist(data);
            this.skillEnegry = 20 - HeroSystem.GetHeroStatusSkillEnergy(data);

            Debugging.Log(string.Format("{0} => 공격력:{1} 방어력:{2} 체력:{3} 크리티컬:{4} 공격속도:{5} 이동속도:{6} 넉백:{7} 에너지효율:{8}", data.name, this.attack, this.defence, this.maxHp, this.criticalPercent, this.attackSpeed, this.moveSpeed, this.knockbackResist, this.skillEnegry));
        }
    }

    public void LevelUp()
    {
        if (status.exp >= Common.EXP_TABLE[status.level - 1] && status.level < 10&&!isDead)
        {
            LevelUpEffect();
            HeroSystem.LevelUpStatusSet(this.id,this);
            if (hpUI != null)
            {
                hpUI.GetComponent<UI_hp>().levelUI.GetComponentInChildren<Text>().text = status.level.ToString();
                if (status.level < 10)
                    hpUI.GetComponent<UI_hp>().levelUI.GetComponentInChildren<Slider>().value = (float)status.exp / (float)Common.EXP_TABLE[status.level - 1];
                else
                    hpUI.GetComponent<UI_hp>().levelUI.GetComponentInChildren<Slider>().value = 0;
                ShowHpBar();
            }
            Debugging.Log(name + " 의 레벨업 ! >> " + status.level);
        }
    }

    public void ExpUp(int nExp)
    {
        if (status!=null&&status.level<10 && !isDead)
        {
            status.exp += nExp;
            HeroSystem.SetHero(this);
            hpUI.GetComponent<UI_hp>().levelUI.GetComponentInChildren<Slider>().value = (float)status.exp / (float)Common.EXP_TABLE[(status.level-1)];
            LevelUp();
        }
    }
    #endregion
}
