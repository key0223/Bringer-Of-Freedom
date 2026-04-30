using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using static Define;
using static EnemyDefine;
using Random = UnityEngine.Random;

public class MainMonsterController : MonoBehaviour
{
    CameraController cameraController;

    PlayerMovement playerMovement;
    MonsterAnimation monsterAnimation;
    YiSunShinSkillController skillController;
    YiSunShinPhaseManager phaseManager;

    RigBuilder rigBuilder;

    List<DragonHealth> dragonHealthList = new List<DragonHealth>();

    /* Dragon State */
    [SerializeField] EnemyState bodyState = EnemyState.IDLE;
    [SerializeField] GreenDragonState greenState = GreenDragonState.IDLE;
    [SerializeField] BlackDragonState blackState = BlackDragonState.IDLE;
    [SerializeField] RedDragonState redState = RedDragonState.IDLE;
    [SerializeField] BlueDragonState blueState = BlueDragonState.IDLE;
    [SerializeField] YellowDragonState yellowState = YellowDragonState.IDLE;

    [Header("Yellow Groggy")]
    [SerializeField] float currentGroggyPoint = 0;
    float maxGroggyPoint = 100f;

    [Header("Test")]
    [SerializeField] bool isTest = false;
    [SerializeField] bool receiveInput = false;
    [SerializeField] bool showGizmo = false;
    [Space(5f)]
    [SerializeField] DragonType damageDragon;
    [SerializeField] int testDamage;

    [Header("Common Settings")]
    [SerializeField] Transform target;
    [SerializeField] Transform attackTarget;
    [SerializeField] Transform greenTransform;
    [Space(5f)]
    [SerializeField] MeshCollider greenMeshColl;
    [SerializeField] MeshCollider blackMeshColl;
    [SerializeField] MeshCollider redMeshColl;
    [SerializeField] MeshCollider blueMeshColl;
    [Space(5f)]
    [SerializeField] Mesh greenMesh;
    [SerializeField] Mesh blackMesh;
    [SerializeField] Mesh redMesh;
    [SerializeField] Mesh blueMesh;
    [Space(5f)]
    [SerializeField] GameObject hatPrefab;
    [Space(5f)]
    [SerializeField] GameObject greenBall;
    [SerializeField] GameObject blackBall;
    [SerializeField] GameObject blue1Ball;
    [SerializeField] GameObject blue2Ball;
    [SerializeField] GameObject YellowBall;

    [Header("임시 투명화")]
    [SerializeField] SkinnedMeshRenderer greenMeshRenderer;
    [SerializeField] SkinnedMeshRenderer blackMeshRenderer;
    [SerializeField] SkinnedMeshRenderer redMeshRenderer;
    [SerializeField] SkinnedMeshRenderer blueMeshRenderer;
    [SerializeField] SkinnedMeshRenderer yellowMeshRenderer;
    [SerializeField] SkinnedMeshRenderer yellowMustacheRenderer;

    [Space(10f)]
    [SerializeField] bool playerGrounded = false;
    [SerializeField] bool isPlayerBehind = false;

    [Space(10f)]
    [Header("Follow Settings")]
    [SerializeField] float followRange = 100f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float viewAngle = 45;
    [SerializeField] float viewRange = 250f;
    [SerializeField] float rotationSpeed = 8f;
    [SerializeField] bool lockWhileMoving = true;
    [SerializeField] bool lockWhileAttacking = true;
    bool canMove = true;


    [Header("Sound")]
    [SerializeField] List<AudioClip> idleClips;
    [SerializeField] Vector2 intervalRange = new Vector2(6f, 14f);

    private Coroutine coIdleSound;
    #region Animation Parameters
    // Animation
    Animator anim;
    bool isMoving = false;
    bool isStomping = false;
    bool isSwordAttacking = false;

    bool isGreenPoisonBreathing = false;
    bool isPouncing = false;

    bool isShootingFireball = false;
    bool isPunching = false;

    bool isIceBreathing = false;
    bool isWaterBreathing = false;

    bool isLaserBeaming = false;
    bool isHeadAttack = false;
    bool isRadiateAttack = false;
    bool isPerformingLightOfJudgment = false;
    bool isPerformingLightningStrike = false;

    bool isSwordAttacking_Gimmick = false;
    bool isPoisonAttacking_Gimmick = false;

