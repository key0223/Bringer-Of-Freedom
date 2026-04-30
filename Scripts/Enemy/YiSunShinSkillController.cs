using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Define;
using static EnemyDefine;
using Random = UnityEngine.Random;

public class YiSunShinSkillController : MonoBehaviour
{

    [SerializeField] MainMonsterController monsterController;

    SkillScheduler skillScheduler;
    Dictionary<EnemySkillType, EnemySkill> skills = new Dictionary<EnemySkillType, EnemySkill>();

    Dictionary<EnemySkillType, Func<bool>> skillConditionCheckers;
    Dictionary<EnemySkillType, HashSet<GamePhase>> skillAllowedPhases;

    [SerializeField] bool executingNonParallel = false; // 동시 발동 금지 스킬 시전 중인가
    [SerializeField] float skillDelay = 1;

    #region Settings
    [Header("Gizmo Skill To Show")]
    [SerializeField] bool showGizmo;
    [SerializeField] EnemySkillType skillToShow = EnemySkillType.Stomp;

    [Header("Common Settings")]
    [Space(10f)]
    [SerializeField] float centerCheckRadius = 25f; // 중심 주변 확인 범위
    [Space(5f)]
    [SerializeField] Transform rightFootTrasform;
    [SerializeField] float rightFootCheckRadius = 25; // 오른발 주변 확인 범위
    [Space(5f)]
    [SerializeField] Transform leftFootTrasform;
    [SerializeField] float leftFootCheckRadius = 25f; // 왼발 주변 확인 범위
    [Space(5f)]
    [SerializeField] float knockBackForce = 60f; // 넉백 힘

    [Space(10f)]
    [SerializeField] BodySkillSettings bodySkillSettings;
    [SerializeField] GreenSkillSettings greenSkillSettings;
    [SerializeField] BlueSkillSettings blueSkillSettings;
    [SerializeField] RedSkillSettings redSkillSettings;
    [SerializeField] YellowSkillSettings yellowSkillSettings;

    [SerializeField] IKSettings ikSettings;

    bool isPlayerOnShoulder = false;

    WaitForSeconds skillDelayWFS1 = new WaitForSeconds(1.0f);
    WaitForSeconds skillDelayWFS2 = new WaitForSeconds(2.0f);
    WaitForSeconds skillDelayWFS3 = new WaitForSeconds(3.0f);
    WaitForSeconds skillDelayWFS03 = new WaitForSeconds(0.3f);
    
    #endregion

    #region Properties
    public MultiPositionConstraint GreenMPos { get { return ikSettings.greenMPos; } set { ikSettings.greenMPos = value; } }
    public MultiRotationConstraint GreenMRot { get { return ikSettings.greenMRot; } set { ikSettings.greenMRot = value; } }
    public MultiPositionConstraint RedMPos { get { return ikSettings.redMPos; } set { ikSettings.redMPos = value; } }
    public MultiRotationConstraint RedMRot { get { return ikSettings.redMRot; } set { ikSettings.redMRot = value; } }
    public MultiPositionConstraint BlueRightMPos { get { return ikSettings.blueRightMPos; } set { ikSettings.blueRightMPos = value; } } // Ice
    public MultiRotationConstraint BlueRightMRot { get { return ikSettings.blueRightMRot; } set { ikSettings.blueRightMRot = value; } }
    public MultiPositionConstraint BlueLeftMPos { get { return ikSettings.blueLeftMPos; } set { ikSettings.blueLeftMPos = value; } } // Water
    public MultiRotationConstraint BlueLeftMRot { get { return ikSettings.blueLeftMRot; } set { ikSettings.blueLeftMRot = value; } }
    public MultiPositionConstraint BlackMPos { get { return ikSettings.blackMPos; } set { ikSettings.blackMPos = value; } }
    public MultiRotationConstraint BlackMRot { get { return ikSettings.blackMRot; } set { ikSettings.blackMRot = value; } }
    public MultiPositionConstraint YellowMPos { get { return ikSettings.yellowMPos; } set { ikSettings.yellowMPos = value; } }
    public MultiRotationConstraint YellowMRot { get { return ikSettings.yellowMRot; } set { ikSettings.yellowMRot = value; } }
    public GameObject GreenRig { get { return ikSettings.greenRig; } }
    public GameObject RedRig { get { return ikSettings.redRig; } }
    public GameObject BlueRightRig { get { return ikSettings.blueRightRig; } }
    public GameObject BlueLeftRig { get { return ikSettings.blueLeftRig; } }
    public GameObject BlackRig { get { return ikSettings.blackRig; } }
    public GameObject YellowRig { get { return ikSettings.yellowRig; } }
    public bool IsPlayerOnShoulder { get { return isPlayerOnShoulder; } set { isPlayerOnShoulder = value; } }

    public BodySkillSettings BodySettings { get { return bodySkillSettings; } }
    public GreenSkillSettings GreenSettings { get { return greenSkillSettings; } }
    public BlueSkillSettings BlueSettings { get { return blueSkillSettings; } }
    public RedSkillSettings RedSettings { get { return redSkillSettings; } }
    public YellowSkillSettings YellowSettings { get { return yellowSkillSettings; } }
    #endregion

    #region Init

    public void Init(MainMonsterController monsterController)
    {
        this.monsterController = monsterController;

        Init_SkillAllowedPhase();
        Init_SkillCondition();
    }