    bool isBlackDown = false;
    bool isBlueAwake = false;
    bool isYellowAwake = false;
    bool isYellowDown = false;

    bool isGreenDead = false;
    bool isBlackDead = false;
    bool isBlueDead = false;
    bool isRedDead = false;
    bool isYellowDead = false;
    bool isYiSunShinDead = false;
    #endregion

    #region Properties
    public EnemyState BodyState
    {
        get { return bodyState; }
        set
        {
            if (bodyState == value) return;

            bodyState = value;
            OnStateChanged(bodyState);
        }
    }
    public GreenDragonState GreenState { get { return greenState; } set { greenState = value; } }
    public BlackDragonState BlackState { get { return blackState; } set { blackState = value; } }
    public RedDragonState RedState { get { return redState; } set { redState = value; } }
    public BlueDragonState BlueState { get { return blueState; } set { blueState = value; } }
    public YellowDragonState YellowState { get { return yellowState; } set { yellowState = value; } }

    public RigBuilder RigBuilder { get { return rigBuilder; } }
    public PlayerMovement PlayerMove { get { return playerMovement; } }
    public MonsterAnimation MonsterAnim { get { return monsterAnimation; } }
    public YiSunShinSkillController SkillController { get { return skillController; } }
    public YiSunShinPhaseManager PhaseManager { get { return phaseManager; } }
    public List<DragonHealth> DragonHealthList { get { return dragonHealthList; } }
    public Transform Target { get { return target; } }
    public Transform AttackTarget { get { return attackTarget; } }
    public bool IsTest { get { return isTest; } }
    public bool ReceiveInput { get { return receiveInput; } }
    public bool PlayerGrounded { get { return playerGrounded; } }
    public bool IsPlayerBehind { get { return isPlayerBehind; } }
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    // Animation
    public Animator Anim { get { return anim; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
    public bool IsStomping { get { return isStomping; } set { isStomping = value; } }
    public bool IsSwordAttacking { get { return isSwordAttacking; } set { isSwordAttacking = value; } }
    public bool IsGreenPoisonBreathing { get { return isGreenPoisonBreathing; } set { isGreenPoisonBreathing = value; } }
    public bool IsPouncing { get { return isPouncing; } set { isPouncing = value; } }
    public bool IsShootingFireball { get { return isShootingFireball; } set { isShootingFireball = value; } }
    public bool IsPunching { get { return isPunching; } set { isPunching = value; } }
    public bool IsIceBreathing { get { return isIceBreathing; } set { isIceBreathing = value; } }
    public bool IsWaterBreathing { get { return isWaterBreathing; } set { isWaterBreathing = value; } }
    public bool IsLaserBeaming { get { return isLaserBeaming; } set { isLaserBeaming = value; } }
    public bool IsHeadAttack { get { return isHeadAttack; } set { isHeadAttack = value; } }
    public bool IsRadiateAttack { get { return isRadiateAttack; } set { isRadiateAttack = value; } }
    public bool IsPerformingLightOfJudgment { get { return isPerformingLightOfJudgment; } set { isPerformingLightOfJudgment = value; } }
    public bool IsPerformingLightningStrike { get { return isPerformingLightningStrike; } set { isPerformingLightningStrike = value; } }
    public bool IsSwordAttacking_Gimmick { get { return isSwordAttacking_Gimmick; } set { isSwordAttacking_Gimmick = value; } }
    public bool IsPoisonAttacking_Gimmick { get { return isPoisonAttacking_Gimmick; } set { isPoisonAttacking_Gimmick = value; } }
    public bool IsBlackDown { get { return isBlackDown; } set { isBlackDown = value; } }
    public bool IsBlueAwake { get { return isBlueAwake; } set { isBlueAwake = value; } }
    public bool IsYellowAwake { get { return isYellowAwake; } set { isYellowAwake = value; } }
    public bool IsYellowDown { get { return isYellowDown; } set { isYellowDown = value; } }
    public bool IsGreenDead { get { return isGreenDead; } set { isGreenDead = value; } }
    public bool IsBlackDead { get { return isBlackDead; } set { isBlackDead = value; } }
    public bool IsBlueDead { get { return isBlueDead; } set { isBlueDead = value; } }
    public bool IsRedDead { get { return isRedDead; } set { isRedDead = value; } }
    public bool IsYellowDead { get { return isYellowDead; } set { isYellowDead = value; } }
    public bool IsYiSunShinDead { get { return isYiSunShinDead; } set { isYiSunShinDead = value; } }


    public void SetAllSkillFalse()
    {
        isStomping = false;
        IsSwordAttacking = false;
        isGreenPoisonBreathing = false;
        isPouncing = false;
        isShootingFireball = false;
        isIceBreathing = false;
        isWaterBreathing = false;
        isLaserBeaming = false;
        isHeadAttack = false;
        isRadiateAttack = false;
        isPerformingLightOfJudgment = false;
        isPerformingLightningStrike = false;
        isSwordAttacking_Gimmick = false;
        isPoisonAttacking_Gimmick = false;
        IsBlackDown = false;
        isBlueAwake = false;
        isYellowAwake = false;
        isGreenDead = false;
        isBlackDead = false;
        isBlueDead = false;
        isYellowDead = false;
    }
    #endregion

    public void TestDamage()
    {
        OnDamageDragon(damageDragon, testDamage);
    }
    void Awake()
    {
        cameraController = FindAnyObjectByType<CameraController>();
        monsterAnimation = GetComponent<MonsterAnimation>();
        skillController = GetComponent<YiSunShinSkillController>();
        phaseManager = GetComponentInChildren<YiSunShinPhaseManager>();

        Init_DragonHealth();

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerMovement = FindObjectOfType<PlayerMovement>();
        anim = GetComponent<Animator>();

        monsterAnimation.Init(this);
        skillController.Init(this);

        rigBuilder = GetComponent<RigBuilder>();
        Init_IK();

        //attackTarget.gameObject.SetActive(false);
        attackTarget.transform.SetParent(null, false);
    }
    void Start()
    {
        //UIManager.Instance.Hud.DragonHealthUI.Init_DragonHealthUI();
        phaseManager.OnPhaseChanged -= OnPhaseChanged;
        phaseManager.OnPhaseChanged += OnPhaseChanged;
    }

    void OnEnable()
    {
        phaseManager.OnPhaseChanged -= OnPhaseChanged;
        phaseManager.OnPhaseChanged += OnPhaseChanged;
        TryStart();
    }
    void OnDisable()
    {
        phaseManager.OnPhaseChanged -= OnPhaseChanged;
        TryStop();
    }

    void Update()
    {
        if (phaseManager.CurrentPhaseState != PhaseState.IDLE || yellowState == YellowDragonState.GROGGY) return;

        UpdateState();
        CheckPlayerGrounded();
        CheckPlayerDir();
        CallDragonHealthUpdate();
    }
    void Init_DragonHealth()
    {
        dragonHealthList.Add(new DragonHealth(this, DragonType.DRAGON_GREEN, 100f));
        dragonHealthList.Add(new DragonHealth(this, DragonType.DRAGON_RED, 100f));
        dragonHealthList.Add(new DragonHealth(this, DragonType.DRAGON_BLUE, 150f));
        dragonHealthList.Add(new DragonHealth(this, DragonType.DRAGON_BLACK, 100f));
        dragonHealthList.Add(new DragonHealth(this, DragonType.DRAGON_YELLOW, 200f));

        for (int i = 0; i < dragonHealthList.Count; i++)
        {
            dragonHealthList[i].OnDragonDeath += OnDragonDeath;
            dragonHealthList[i].OnDragonDeath += phaseManager.OnDragonPartDead;
        }
    }

    void Init_IK()
    {
        /* Target = 0 , Attack Target : 1 , */
        var sources = new WeightedTransformArray();
        sources.Add(new WeightedTransform(target, 1));
        sources.Add(new WeightedTransform(attackTarget, 0));
        skillController.GreenMPos.data.sourceObjects = sources;
        skillController.GreenMRot.data.sourceObjects = sources;

        skillController.RedMPos.data.sourceObjects = sources;
        skillController.RedMRot.data.sourceObjects = sources;

        skillController.BlueRightMPos.data.sourceObjects = sources;
        skillController.BlueRightMRot.data.sourceObjects = sources;

        skillController.BlueLeftMPos.data.sourceObjects = sources;
        skillController.BlueLeftMRot.data.sourceObjects = sources;

        skillController.BlueLeftMPos.data.sourceObjects = sources;
        skillController.BlueLeftMRot.data.sourceObjects = sources;

        skillController.YellowMPos.data.sourceObjects = sources;
        skillController.YellowMRot.data.sourceObjects = sources;

        var blackSources = new WeightedTransformArray();
        blackSources.Add(new WeightedTransform(greenTransform, 1));

        skillController.BlackMPos.data.sourceObjects = blackSources;
        skillController.BlackMRot.data.sourceObjects = blackSources;

        /* 1페이즈 활성화 용 제외 리그 0 세팅 */

        rigBuilder.layers[2].rig.weight = 0;
        rigBuilder.layers[3].rig.weight = 0;
        rigBuilder.layers[4].rig.weight = 0;
        rigBuilder.layers[5].rig.weight = 0;

        rigBuilder.Build();

        skillController.BlueRightRig.SetActive(false);
        skillController.BlueLeftRig.SetActive(false);
        skillController.YellowRig.SetActive(false);
    }

    public bool IsDragonPartAttacking(DragonType type)
    {
        switch (type)
        {
            case DragonType.DRAGON_GREEN: return BlackState == BlackDragonState.ATTACK;
            case DragonType.DRAGON_BLACK: return GreenState == GreenDragonState.ATTACK;
            case DragonType.DRAGON_RED: return RedState == RedDragonState.ATTACK;
            case DragonType.DRAGON_BLUE: return BlueState == BlueDragonState.ATTACK;
            case DragonType.DRAGON_YELLOW: return YellowState == YellowDragonState.ATTACK;
            case DragonType.BODY: return BodyState == EnemyState.ATTACK;
            default: return false;
        }
    }

    public bool IsDragonPartIdle(DragonType type)
    {
        switch (type)
        {
            case DragonType.DRAGON_GREEN: return BlackState == BlackDragonState.IDLE;
            case DragonType.DRAGON_BLACK: return GreenState == GreenDragonState.IDLE;
            case DragonType.DRAGON_RED: return RedState == RedDragonState.IDLE;
            case DragonType.DRAGON_BLUE: return BlueState == BlueDragonState.IDLE;
            case DragonType.DRAGON_YELLOW: return YellowState == YellowDragonState.IDLE;
            case DragonType.BODY: return BodyState == EnemyState.IDLE;
            default: return false;
        }
    }
    void UpdateState()
    {
        switch (bodyState)
        {
            case EnemyState.IDLE:
                UpdateIdle();
                break;
            case EnemyState.MOVE:
                UpdateMove();
                break;
            case EnemyState.ATTACK:
                break;
        }
    }

    void OnStateChanged(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.IDLE:
                break;
            case EnemyState.ATTACK:
                break;
            case EnemyState.MOVE:
                break;
            case EnemyState.GIMMICK:
                break;
        }
    }

    void OnPhaseChanged(GamePhase currentPhase)
    {
        switch (currentPhase)
        {
            case GamePhase.PHASE_1:
                {
                    rigBuilder.layers[0].rig.weight = 1;
                    rigBuilder.layers[1].rig.weight = 1;
                    rigBuilder.layers[2].rig.weight = 0;
                    rigBuilder.layers[3].rig.weight = 0;
                    rigBuilder.layers[4].rig.weight = 1;
                    rigBuilder.layers[5].rig.weight = 0;
                }
                break;
            case GamePhase.PHASE_2:
                {
                    rigBuilder.layers[0].rig.weight = 0;
                    rigBuilder.layers[1].rig.weight = 1;
                    rigBuilder.layers[2].rig.weight = 1;
                    rigBuilder.layers[3].rig.weight = 1;
                    rigBuilder.layers[4].rig.weight = 0;
                    rigBuilder.layers[5].rig.weight = 0;

                    skillController.BlueRightRig.SetActive(true);
                    skillController.BlueLeftRig.SetActive(true);
                }
                break;
            case GamePhase.PHASE_3:
                {
                    rigBuilder.layers[0].rig.weight = 0;
                    rigBuilder.layers[1].rig.weight = 0;
                    rigBuilder.layers[2].rig.weight = 0;
                    rigBuilder.layers[3].rig.weight = 0;
                    rigBuilder.layers[4].rig.weight = 0;
                    rigBuilder.layers[5].rig.weight = 1;

                    skillController.YellowRig.SetActive(true);
                }
                break;

        }
    }
    public void SetDragonState(DragonType dragonType, System.Object state)
    {
        if (!IsDragonAlive(dragonType)) return;

        switch (dragonType)
        {
            case DragonType.DRAGON_GREEN: GreenState = (GreenDragonState)state; break;
            case DragonType.DRAGON_BLACK: BlackState = (BlackDragonState)state; break;
            case DragonType.DRAGON_RED: RedState = (RedDragonState)state; break;
            case DragonType.DRAGON_BLUE: BlueState = (BlueDragonState)state; break;
            case DragonType.DRAGON_YELLOW: YellowState = (YellowDragonState)state; break;
        }
    }
    public void SetOwnerStatesOnSkillStart(EnemySkillType skillType)
    {
        BodyState = EnemyState.ATTACK;

        if (SkillController.TryGetRequiredDragon(skillType, out DragonType? dragonType) && dragonType.HasValue)
        {
            switch (dragonType.Value)
            {
                case DragonType.DRAGON_GREEN: SetDragonState(dragonType.Value, GreenDragonState.ATTACK); break;
                case DragonType.DRAGON_BLACK: SetDragonState(dragonType.Value, BlackDragonState.ATTACK); break;
                case DragonType.DRAGON_RED: SetDragonState(dragonType.Value, RedDragonState.ATTACK); break;
                case DragonType.DRAGON_BLUE: SetDragonState(dragonType.Value, BlueDragonState.ATTACK); break;
                case DragonType.DRAGON_YELLOW: SetDragonState(dragonType.Value, YellowDragonState.ATTACK); break;
            }
        }
    }
    public void SetOwnerStatesOnSkillEnd(EnemySkillType skillType)
    {
        BodyState = EnemyState.IDLE;
        if (SkillController.TryGetRequiredDragon(skillType, out DragonType? dragonType) && dragonType.HasValue)
        {
            switch (dragonType.Value)
            {
                case DragonType.DRAGON_GREEN: SetDragonState(dragonType.Value, GreenDragonState.IDLE); break;
                case DragonType.DRAGON_BLACK: SetDragonState(dragonType.Value, BlackDragonState.IDLE); break;
                case DragonType.DRAGON_RED: SetDragonState(dragonType.Value, RedDragonState.IDLE); break;
                case DragonType.DRAGON_BLUE: SetDragonState(dragonType.Value, BlueDragonState.IDLE); break;
                case DragonType.DRAGON_YELLOW: SetDragonState(dragonType.Value, YellowDragonState.IDLE); break;
            }
        }
    }
    void UpdateIdle()
    {
        // TODO : 플레이어 사망 시 대기
        if (target == null || !canMove || bodyState == EnemyState.DEAD) return;

        RotateTowardTargetIfOutOfView();

        float distance = (target.position - transform.position).magnitude;

        if (distance > followRange)
        {
            BodyState = EnemyState.MOVE;
        }

        if (isTest && receiveInput) return;
        if (phaseManager == null || phaseManager.GimmickInProgress) return;
    }
    void UpdateMove()
    {
        if (target == null || !canMove || bodyState == EnemyState.DEAD) return;

        isMoving = true;

        Vector3 horizontalDir = target.position - transform.position;
        horizontalDir.y = 0;
        float horizontalDistance = horizontalDir.magnitude;
        Vector3 dir = horizontalDir.sqrMagnitude > 0.0001f ? horizontalDir.normalized : Vector3.zero;


        RotateTowardTargetIfOutOfView(dir);

        // 커브 값 읽기
        float drive = Mathf.Clamp01(anim.GetFloat("moveCurve"));

        Vector3 fwd = transform.forward;
        fwd.y = 0;
        fwd.Normalize();

        if (horizontalDistance >= followRange)
        {
            transform.position += fwd * moveSpeed * drive * Time.deltaTime;
        }
        else
        {
            BodyState = EnemyState.IDLE;
            isMoving = false;

        }

        if (isTest && receiveInput) return;
        if (phaseManager == null || phaseManager.GimmickInProgress) return;

        //skillController.SelectSkillByPriority();
    }
    void RotateTowardTargetIfOutOfView()
    {
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.0001f) return;
        RotateTowardTargetIfOutOfView(toTarget.normalized);
    }