    void Start()
    {
        Init_EnemySkills();
        skillScheduler = new SkillScheduler(monsterController, skills);

        bodySkillSettings.cutsceneSword.gameObject.SetActive(false);
        bodySkillSettings.wall1.gameObject.SetActive(false);
        bodySkillSettings.wall2.gameObject.SetActive(false);
        // --- [핵심 추가] 씬에서 플레이어 찾기 ---
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("YiSunShinSkillController: 씬에서 Player를 찾을 수 없어 컷신 상태 변경이 불가합니다!");
        }
        // ---------------------------------
    }
    void Init_EnemySkills()
    {
        foreach (var kvp in DataManager.Instance.MonsterSkillDict)
        {
            EnemySkillType key = kvp.Key;
            MonsterSkillData skillData = kvp.Value;

            skills.Add(key, new EnemySkill(monsterController, skillData));
        }
    }

    void Init_SkillCondition()
    {
        skillConditionCheckers = new Dictionary<EnemySkillType, Func<bool>>()
    {
        { EnemySkillType.FireBall, IsFireBallAttackConditionMet },
        { EnemySkillType.Punch, IsPunchAttackConditionMet },
        { EnemySkillType.Poison, IsPoisonAttackConditionMet },
        { EnemySkillType.Pounce, IsPounceAttackConditionMet },
        { EnemySkillType.Stomp, IsStompAttackConditionMet },
        { EnemySkillType.Sword, IsSwordAttackConditionMet },
        { EnemySkillType.IceBreath, IsIceBreathAttackConditionMet },
        { EnemySkillType.WaterBreath, IsWaterBreathAttackConditionMet },
        { EnemySkillType.LaserBeam, IsLaserAttackConditionMet },
        { EnemySkillType.LightOfJudgment, IsLightOfJudgmentAttackConditionMet },
        { EnemySkillType.LightningStrike, IsLightningAttackConditionMet },
        { EnemySkillType.HeadAttack, IsHeadAttackConditionMet },
        { EnemySkillType.Radiate, IsRadiateAttackConditionMet },
        { EnemySkillType.LightCylinder, IsLightCylinderAttackConditionMet },
    };
    }
    void Init_SkillAllowedPhase()
    {
        skillAllowedPhases = new()
        {
        { EnemySkillType.Sword, new HashSet<GamePhase>{ GamePhase.PHASE_1 } },
        { EnemySkillType.Poison, new HashSet<GamePhase>{GamePhase.PHASE_1 } },
        { EnemySkillType.Pounce, new HashSet<GamePhase>{GamePhase.PHASE_1 }},
        { EnemySkillType.FireBall, new HashSet < GamePhase > { GamePhase.PHASE_1, GamePhase.PHASE_2 } },
        { EnemySkillType.Punch, new HashSet < GamePhase > {GamePhase.PHASE_2 } },
        { EnemySkillType.IceBreath, new HashSet < GamePhase > { GamePhase.PHASE_2 } },
        { EnemySkillType.WaterBreath, new HashSet < GamePhase > { GamePhase.PHASE_2 } },
        { EnemySkillType.LaserBeam, new HashSet < GamePhase > { GamePhase.PHASE_3 } },
        { EnemySkillType.LightOfJudgment, new HashSet < GamePhase > { GamePhase.PHASE_3 } },
        { EnemySkillType.LightningStrike, new HashSet < GamePhase > { GamePhase.PHASE_3 } },
        { EnemySkillType.HeadAttack, new HashSet < GamePhase > { GamePhase.PHASE_3 } },
        { EnemySkillType.Radiate, new HashSet < GamePhase > { GamePhase.PHASE_3 } },
        { EnemySkillType.LightCylinder, new HashSet < GamePhase > { GamePhase.PHASE_3 } },
        { EnemySkillType.Stomp, new HashSet < GamePhase > { GamePhase.PHASE_1 ,GamePhase.PHASE_2, GamePhase.PHASE_3} }, // 예시
        };
    }

    #endregion

    private Player player;

    void Update()
    {
        Update_CooldownTimers();
        if (CanTrySkillTick())
            TrySelectAndStart();

        if (monsterController.ReceiveInput)
        {
            //if (Input.GetKeyDown(KeyCode.Alpha9))
            //    StartPounce();

            if (Input.GetKeyDown(KeyCode.Q))
                StartStomp();

            //if (Input.GetKeyDown(KeyCode.H))
            //    StartCoroutine(CoStartPhaseSkill_Sword());

            //if (Input.GetKeyDown(KeyCode.E))
            //    StartPoison();

            //if (Input.GetKeyDown(KeyCode.R))
            //    StartFireball();

            //if (Input.GetKeyDown(KeyCode.T))
            //    StartIceBreath();

            //if (Input.GetKeyDown(KeyCode.Y))
            //    StartWaterBreath();

            //if (Input.GetKeyDown(KeyCode.U))
            //    StartCoroutine(CoStarLaserBeam());

            //if (Input.GetKeyDown(KeyCode.I))
            //    StartCoroutine(CoLightOfJudgment());

            //if (Input.GetKeyDown(KeyCode.O))
            //    StartCoroutine(CoLightningStrike());

            //if (Input.GetKeyDown(KeyCode.P))
            //    StartPunch();

            //if (Input.GetKeyDown(KeyCode.L))
            //    StartHeadAttack();

            //if (Input.GetKeyDown(KeyCode.K))
            //    StartRadiate();

            //if (Input.GetKeyDown(KeyCode.J))
            //    StartCoroutine(CoStartLightCylinder());
            if (Input.GetKeyDown(KeyCode.Alpha0))
                monsterController.TestDamage();
        }
    }

    void LateUpdate()
    {
        if(!ikSettings.rigResolved)
        {
            ikSettings.rigResolved = true;
            monsterController.AttackTarget.transform.SetParent(null, false);
            monsterController.RigBuilder.Build();
        }

    }
    void Update_CooldownTimers()
    {
        float delta = Time.deltaTime;
        foreach (EnemySkill skill in skills.Values)
        {
            skill.Update(delta);
        }
    }

    bool CanTrySkillTick()
    {
        if (monsterController.PhaseManager.GimmickInProgress) return false; // 기믹 중 선택 금지
        return true;
    }

    bool ExtraFilter(EnemySkillType skillType)
    {
        return IsSkillConditionMet(skillType);
    }

    public bool TryGetRequiredDragon(EnemySkillType skliiType, out DragonType? dragonType)
    {
        if (skills.TryGetValue(skliiType, out EnemySkill skill))
        {
            dragonType = skill.DragonType;
            return true;
        }
        dragonType = null;
        return false;
    }
    void TrySelectAndStart()
    {
        var next = skillScheduler.SelectNext(IsPhaseAllowed, ExtraFilter);
        if (!next.HasValue) return;

        var skillType = next.Value;
        var sk = skills[skillType];

        // TrySchedule에서 병렬/독점, 드래곤 리소스 락, 준비 상태를 최종 확인
        bool started = skillScheduler.TrySchedule(skillType, sk.DragonType, () => {

            SetAnimationBool(skillType, true);
            monsterController.SetOwnerStatesOnSkillStart(skillType);
            sk.Execute(); 
        });

        if (!started)
        {
            // 레이스 컨디션으로 실패 시 다음 틱에 재도전
            return;
        }
    }
    public IEnumerator ExecuteSkillCoroutine(EnemySkillType skill)
    {
        monsterController.IsMoving = false;
        monsterController.SetOwnerStatesOnSkillStart(skill);

        switch (skill)
        {
            case EnemySkillType.Stomp:
                StartStomp();
                break;
            case EnemySkillType.Sword:
                StartSword();
                break;
            case EnemySkillType.Poison:
                StartPoison();
                break;
            case EnemySkillType.Pounce:
                StartPounce();
                break;
            case EnemySkillType.IceBreath:
                StartIceBreath();
                break;
            case EnemySkillType.WaterBreath:
                StartWaterBreath();
                break;
            case EnemySkillType.LaserBeam:
                yield return CoStarLaserBeam();
                break;
            case EnemySkillType.FireBall:
                StartFireball();
                break;
            case EnemySkillType.Punch:
                StartPunch();
                break;
            case EnemySkillType.LightOfJudgment:
                yield return CoLightOfJudgment();
                break;
            case EnemySkillType.LightningStrike:
                yield return CoLightningStrike();
                break;
            case EnemySkillType.HeadAttack:
                StartHeadAttack();
                break;
            case EnemySkillType.Radiate:
                StartRadiate();
                break;
            case EnemySkillType.LightCylinder:
                StartCoroutine(CoStartLightCylinder());
                //StartLightCylinder();
                break;
            default:
                yield break;
        }
    }


    #region Body Skill

    void StartStomp()
    {
        monsterController.IsStomping = true;
    }
    /* Called by animation event */
    public void PerformStompAttack()
    {
        Vector3 stompCenter = rightFootTrasform.position + Vector3.down;

        Collider[] directHits = Physics.OverlapSphere(stompCenter, bodySkillSettings.stompDirectRadius, LayerMask.GetMask("Player"));
        foreach (var col in directHits)
        {
            if (!monsterController.PlayerGrounded || monsterController.PlayerMove == null)
            {
                continue;
            }

            IDamageable stats = col.GetComponent<IDamageable>();
            if (stats != null)
                stats.TakeDamage(bodySkillSettings.stompMainDamage);

            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (col.transform.position - rightFootTrasform.position).normalized;
                Vector3 force = (pushDir).normalized * knockBackForce;
                rb.AddForce(force, ForceMode.Impulse);

            }
        }
        Collider[] aoeHits = Physics.OverlapSphere(stompCenter, bodySkillSettings.stompAoeRadius, LayerMask.GetMask("Player"));
        foreach (var col in aoeHits)
        {
            // 직격에 포함된 객체는 제외
            bool isDirect = System.Array.Exists(directHits, c => c == col);
            if (isDirect) continue;
            IDamageable stats = col.GetComponent<IDamageable>();
            if (stats != null)
            {
                stats.TakeDamage(bodySkillSettings.stompAoeDamage);

                Debuff stiffness = new Debuff(Define.DebuffType.DEBUFF_STIFFNESS, duration: 0.2f);
                BuffManager.Instance.AddDebuff(stiffness);
            }
        }

        Vector3 rayStart = stompCenter + Vector3.up * 2;

        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Stomp, stompCenter);
        Debug.Log("PerformStompAttack");
    }

    void StartSword()
    {
        monsterController.IsSwordAttacking = true;
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;
    }

    /* Called by animtion event */
    public void OnPerformSword()
    {
        Vector3 swordCenter = monsterController.AttackTarget.position + Vector3.down;

        Collider[] directHits = Physics.OverlapSphere(swordCenter, bodySkillSettings.swordDirectRadius, LayerMask.GetMask("Player"));
        foreach (var col in directHits)
        {
            if (!monsterController.PlayerGrounded)
            {
                continue;
            }
            IDamageable stats = col.GetComponent<IDamageable>();
            if (stats != null)
                stats.TakeDamage((int)bodySkillSettings.swordMainDamage);

            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (col.transform.position - monsterController.AttackTarget.position).normalized;
                Vector3 force = (pushDir).normalized * knockBackForce;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
        Collider[] aoeHits = Physics.OverlapSphere(swordCenter, bodySkillSettings.swordAoeRadius, LayerMask.GetMask("Player"));
        foreach (var col in aoeHits)
        {
            // 직격에 포함된 객체는 제외
            bool isDirect = System.Array.Exists(directHits, c => c == col);
            if (isDirect) continue;
            IDamageable stats = col.GetComponent<IDamageable>();
            if (stats != null)
            {
                stats.TakeDamage((int)bodySkillSettings.swordAoeDamage);

                Debuff stiffness = new Debuff(Define.DebuffType.DEBUFF_STIFFNESS, duration: 0.2f);
                BuffManager.Instance.AddDebuff(stiffness);
            }
        }

        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Sword);
        Debug.Log("PerformSwordAttack");
    }


    #endregion

    #region Green Skill
    void StartPoison()
    {
        monsterController.IsGreenPoisonBreathing = true;
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;
    }
    /* Called by animation event */
    public void OnFirePoisonFlame()
    {
        StartCoroutine(CoPerformPoisonFlame());
    }
    IEnumerator CoPerformPoisonFlame()
    {
        greenSkillSettings.poisonFlameSkill.Shoot(greenSkillSettings.greenAttackPoint);

        /* 스킬 지속 후 애니메이션 세팅 */
        yield return skillDelayWFS1;

        greenSkillSettings.poisonFlameSkill.MakeAfterFX(greenSkillSettings.greenAttackPoint);

        monsterController.IsGreenPoisonBreathing = false;
    }

    void StartPounce()
    {
        monsterController.IsPouncing = true;
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;
      
    }

    /* Called by animation event */
    public void OnPerformPounce()
    {
        greenSkillSettings.pounceSkill.Shoot(greenSkillSettings.greenAttackPoint);
    }

    #endregion

    #region Blue Skill
    void StartIceBreath()
    {
        // right
        monsterController.IsIceBreathing = true;
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;

        blueSkillSettings.iceBreathSkill.MakeBeforeFX(blueSkillSettings.blueRightAttackPoint);
    }

    /* Called by animation event */
    public void OnPerformIceBreath()
    {
        StartCoroutine(CoPerformIceBreath());
    }

    IEnumerator CoPerformIceBreath()
    {
        blueSkillSettings.iceBreathSkill.Shoot(blueSkillSettings.blueRightAttackPoint);
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_WaterBreath);
        yield return skillDelayWFS1;
        StartCoroutine(CoMakeBlueBreathAfterEffect(3, blueSkillSettings.blueRightAttackPoint, monsterController.AttackTarget, blueSkillSettings.iceBreathSkill.MakeAfterFX));
        /* 스킬 지속 후 애니메이션 세팅 */
        yield return skillDelayWFS3;
        monsterController.IsIceBreathing = false;
    }

    void StartWaterBreath()
    {
        // left
        monsterController.IsWaterBreathing = true;
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;
        blueSkillSettings.waterBreathSkill.MakeBeforeFX(blueSkillSettings.blueLeftAttackPoint);
    }
    IEnumerator CoPerformWaterBreath()
    {
        blueSkillSettings.waterBreathSkill.Shoot(blueSkillSettings.blueLeftAttackPoint);
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_WaterBreath);
        yield return skillDelayWFS1;
        StartCoroutine(CoMakeBlueBreathAfterEffect(3, blueSkillSettings.blueLeftAttackPoint, monsterController.AttackTarget, blueSkillSettings.waterBreathSkill.MakeAfterFX));
        /* 스킬 지속 후 애니메이션 세팅 */
        yield return skillDelayWFS2;
        monsterController.IsWaterBreathing = false;
    }

    IEnumerator CoMakeBlueBreathAfterEffect(float duration, Transform pos, Transform target, Action<Transform, Transform> makeEffect)
    {
        float elasped = 0f;
        while (elasped < duration)
        {

            makeEffect?.Invoke(pos, target);
            yield return skillDelayWFS03;
            elasped += 0.3f;

        }
    }
    /* Called by animation event */
    public void OnPerformWaterBreath()
    {
        StartCoroutine(CoPerformWaterBreath());
    }


    #endregion

    #region Red Skill

    void StartFireball()
    {
        monsterController.IsShootingFireball = true;

        GameObject effect = Instantiate(redSkillSettings.fireBallReadyFX);
        effect.transform.SetParent(redSkillSettings.redAttackPoint, false);
        effect.transform.position = redSkillSettings.redAttackPoint.position;

        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;

    }
    /* Called by animation event */
    public void OnShootFakeFireball()
    {
        //  Shoot Fake Fireball 
        float startAngle = -60f;
        float endAngle = 60;
        float angleStep = (endAngle - startAngle) / redSkillSettings.fireBallToCreate;

        for (int i = 0; i < redSkillSettings.fireBallToCreate; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector3 direction = (monsterController.AttackTarget.position - redSkillSettings.redAttackPoint.position).normalized;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 shootDir = rotation * direction;

            FakeFireball fireBall = Instantiate(redSkillSettings.fakefireball,redSkillSettings.redAttackPoint.position,Quaternion.LookRotation(shootDir));
            fireBall.SetMuzzleTransform(redSkillSettings.redAttackPoint);
            fireBall.SetDirection(shootDir);
            fireBall.Shooting();
        }
    }
    /* Called by animation event */
    public void OnShootFireball()
    {
        float startAngle = -60f;
        float endAngle = 60;
        float angleStep = (endAngle - startAngle) / redSkillSettings.fireBallToCreate;

        for (int i = 0; i < redSkillSettings.fireBallToCreate; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector3 direction = (monsterController.AttackTarget.position - redSkillSettings.redAttackPoint.position).normalized;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 shootDir = rotation * direction;
            GameObject fireBallObj = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.FireBall_Obj);
            fireBallObj.transform.position = redSkillSettings.redAttackPoint.position;
            fireBallObj.transform.rotation = Quaternion.LookRotation(shootDir);

            //GameObject fireBallObj = Instantiate(redSkillSettings.fireBallPrefab, redSkillSettings.redAttackPoint.position, Quaternion.LookRotation(shootDir));
            FireBall fireBall = fireBallObj.GetComponent<FireBall>();
            fireBall.SetMuzzleTransform(redSkillSettings.redAttackPoint);
            fireBall.SetDirection(shootDir);
            fireBall.Shooting();
        }

        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Fireball, redSkillSettings.redAttackPoint.position);
    }
  
    void StartPunch()
    {
        monsterController.IsPunching = true;
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;
    }

    /* Called by animation event */
    public void OnPerformPunch()
    {
        // Damage
        GameObject damage = Instantiate(redSkillSettings.punchDamage);
        damage.transform.SetParent(redSkillSettings.redAttackPoint, false);
    }

    #endregion

    #region Yellow Skill

    IEnumerator CoStarLaserBeam()
    {
        monsterController.IsLaserBeaming = true;
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Laser_Charge);

        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;
        yield return null;
    }

    /* Called by animation event */
    public void OnPerformLaserBeam()
    {
        StartCoroutine(CoPerformLaserBeam());
    }
    IEnumerator CoPerformLaserBeam()
    {
        yellowSkillSettings.laserBeamSkill.Shoot(yellowSkillSettings.yellowAttackPoint);

        yield return new WaitForSeconds(5f);
        /* 스킬 지속 후 애니메이션 세팅 */
        monsterController.IsLaserBeaming = false;
    }

    IEnumerator CoLightOfJudgment()
    {
        monsterController.IsPerformingLightOfJudgment = true;
        List<Vector3> lightPos = new List<Vector3>();

        for (int i = 0; i < yellowSkillSettings.lightToCreate; i++)
        {
            Vector2 randomOnCircle = Random.insideUnitCircle * yellowSkillSettings.lightOfJudgmentRange;
            Vector3 dropPos = transform.position + new Vector3(randomOnCircle.x, 0, randomOnCircle.y);
            Vector3 spawnPos = dropPos + Vector3.up; // 공중에서 생성 (원하는 높이)

            yellowSkillSettings.lightOfJudgmentSkill.ShootIndicator(spawnPos);
            lightPos.Add(spawnPos);

            yield return skillDelayWFS03;
        }
        yield return skillDelayWFS2;

        for (int i = 0; i < lightPos.Count; i++)
        {
            Vector3 spawnPos = lightPos[i];
            yield return skillDelayWFS03;
            yellowSkillSettings.lightOfJudgmentSkill.Shoot(spawnPos);

            SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_LightOfJudgment,spawnPos);
        }
    }

    IEnumerator CoLightningStrike()
    {
        monsterController.IsPerformingLightningStrike = true;
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;

        // Set drop position
        Vector2 randomCircle = Random.insideUnitCircle * yellowSkillSettings.lightningStrikeRange;
        Vector3 dropPos = monsterController.AttackTarget.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        Vector3 spawnPos = dropPos;

        yellowSkillSettings.lightningStrikeSkill.ShootIndicator(spawnPos);

        yield return skillDelayWFS1;

        yellowSkillSettings.lightningStrikeSkill.Shoot(spawnPos);

        OnSkillEnd((int)EnemySkillType.LightningStrike);
    }

    void StartHeadAttack()
    {
        monsterController.IsHeadAttack = true;
    }

    /* Called by animation event */
    public void OnPerformHeadAttack()
    {
        yellowSkillSettings.headAttackSkill.Shoot(yellowSkillSettings.yellowAttackPoint);
    }

    void StartRadiate()
    {
        monsterController.IsRadiateAttack = true;
        yellowSkillSettings.radiateSkill.MakeBeforeFX(yellowSkillSettings.radiateSkillPoint);
    }
    /* Called by animation event */
    public void OnPerformRadiateAttack()
    {
        StartCoroutine(CoPerformRadiate());
    }

    IEnumerator CoPerformRadiate()
    {
        yellowSkillSettings.radiateSkill.Shoot(yellowSkillSettings.radiateSkillPoint);

        yield return skillDelayWFS2;
        /* 스킬 지속 후 애니메이션 세팅 */
        monsterController.IsRadiateAttack = false;
    }

    IEnumerator CoStartLightCylinder()
    {
        for (int i = 0; i <yellowSkillSettings.lightCylinderToCreate; i++)
        {
            GameObject lightCylinderObj = Instantiate(yellowSkillSettings.lightCylinderPrefab);
            lightCylinderObj.transform.position = monsterController.Target.position;

            LightCylinder lightCylinder = lightCylinderObj.GetComponent<LightCylinder>();
            lightCylinder.Init(monsterController.Target, chaseDuration: yellowSkillSettings.chaseDuration);

            yield return skillDelayWFS3;
        }

        OnSkillEnd((int)EnemySkillType.LightCylinder);
    }
    #endregion

    /* Called by animation event */
    public void OnSkillEnd(int skillEnumId)
    {
        if (!Enum.IsDefined(typeof(EnemySkillType), skillEnumId)) return;
        EnemySkillType skillType = (EnemySkillType)skillEnumId;

        SetAnimationBool(skillType,false);
        skillScheduler.OnSkillEnded(skillType, skills[skillType].DragonType);

        monsterController.SetOwnerStatesOnSkillEnd(skillType);
        //monsterController.AttackTarget.gameObject.SetActive(false);
        monsterController.BodyState = EnemyState.IDLE;
    }

    void SetAnimationBool(EnemySkillType skillType, bool running)
    {
        //Debug.Log($"Called SetAnimation Bool : {skillType} ");

        switch (skillType)
        {
            case EnemySkillType.Stomp:
                monsterController.IsStomping = false;
                break;
            case EnemySkillType.Sword:
                monsterController.IsSwordAttacking = false;
                break;
            case EnemySkillType.Poison:
                monsterController.IsGreenPoisonBreathing = false;
                break;
            case EnemySkillType.Pounce:
                monsterController.IsPouncing = false;
                break;
            case EnemySkillType.FireBall:
                monsterController.IsShootingFireball = false;
                break;
            case EnemySkillType.Punch:
                monsterController.IsPunching = false;
                break;
            case EnemySkillType.IceBreath:
                monsterController.IsIceBreathing = false;
                break;
            case EnemySkillType.WaterBreath:
                monsterController.IsWaterBreathing = false;
                break;
            case EnemySkillType.LaserBeam:
                monsterController.IsLaserBeaming = false;
                break;
            case EnemySkillType.LightOfJudgment:
                monsterController.IsPerformingLightOfJudgment = false;
                break;
            case EnemySkillType.LightningStrike:
                monsterController.IsPerformingLightningStrike = false;
                break;
            case EnemySkillType.HeadAttack:
                monsterController.IsHeadAttack = false;
                break;
            case EnemySkillType.Radiate:
                monsterController.IsRadiateAttack = false;
                break;
            case EnemySkillType.LightCylinder:
                break;
            default:
                Debug.Log($"On Skill End : {skillType}이 swith문에 없습니다.");
                break;
        }
    }
   
    #region Gimmick Skill
    public IEnumerator CoStartPhaseSkill_Sword()
    {
        monsterController.CanMove = false;
        monsterController.IsMoving = false;
        monsterController.IsSwordAttacking_Gimmick = true;

        yield return null;
    }

    IEnumerator PerformSwordAttack_Gimmick()
    {
        Debug.Log("Gimmic Sword Attack");

        Vector3 swordCenter = monsterController.AttackTarget.position + Vector3.down;

        //monsterController.AttackTarget.gameObject.SetActive(true);

        Collider[] directHits = Physics.OverlapSphere(swordCenter, bodySkillSettings.swordDirectRadius * 1.5f, LayerMask.GetMask("Player"));
        foreach (var col in directHits)
        {
            if (!monsterController.PlayerGrounded)
            {
                continue;
            }
            IDamageable stats = col.GetComponent<IDamageable>();
            if (stats != null)
                stats.TakeDamage((int)bodySkillSettings.swordMainDamage * 2);

            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (col.transform.position - monsterController.AttackTarget.position).normalized;
                Vector3 force = (pushDir).normalized * knockBackForce;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
        Collider[] aoeHits = Physics.OverlapSphere(swordCenter, bodySkillSettings.swordAoeRadius * 1.5f, LayerMask.GetMask("Player"));
        foreach (var col in aoeHits)
        {
            // 직격에 포함된 객체는 제외
            bool isDirect = System.Array.Exists(directHits, c => c == col);
            if (isDirect) continue;
            IDamageable stats = col.GetComponent<IDamageable>();
            if (stats != null)
            {
                stats.TakeDamage((int)bodySkillSettings.swordAoeDamage * 2);

                Debuff stun = new Debuff(Define.DebuffType.DEBUFF_STUN, duration: 1f);
                BuffManager.Instance.AddDebuff(stun);
            }
        }

        TutorialData tutorialData = DataManager.Instance.GetTutorial("발악");
        if (tutorialData != null)
            UIManager.Instance.Popup.TutorialUI.ShowTutorial(tutorialData);

        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(GameMessages.MESSAGE_PHASE1_GIMMICK_START);
        monsterController.PhaseManager.Phase1Trigger.gameObject.SetActive(true);

        // 플레이어가 검용을 때리면 Cutscene 실행
        yield return new WaitUntil(() => monsterController.PhaseManager.Phase1Trigger.IsTriggered);

        UIManager.Instance.Hud.gameObject.SetActive(false);

        CutsceneManager.Instance.Cutscene1Cam.gameObject.SetActive(true);
        CutsceneManager.Instance.CutscenePlayer.gameObject.SetActive(true);
        player.gameObject.SetActive(false);
        // --- [핵심 추가] 컷신 재생 전 플레이어 상태 변경 ---
        // Player.cs에 추가된 StartCutsceneState 함수 호출 (0 = 무기한)
        player?.StartCutsceneState(0);

        monsterController.IsSwordAttacking_Gimmick = false;
        monsterController.IsBlackDown = true;
        bodySkillSettings.sword.gameObject.SetActive(false);
        bodySkillSettings.cutsceneSword.gameObject.SetActive(true);

        yield return StartCoroutine(CutsceneManager.Instance.PlayAndWait(0));
        //StartCoroutine(CutsceneManager.Instance.PlayAndWait(1));
        player?.EndCutsceneState();
        UIManager.Instance.Hud.gameObject.SetActive(true);


        // --- [핵심 추가] 컷신 종료 후 GimmickOrb 활성화 ---
        // (GimmickOrb가 Phase1Trigger 오브젝트에 붙어있다고 가정)
        GimmickOrb orb = monsterController.PhaseManager.Phase1Trigger.GetComponent<GimmickOrb>();
        if (orb != null)
        {
            orb.ActivateOrb(); // GimmickOrb.cs에 새로 추가한 함수 호출
        }
        else
        {
            // 만약 GimmickOrb가 별도로 스폰되었다면 FindObjectOfType 사용 (차선책)
            GimmickOrb orbInScene = FindObjectOfType<GimmickOrb>();
            if (orbInScene != null)
            {
                orbInScene.ActivateOrb();
            }
            else
            {
                Debug.LogError("GimmickOrb를 씬에서 찾을 수 없습니다! Phase1Trigger 또는 씬 전체를 확인하세요.");
            }
        }
        // ------------------------------------------------
    }

    /* Called by animation event */
    public void OnDetacthSword()
    {
        // 검용 칼이 박힌 상태
        bodySkillSettings.sword.gameObject.transform.SetParent(null, true);
        bodySkillSettings.wall1.gameObject.SetActive(true);
        bodySkillSettings.wall2.gameObject.SetActive(true);
    }
    public void OnPerformSwordAttack_Gimmick()
    {
        StartCoroutine(PerformSwordAttack_Gimmick());
    }

    /* Called by PerformSwordAttack_Gimmick */
    public void StartPoisonGimmick()
    {
        float duration = greenSkillSettings.pounceSpeed;

        StartCoroutine(monsterController.BlendWeightOverTime(ikSettings.greenMPos, ikSettings.greenMRot, monsterController.Target,
                                                            startWeight: 1, endWeight: 0, duration));
        StartCoroutine(monsterController.BlendWeightOverTime(ikSettings.greenMPos, ikSettings.greenMRot, monsterController.AttackTarget,
                                                            startWeight: 0, endWeight: 1, duration));
        StartCoroutine(CoStartPoisonGimmick());

    }
    IEnumerator CoStartPoisonGimmick()
    {
        //monsterController.IsPoisonAttacking_Gimmick = true;
        float elapsed = 0f;
        float duration = greenSkillSettings.pounceSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = Mathf.Clamp01(elapsed / duration);
            float currentWeight = Mathf.Lerp(0.05f, 0.3f, time);

            ikSettings.greenChainIK.weight = currentWeight;
            monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;
            yield return null;
        }
    }

    IEnumerator CoPerformPoisonGimmick()
    {
        
        greenSkillSettings.poisonFlamesGimmickSkill.Shoot(greenSkillSettings.greenAttackPoint, monsterController.Target);

        /* 스킬 지속 후 애니메이션 세팅 */

        yield return null;
        //yield return new WaitForSeconds(3f);
        //monsterController.IsPoisonAttacking_Gimmick = false;
    }

    IEnumerator CoEndPoisonGimmick()
    {
        /* Green, black death */
        monsterController.IsBlackDown = false;
        monsterController.PhaseManager.CurrentPhaseState = PhaseState.GIMMICK_AWAITING_COMPLETION;
        monsterController.PhaseManager.GimmickInProgress = false;

        yield return new WaitForEndOfFrame();
        monsterController.OnDamageDragon(DragonType.DRAGON_BLACK, 1000f);
        monsterController.OnDamageDragon(DragonType.DRAGON_GREEN, 1000f);

        monsterController.CanMove = true;
    }

  
    /* Called by animation event|| cutscene  */
    public void OnPerformPoisonGimmick()
    {
        monsterController.AttackTarget.transform.position = monsterController.Target.transform.position;

        StartCoroutine(monsterController.BlendWeightOverTime(ikSettings.greenMPos, ikSettings.greenMRot, monsterController.Target,
                                                           startWeight: 1, endWeight: 0, 1));
        StartCoroutine(monsterController.BlendWeightOverTime(ikSettings.greenMPos, ikSettings.greenMRot, monsterController.AttackTarget,
                                                            startWeight: 0, endWeight: 1, 1));
        StartCoroutine(CoPerformPoisonGimmick());
    }
    public void OnEndPoisonGimmick()
    {

        StartCoroutine(CoEndPoisonGimmick());
    }
    #endregion

    #region Check if can use skill
    public bool IsSkillConditionMet(EnemySkillType skill)
    {
        if (skillConditionCheckers.TryGetValue(skill, out var condition))
            return condition();
        return false;
    }
    #endregion

    #region Attack condition met
    bool IsPhaseAllowed(EnemySkillType skillType)
    {
        if (monsterController.PhaseManager.GimmickInProgress) return false;
        return IsSkillAllowedInPhase(skillType, monsterController.PhaseManager.CurrentPhase);
    }
    public bool IsSkillAllowedInPhase(EnemySkillType skillType, GamePhase phase)
    {
        /* 스킬이 등록되어 있는가 */
        if (!skillAllowedPhases.TryGetValue(skillType, out var allowedPhases))
            return false;

        /* 스킬이 현재 페이즈에 사용 가능 한가 */
        if (!allowedPhases.Contains(monsterController.PhaseManager.CurrentPhase)) return false;

        if (skills.TryGetValue(skillType, out EnemySkill enemySkill))
        {
            if (!monsterController.IsDragonAlive(enemySkill.DragonType)) return false;
        }
        return true;
    }

    bool IsPlayerGrounded()
    {
        return monsterController.PlayerGrounded;
    }
 
    bool IsTargetWithinRange(EnemySkillType skillType)
    {
        if(monsterController.Target ==  null) return false;

        MonsterSkillData skillData = null;
        DataManager.Instance.MonsterSkillDict.TryGetValue(skillType, out skillData);
        if (skillData == null) return false;

        float radius = Mathf.Max(skillData.checkRadius, 0.01f);

        Vector3 pivot = Vector3.zero;

        if (skillData.checkPivot == EnemyCheckPivot.BODY)
            pivot = transform.position;
        else if (skillData.checkPivot == EnemyCheckPivot.LEFT)
            pivot = leftFootTrasform.position;
        else
            pivot = rightFootTrasform.position;

        float dist = Vector3.Distance(pivot, monsterController.Target.position);

        return dist <= radius;
    }

    bool IsStompAttackConditionMet()
    {
        if (!IsPlayerGrounded()) return false;
        bool inRange = IsTargetWithinRange(EnemySkillType.Stomp);

        return inRange;
    }

    bool IsSwordAttackConditionMet()
    {
        if (!IsPlayerGrounded()) return false;

        bool inRange = IsTargetWithinRange(EnemySkillType.Sword);

        return inRange;
    }
    bool IsFireBallAttackConditionMet()
    {
        bool isOnLowPlatform = monsterController.Target.position.y <= redSkillSettings.fireBallMaxHeight;
        bool inRange = IsTargetWithinRange(EnemySkillType.FireBall);
        return isOnLowPlatform && inRange;
    }

    bool IsPunchAttackConditionMet()
    {
        bool isOnLowPlatform = monsterController.Target.position.y <= redSkillSettings.punchMaxHeight;
        bool inRange = IsTargetWithinRange(EnemySkillType.Punch);
        return isOnLowPlatform && inRange;
    }
    bool IsPoisonAttackConditionMet()
    {
        bool isOnLowPlatform = monsterController.Target.position.y <= greenSkillSettings.poisonMaxHeight;
        bool inRange = IsTargetWithinRange(EnemySkillType.Poison);
        return isOnLowPlatform && inRange;
    }
    bool IsPounceAttackConditionMet()
    {
        bool isOnLowPlatform = monsterController.Target.position.y <= greenSkillSettings.pounceMaxHeight;
        bool inRange = IsTargetWithinRange(EnemySkillType.Pounce);
        return isOnLowPlatform && inRange;
    }

    bool IsIceBreathAttackConditionMet()
    {
        bool isOnLowPlatform = monsterController.Target.position.y <= blueSkillSettings.blueMaxHeight;
        bool inRange = IsTargetWithinRange(EnemySkillType.IceBreath);
        return isOnLowPlatform && inRange;
    }
    bool IsWaterBreathAttackConditionMet()
    {
        bool isOnLowPlatform = monsterController.Target.position.y <= blueSkillSettings.blueMaxHeight;
        bool inRange = IsTargetWithinRange(EnemySkillType.WaterBreath);
        return isOnLowPlatform && inRange;
    }
    bool IsLightningAttackConditionMet()
    {
        bool isOnLowPlatform = monsterController.Target.position.y <= yellowSkillSettings.lightningStrikeMaxHeight;
        bool inRange = IsTargetWithinRange(EnemySkillType.LightningStrike);
        return isOnLowPlatform && inRange;
    }
    bool IsLaserAttackConditionMet()
    {
        float playerYPos = monsterController.Target.position.y;
        bool isOnLowPlatform = (playerYPos >= yellowSkillSettings.laserMinHeight) && (playerYPos <= yellowSkillSettings.laserMaxHeight);
        bool inRange = IsTargetWithinRange(EnemySkillType.LaserBeam);
        return isOnLowPlatform && inRange;
    }
    bool IsLightOfJudgmentAttackConditionMet()
    {
        bool inRange = IsTargetWithinRange(EnemySkillType.LightOfJudgment);
        return inRange;
    }

    bool IsHeadAttackConditionMet()
    {
        return isPlayerOnShoulder;
    }
    bool IsRadiateAttackConditionMet()
    {
        float playerYPos = monsterController.Target.position.y;
        bool isOnLowPlatform = (playerYPos >= yellowSkillSettings.radiateMinHeight) && (playerYPos <= yellowSkillSettings.radiateMaxHeight);
        bool playerBehinde = monsterController.IsPlayerBehind;

        return isOnLowPlatform && !playerBehinde;
    }
    bool IsLightCylinderAttackConditionMet()
    {
        bool inRange = IsTargetWithinRange(EnemySkillType.LightCylinder);
        return inRange;
    }
    #endregion

    #region Gizmo

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        if (monsterController == null)
            return;
        if(!showGizmo) return;

        switch (skillToShow)
        {
            case EnemySkillType.Stomp:
                if (rightFootTrasform != null)
                {
                    Vector3 stompCenter = rightFootTrasform.position;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(stompCenter, bodySkillSettings.stompDirectRadius);
                    Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
                    Gizmos.DrawWireSphere(stompCenter, bodySkillSettings.stompAoeRadius);

                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(stompCenter, bodySkillSettings.stompCheckRadius);
                }
                break;
            case EnemySkillType.Sword:
                if (bodySkillSettings.blackAttackPoint != null)
                {
                    Vector3 swordCenter = transform.position;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(swordCenter, bodySkillSettings.swordDirectRadius);
                    Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
                    Gizmos.DrawWireSphere(swordCenter, bodySkillSettings.swordAoeRadius);

                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(swordCenter, bodySkillSettings.swordCheckRadius);
                }
                break;
            case EnemySkillType.Poison:
                {
                    DrawCylSlab(leftFootTrasform.position, greenSkillSettings.poisonCheckRadius, 
                                0, greenSkillSettings.poisonMaxHeight, "Poison skill");
                }
                break;
            case EnemySkillType.Pounce:
                {
                    DrawCylSlab(leftFootTrasform.position, greenSkillSettings.pounceCheckRadius, 
                                0, greenSkillSettings.pounceMaxHeight, "Pounce skill");
                }
                break;
            case EnemySkillType.FireBall:
                {
                    DrawCylSlab(leftFootTrasform.position, redSkillSettings.fireballCheckRadius, 
                                0, redSkillSettings.fireBallMaxHeight ,"Fireball");
                }
                break;
            case EnemySkillType.IceBreath:
            case EnemySkillType.WaterBreath:
                DrawCylSlab(transform.position, blueSkillSettings.breathCheckRadius, 
                            0, blueSkillSettings.blueMaxHeight, "Ice/Water Breath");
                break;
            case EnemySkillType.LaserBeam:
                {
                    DrawCylSlab(transform.position, yellowSkillSettings.laserCheckRadius,
                          yellowSkillSettings.laserMinHeight, yellowSkillSettings.laserMaxHeight, "Laser");
                }
                break;
            case EnemySkillType.LightOfJudgment:
                {
                    DrawCylSlab(transform.position, yellowSkillSettings.lightOfJudgmentCheckRadius,
                                0, 0, "LightOfJudgment (높이 상관 없음)");

                    Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.9f);

                    Vector3 center = transform.position;
                    float r = yellowSkillSettings.lightOfJudgmentRange;
                    Gizmos.DrawWireSphere(center, r);
                }
                break;
            case EnemySkillType.LightningStrike:
                {
                    DrawCylSlab(transform.position, yellowSkillSettings.lightningStrikeCheckRadius, 
                                0, yellowSkillSettings.lightningStrikeMaxHeight, "Lightning Strike");

                    Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.9f);

                    Vector3 center = Vector3.zero;
                    if (monsterController.AttackTarget == null)
                        center = transform.position;
                    else
                        center = monsterController.AttackTarget.position;

                    float r = yellowSkillSettings.lightningStrikeRange;
                    Gizmos.DrawWireSphere(center, r);
                }
                break;
            case EnemySkillType.Punch:
                {
                    DrawCylSlab(leftFootTrasform.position, redSkillSettings.punchCheckRadius,
                                0, redSkillSettings.punchMaxHeight, "Red Punch");
                }
                break;
            case EnemySkillType.HeadAttack:
                {
                    DrawCylSlab(yellowSkillSettings.headAttackPivot.position, yellowSkillSettings.headCheckRadius, 
                                yellowSkillSettings.headMinHeight, yellowSkillSettings.headMaxHeight, "Head Attack");
                }
                break;
            case EnemySkillType.Radiate:
                {
                    DrawCylSlab(yellowSkillSettings.headAttackPivot.position, yellowSkillSettings.radiateCheckRadius, 
                                yellowSkillSettings.radiateMinHeight, yellowSkillSettings.radiateMaxHeight, "Radiate");
                }
                break;
            case EnemySkillType.LightCylinder:
                {
                    DrawCylSlab(transform.position, yellowSkillSettings.lightCylinderCheckRadius, 
                                yellowSkillSettings.lightCylinderMinHeight, yellowSkillSettings.lightCylinderMaxHeight, "LightCylinder");
                }
                break;
            default:
                break;
        }
    }


    void DrawCylSlab(Vector3 centerXZ, float radius, float minY, float maxY, string label)
    {
        float y0 = minY;
        float y1 = maxY;
        Vector3 top = new Vector3(centerXZ.x, y1, centerXZ.z);
        Vector3 bot = new Vector3(centerXZ.x, y0, centerXZ.z);

        Color fill = new Color(0, 1, 0, 0.15f);
        Color line = new Color(0, 1, 0, 1);
        // 와이어 원 두 개
        Gizmos.color = fill;
        Gizmos.DrawWireSphere(top, radius);
        Gizmos.DrawWireSphere(bot, radius);

        // 수직 윤곽선
        int verticals = 6;
        for (int i = 0; i < verticals; i++)
        {
            float ang = (i / (float)verticals) * Mathf.PI * 2f;
            float x = Mathf.Cos(ang) * radius;
            float z = Mathf.Sin(ang) * radius;
            Gizmos.DrawLine(new Vector3(centerXZ.x + x, y0, centerXZ.z + z),
                            new Vector3(centerXZ.x + x, y1, centerXZ.z + z));
        }

#if UNITY_EDITOR
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.color = new Color(fill.r, fill.g, fill.b, fill.a);
        Handles.DrawSolidDisc(top, Vector3.up, radius);
        Handles.DrawSolidDisc(bot, Vector3.up, radius);

        // 라벨
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.normal.textColor = new Color(line.r, line.g, line.b, 0.95f);
        Handles.Label(top + Vector3.up * 0.5f, $"{label} [{y0:0.##} ~ {y1:0.##}] r={radius:0.##}", style);
#endif
    }
    #endif
    #endregion
}