    void RotateTowardTargetIfOutOfView(Vector3 dir)
    {
        // 현재 전방과 목표 각도
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle <= viewAngle) return;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRot, rotationSpeed * Time.deltaTime
        );
    }
    void CallDragonHealthUpdate()
    {
        foreach (DragonHealth dragonHealth in dragonHealthList)
        {
            if (dragonHealth.IsDead) continue;
            dragonHealth.Update();
        }
    }

    public void AddMonsterDebuff(DragonType type, Debuff debuff)
    {
        GetDragonHealth(type).AddDebuff(debuff);
    }
    public void OnDamageDragon(DragonType type, float damage)
    {
        if (phaseManager.GimmickInProgress)
        {
            SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_NoDamage_Hit);
            return;
        }

        // TODO : Debuff 있는지 확인하고 적용

        int rand = Random.Range(0, 2);

        SoundEffect hitType = rand == 0 ? SoundEffect.Enemy_Hit2 : SoundEffect.Enemy_Hit3;
        SoundManager.Instance?.PlaySFX(hitType);

        GetDragonHealth(type).TakeDamage(type, damage);

        if (phaseManager.CurrentPhase == GamePhase.PHASE_3 && type == DragonType.DRAGON_YELLOW)
        {
            if (yellowState == YellowDragonState.GROGGY) return;

            float addPoint = damage * phaseManager.YellowGroggyRatio;
            currentGroggyPoint += addPoint;

            if (currentGroggyPoint >= maxGroggyPoint)
            {
                bodyState = EnemyState.IDLE;
                isMoving = false;

                isYellowDown = true;
                yellowState = YellowDragonState.GROGGY;
                phaseManager.YellowWeaknessHitBox.gameObject.SetActive(true);

                Invoke("SetYellowRecover", 12f);
            }
        }
    }
    public void OnDragonDeath(DragonType type, DragonHealth deadDragon)
    {
        deadDragon.OnDragonDeath -= OnDragonDeath;
        SetDragonDead(type);

        phaseManager.NotifyDragonDeath(type, deadDragon);
    }

    public bool IsDragonAlive(DragonType type)
    {
        DragonHealth dragon = GetDragonHealth(type);
        return dragon != null && dragon.CurrentHp > 0f;
    }
    public DragonHealth GetDragonHealth(DragonType type)
    {
        foreach (DragonHealth dragon in dragonHealthList)
        {
            if (dragon.DragonType == type)
                return dragon;
        }
        return null;
    }

    void SetDragonDead(DragonType dragonType)
    {
        switch (dragonType)
        {
            case DragonType.DRAGON_GREEN:
                {
                    isGreenDead = true;
                    RigLayer layer = rigBuilder.layers[0];
                    float start = layer.rig.weight;
                    StartCoroutine(BlendRigWeightOverTime(layer, start, endWeight: 0, duration: 0.3f, active: false));
                    GreenState = GreenDragonState.DEAD;
                    greenMeshColl.sharedMesh = greenMesh; // Collider
                    greenBall.gameObject.SetActive(false); // Dragon ball
                    /* Material */
                    greenMeshRenderer.materials = skillController.GreenSettings.greenDeadMat;
                    skillController.GreenRig.SetActive(false);
                }
                break;
            case DragonType.DRAGON_RED:
                {
                    isRedDead = true;
                    RigLayer layer = rigBuilder.layers[1];
                    float start = layer.rig.weight;
                    StartCoroutine(BlendRigWeightOverTime(layer, start, endWeight: 0, duration: 0.3f, active: false));
                    RedState = RedDragonState.DEAD;
                    redMeshColl.sharedMesh = redMesh;
                    
                    /* Material */
                    redMeshRenderer.materials = skillController.RedSettings.redDeadMat;
                    skillController.RedRig.SetActive(false);

                }
                break;
            case DragonType.DRAGON_BLUE:
                {
                    isBlueDead = true;
                    RigLayer iceLayer = rigBuilder.layers[2];
                    float iceStart = iceLayer.rig.weight;
                    StartCoroutine(BlendRigWeightOverTime(iceLayer, iceStart, endWeight: 0, duration: 0.3f, active: false));

                    RigLayer waterLayer = rigBuilder.layers[3];
                    float waterStart = waterLayer.rig.weight;
                    StartCoroutine(BlendRigWeightOverTime(waterLayer, waterStart, endWeight: 0, duration: 0.3f, active: false));
                    BlueState = BlueDragonState.DEAD;
                    blueMeshColl.sharedMesh = blueMesh;

                    blue1Ball.gameObject.SetActive(false);
                    blue2Ball.gameObject.SetActive(false);
                    /* Material */
                    blueMeshRenderer.materials = skillController.BlueSettings.blueDeadMat;
                }
                break;
            case DragonType.DRAGON_BLACK:
                {
                    isBlackDead = true;
                    RigLayer layer = rigBuilder.layers[4];
                    float start = layer.rig.weight;
                    StartCoroutine(BlendRigWeightOverTime(layer, start, endWeight: 0, duration: 0.3f, active: false));
                    BlackState = BlackDragonState.DEAD;
                    blackMeshColl.sharedMesh = blackMesh;

                    blackBall.transform.SetParent(null, true);
                    Rigidbody rigid = blackBall.GetComponent<Rigidbody>();
                    rigid.useGravity = true;
                    /* Material */
                    blackMeshRenderer.materials = skillController.BodySettings.bodyDeadMat; // 임시로 Body에 매터리얼 추가해놨음
                    skillController.BlackRig.SetActive(false);

                }
                break;
            case DragonType.DRAGON_YELLOW:
                {
                    isYellowDead = true;
                    RigLayer layer = rigBuilder.layers[5];
                    float start = layer.rig.weight;
                    StartCoroutine(BlendRigWeightOverTime(layer, start, endWeight: 0, duration: 0.3f, active: false));
                    yellowState = YellowDragonState.DEAD;

                    isYiSunShinDead = true;
                    bodyState = EnemyState.DEAD;
                    skillController.YellowRig.SetActive(false);
                }
                break;
        }
    }



    /* Called by animation event */
    public void OnYiSunShinDead()
    {
        StartCoroutine(CoYiSunShinDead());
    }
    IEnumerator CoYiSunShinDead()
    {
        // TODO : 이순신 발 딛으면 용 투명화

        greenMeshRenderer.enabled = false;
        blackMeshRenderer.enabled = false;
        redMeshRenderer.enabled = false;
        blueMeshRenderer.enabled = false;
        yellowMeshRenderer.enabled = false;
        yellowMustacheRenderer.enabled = false;

        // TODO : 투구 떨어짐 

        GameObject hat = Instantiate(hatPrefab, skillController.YellowSettings.yellowAttackPoint.position, Quaternion.identity);

        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(GameMessages.MESSAGE_INTERACTION_HAT);


        // TODO : 투구 상호작용


        yield return null;
    }

    void CheckPlayerGrounded()
    {
        if (playerMovement == null) return;

        playerGrounded = playerMovement.isGrounded;
    }
    void CheckPlayerDir()
    {
        if (playerMovement == null) return;
        Vector3 dir = target.position - transform.position;
        float dot = Vector3.Dot(transform.forward, dir);

        if (dot > 0) // in front of this object
            isPlayerBehind = false;
        else isPlayerBehind = true; // behind this object
    }
    /* Called by OnDamageDragon */
    void SetYellowRecover()
    {
        currentGroggyPoint = 0;
        isYellowDown = false;
        yellowState = YellowDragonState.IDLE;
        phaseManager.YellowWeaknessHitBox.gameObject.SetActive(false);
    }
    public IEnumerator BlendWeightOverTime<P, R>(P pos, R rot, Transform target, float startWeight, float endWeight, float duration)
      where P : MultiPositionConstraint
      where R : MultiRotationConstraint
    {
        float elasped = 0f;
        while (elasped < duration)
        {
            elasped += Time.deltaTime;
            float time = Mathf.Clamp01(elasped / duration);
            float currentWeight = Mathf.Lerp(startWeight, endWeight, time);

            UpdatePosAndRotWeight(pos, rot, target, currentWeight);

            yield return null;
        }

        UpdatePosAndRotWeight(pos, rot, target, endWeight);
    }

    IEnumerator BlendRigWeightOverTime(RigLayer rigLayer, float startWeight, float endWeight, float duration, bool active)
    {

        Rig rig = rigLayer.rig;
        float elasped = 0f;
        while (elasped < duration)
        {
            elasped += Time.deltaTime;
            float time = Mathf.Clamp01(elasped / duration);
            float currentWeight = Mathf.Lerp(startWeight, endWeight, time);

            rig.weight = currentWeight;

            yield return null;
        }


        rig.weight = endWeight;
        if (!active)
        {
            rig.gameObject.SetActive(false);
            rigLayer.active = false;
        }
    }

    void UpdatePosAndRotWeight<P, R>(P pos, R rot, Transform weightTarget, float weight)
        where P : MultiPositionConstraint
        where R : MultiRotationConstraint
    {
        /* Target = 0 , Attack Target : 1 , */

        var posSources = pos.data.sourceObjects;
        var rotSources = rot.data.sourceObjects;

        /* Position */
        for (int i = 0; i < posSources.Count; i++)
        {
            if (posSources[i].transform == weightTarget)
                posSources.SetWeight(i, weight);
        }

        /* Rotation */
        for (int i = 0; i < rotSources.Count; i++)
        {
            if (rotSources[i].transform == weightTarget)
                rotSources.SetWeight(i, weight);
        }

        /* 재할당 */
        pos.data.sourceObjects = posSources;
        rot.data.sourceObjects = rotSources;
    }

    [ContextMenu("Shake Test")]
    public void FootsStepCameraShake()
    {
        if (cameraController == null) return;

        cameraController.ShakeCamera(transform.position);
    }

    #region Sound
    void TryStart()
    {
        if (coIdleSound == null)
            coIdleSound = StartCoroutine(CoIdleOneshots());
    }

    void TryStop()
    {
        if (coIdleSound != null)
        {
            StopCoroutine(coIdleSound);
            coIdleSound = null;
        }
    }

    IEnumerator CoIdleOneshots()
    {
        while (true)
        {
            float wait = Random.Range(intervalRange.x, intervalRange.y);
            yield return new WaitForSeconds(wait);

            // 현재 Idle인 용을 우선순위로 선택
            DragonType picked = PickCurrentIdleDragon();
            if (picked == DragonType.BODY) continue;

            AudioClip clip = idleClips[Random.Range(0, idleClips.Count)];

            Transform pos = GetAttackPosition(picked);
            if (pos == null)
                break;

            SoundManager.Instance.PlaySFX(clip,pos.position);
        }
    }
    DragonType PickCurrentIdleDragon()
    {
        // 우선순위 예시: Green > Red > Blue > Black > Yellow
        if (IsDragonPartIdle(DragonType.DRAGON_GREEN)) return DragonType.DRAGON_GREEN;
        if (IsDragonPartIdle(DragonType.DRAGON_RED)) return DragonType.DRAGON_RED;
        if (IsDragonPartIdle(DragonType.DRAGON_BLUE)) return DragonType.DRAGON_BLUE;
        //if (IsDragonPartIdle(DragonType.DRAGON_BLACK)) return DragonType.DRAGON_BLACK;
        if (IsDragonPartIdle(DragonType.DRAGON_YELLOW)) return DragonType.DRAGON_YELLOW;
        return DragonType.BODY;
    }

    Transform GetAttackPosition(DragonType type)
    {
        switch (type)
        {
            case DragonType.DRAGON_GREEN: return skillController.GreenSettings.greenAttackPoint;
            //case DragonType.DRAGON_BLACK: return skillController.BlackSettings.blackAttackPoint;
            case DragonType.DRAGON_RED: return skillController.RedSettings.redAttackPoint;
            case DragonType.DRAGON_BLUE: return skillController.BlueSettings.blueRightAttackPoint;
            case DragonType.DRAGON_YELLOW: return skillController.YellowSettings.headAttackPivot;
        }
        return null;
    }
    #endregion
    #region Gizmo

    void OnDrawGizmosSelected()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, followRange);

            /* 시야각 */
            DrawViewConeHandles(transform.position, transform.forward, viewAngle, viewRange,
                 new Color(0f, 1f, 0f, 0.08f), Color.green);
        }
    }

    void DrawViewConeHandles(Vector3 origin, Vector3 forward, float halfAngleDeg, float range, Color fill, Color line)
    {
        Vector3 f = forward; f.y = 0f; f.Normalize();
        if (f.sqrMagnitude < 1e-4f) f = transform.forward;

#if UNITY_EDITOR
        Handles.color = new Color(fill.r, fill.g, fill.b, fill.a);
        Handles.DrawSolidArc(origin, Vector3.up, Quaternion.AngleAxis(-halfAngleDeg, Vector3.up) * f,
                             halfAngleDeg * 2f, range);
        Handles.color = line;
        Handles.DrawWireArc(origin, Vector3.up, Quaternion.AngleAxis(-halfAngleDeg, Vector3.up) * f,
                            halfAngleDeg * 2f, range);
#endif
    }
    #endregion

}