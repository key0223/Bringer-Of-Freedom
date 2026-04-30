using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static Define;
using static EnemyDefine;

public class YiSunShinPhaseManager : MonoBehaviour
{
    public event Action<GamePhase> OnPhaseChanged;

    MainMonsterController monsterController;

    [Header("Phase changing for test")]
    [SerializeField] GamePhase testPhase = GamePhase.PHASE_1;
    [Space(10f)]
    [SerializeField] GamePhase currentPhase = GamePhase.PHASE_1;
    [SerializeField] PhaseState currentPhaseState = PhaseState.IDLE;
    [SerializeField] Phase1GimmickTrigger phase1Trigger;

    #region Settings
    [Header("Target Marker Settings")]
    [SerializeField] List<Transform> targets;
    [Space(10f)]
    [SerializeField] float centerCheckRadius = 25f; // 중심 주변 확인 범위

    [SerializeField] bool gimmickInProgress = false;
    [SerializeField] bool phaseStopped = false;

    [Space(5)]
    [SerializeField] Transform footTransform;

    [Header("Phase 2 Settings")]
    [SerializeField] GameObject blueRightHitbox;
    [SerializeField] GameObject blueLeftHitbox;

    [Header("Phase 3 Settings")]
    [SerializeField] float yellowGroggyRatio = 0.1f;
    [SerializeField] GameObject yellowWeaknessHitbox;

    /* 데미지 방어용 콜라이더  */
    [SerializeField] GameObject obstacle;
    #endregion
    // --- [핵심 추가 1] PlayerGimmick 참조 변수 ---


    PlayerGimmick playerGimmick;
    Player player;
    readonly HashSet<DragonType> destroyedParts = new HashSet<DragonType>();
    int destroyedCount = 0;

    #region Properties
    // Properties
    public GamePhase CurrentPhase { get { return currentPhase; } }
    public PhaseState CurrentPhaseState { get { return currentPhaseState; } set { currentPhaseState = value; } }
    public Phase1GimmickTrigger Phase1Trigger { get { return phase1Trigger; } }
    public bool GimmickInProgress { get { return gimmickInProgress; } set { gimmickInProgress = value; } }
    public bool PhaseStopped { get { return phaseStopped; } set { phaseStopped = value; } }

    public Transform FootTransform { get { return footTransform; } }
    public float YellowGroggyRatio { get { return yellowGroggyRatio; } }
    public GameObject YellowWeaknessHitBox { get { return yellowWeaknessHitbox; } }
    #endregion

    [ContextMenu("Change Phase")]
    public void ChangePhase()
    {
        if(monsterController.ReceiveInput)
        {
            StopAllCoroutines();
            monsterController.SetAllSkillFalse();
            ApplyPhase(testPhase);

            //Debug.Log($"[Test] 강제로 {testPhase} 진입 완료!");
            //UIManager.Instance.Popup.ShowOperator($"테스트 모드: {testPhase} 페이즈 진입");
            //Invoke("InvokeHideOperatorUI", 2f);

            switch (testPhase)
            {
                case GamePhase.PHASE_1:
                    monsterController.IsBlueAwake = false;
                    monsterController.IsYellowAwake = false;
                    break;

                case GamePhase.PHASE_2:
                    monsterController.IsBlueAwake = true;
                    break;

                case GamePhase.PHASE_3:
                    monsterController.IsYellowAwake = true;
                    break;
            }
        }
    }
    void Awake()
    {
        monsterController = GetComponentInParent<MainMonsterController>();
        phase1Trigger.gameObject.SetActive(false);

        playerGimmick = FindObjectOfType<PlayerGimmick>();
        if (playerGimmick == null)
        {
            Debug.LogError("YiSunShinPhaseManager: 씬에서 PlayerGimmick을 찾을 수 없습니다!");
        }
    }

    void Start()
    {
        UIManager.Instance.Hud.DragonHealthUI.Init_DragonHealthUI(GamePhase.PHASE_1);

        // Target Marker Init
        UIManager.Instance.Popup.TargetMarkerUI.AddTarget(targets[0]); // 독용

        //TutorialData tutorialData = DataManager.Instance.GetTutorial("녹용");
        //if (tutorialData != null)
        //    UIManager.Instance.Popup.TutorialUI.ShowTutorial(tutorialData);

        StartCoroutine(CoMessageSequence(GameMessages.MESSAGE_PHASE1_START,GameMessages.MESSAGE_HIT_GREEN));
        obstacle.gameObject.SetActive(false);

    }

    IEnumerator CoMessageSequence(string message1,string message2)
    {
        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(message1);

        yield return new WaitForSeconds(2f);

        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(message2);
    }
    IEnumerator CoMessageSequence(string message1, string message2,string message3)
    {
        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(message1);

        yield return new WaitForSeconds(2f);

        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(message2);

        yield return new WaitForSeconds(2f);

        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(message3);
    }
    void Update()
    {
        if(currentPhaseState == PhaseState.IDLE)
            Check_GimmickCondition();
    }

    void Check_GimmickCondition()
    {
        if (currentPhaseState != PhaseState.IDLE) return;

        switch (currentPhase)
        {
            case GamePhase.PHASE_1:
                if (IsPhase1GimmickConditionMet())
                {
                    TutorialData tutorialData = DataManager.Instance.GetTutorial("맹독의 이무기");
                    if (tutorialData != null)
                        UIManager.Instance.Popup.TutorialUI.ShowTutorial(tutorialData);
                    StartCoroutine(CoExecutePhase1Gimmick());
                    UIManager.Instance.Popup.TargetMarkerUI.RemoveTarget(targets[0]);
                }
                break;
            case GamePhase.PHASE_2:
                {
                    if (IsPhase2GimmickConditionMet())
                    {
                        currentPhaseState = PhaseState.PHASE2_GIMMICK_READY;

                        blueRightHitbox.gameObject.SetActive(false);
                        blueLeftHitbox.gameObject.SetActive(false);

                        TutorialData tutorialData = DataManager.Instance.GetTutorial("설치");
                        if (tutorialData != null)
                            UIManager.Instance.Popup.TutorialUI.ShowTutorial(tutorialData);

                        UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(GameMessages.MESSAGE_PHASE2_START2);
                        //UIManager.Instance.Popup.ShowOperator("지금부터 일반 공격은 안 먹혀!");
                        //Invoke("InvokeHideOperatorUI", 3f);
                    }
                }
                break;
            case GamePhase.PHASE_3:
                {
                    if(IsPhase3GimmickConditionMet())
                    {
                        currentPhaseState = PhaseState.GIMMICK_EXECUTING;
                    }
                }
                break;
        }
    }

    #region Phase 1
    [ContextMenu("QTE TEST")]
    public void StartPhase1QTE()
    {
        StartCoroutine(CoPhase1QTE());
    }
    public IEnumerator CoPhase1QTE()
    {
        bool isSuccess = false;
        bool isFail = false;

        string qteName = "HoldPressQTE";
        QTEBase qte = QTEManager.Instance.qteDictionary[qteName];

        qte.OnSuccess += () => { isSuccess = true; };
        qte.OnFail += () => { isFail = true; };

        QTEManager.Instance.StartQTE(qteName);

        while (QTEManager.Instance.IsActive)
            yield return null;

        // 결과 처리
        if (isSuccess)
        {
            CutsceneManager.Instance.SetTrackMute("BlackQTE_Success", false);

            obstacle.gameObject.SetActive(true);

            yield return new WaitForSeconds(15f);
            monsterController.SkillController.OnEndPoisonGimmick();


        }
        else if (isFail)
        {
            CutsceneManager.Instance.SetTrackMute("BlackQTE_Failure", false);
            obstacle.gameObject.SetActive(false);
            yield return new WaitForSeconds(10f);

            IDamageable damageable = monsterController.PlayerMove.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(200);
            }

        }

        Debug.Log("Qte 종료");
    }
    IEnumerator CoExecutePhase1Gimmick()
    {
        gimmickInProgress = true;
        currentPhaseState = PhaseState.GIMMICK_EXECUTING;
        monsterController.BodyState = EnemyState.GIMMICK;

        yield return StartCoroutine(monsterController.SkillController.CoStartPhaseSkill_Sword());
    }
    /* Called by OnDragonPartDead */
    public void CompletePhase1Gimmick()
    {
        gimmickInProgress = false;
        monsterController.Anim.speed = 1;
        currentPhaseState = PhaseState.GIMMICK_COMPLETED;

        ApplyPhase(GamePhase.PHASE_2);
        monsterController.IsBlueAwake = true;
        UIManager.Instance.Popup.TargetMarkerUI.AddTarget(targets[1]); // blue left
        UIManager.Instance.Popup.TargetMarkerUI.AddTarget(targets[2]); // blue right

        TutorialData tutorialData = DataManager.Instance.GetTutorial("냉혹한 쌍두");
        if (tutorialData != null)
            UIManager.Instance.Popup.TutorialUI.ShowTutorial(tutorialData);

        StartCoroutine(CoMessageSequence(GameMessages.MESSAGE_PHASE1_END, GameMessages.MESSAGE_PHASE2_START,GameMessages.MESSAGE_HIT_BLACK));

        //phase1BodyStep.SetActive(true);

    }

    /* Called by timeline */ 
    public void OnCutsceneEnd()
    {
        monsterController.Target.transform.position = CutsceneManager.Instance.CutscenePlayer.transform.position;
        CutsceneManager.Instance.CutscenePlayer.gameObject.SetActive(false);
        CutsceneManager.Instance.Cutscene1Cam.gameObject.SetActive(false);
        monsterController.Target.gameObject.SetActive(true);
        obstacle.gameObject.SetActive(false);
    }

    #endregion

    public void InvokeHideOperatorUI()
    {
        UIManager.Instance.Popup.HideOperator();
    }
    public void CompletePhase2Gimmick()
    {
        if (currentPhase == GamePhase.PHASE_2 && currentPhaseState == PhaseState.PHASE2_GIMMICK_READY)
        {
            UIManager.Instance.Popup.TargetMarkerUI.RemoveTarget(targets[1]); // blue left
            UIManager.Instance.Popup.TargetMarkerUI.RemoveTarget(targets[2]); // blue right

            UIManager.Instance.Popup.TargetMarkerUI.AddTarget(targets[3]); // yellow

            currentPhaseState = PhaseState.GIMMICK_COMPLETED;
            gimmickInProgress = false;
            monsterController.OnDamageDragon(DragonType.DRAGON_BLUE, 99999);
            monsterController.OnDamageDragon(DragonType.DRAGON_RED, 99999);

            StartCoroutine(CoMessageSequence(GameMessages.MESSAGE_PHASE2_END, GameMessages.MESSAGE_PHASE3_START,GameMessages.MESSAGE_HIT_YELLOW));
        }
    }

    #region Is condition met
    bool IsPlayerWithinRange(float radius)
    {
        float distance = Vector3.Distance(monsterController.Target.position, monsterController.gameObject.transform.position);
        return distance <= radius;
    }
    bool IsPhase1GimmickConditionMet()
    {
        DragonHealth poisonDragonHealth = monsterController.GetDragonHealth(DragonType.DRAGON_GREEN);
        
        float hpRate = poisonDragonHealth.CurrentHp / poisonDragonHealth.MaxHp;
        bool isHpInRange = hpRate >= 0.01f && hpRate <= 0.1f; // 1 - 10 %

        bool isPlayerGrounded = monsterController.PlayerGrounded;
        bool isPlayerInRange = IsPlayerWithinRange(centerCheckRadius);

        return isHpInRange && isPlayerGrounded && isPlayerInRange;
    }

    bool IsPhase2GimmickConditionMet()
    {
        DragonHealth blueHealth = monsterController.GetDragonHealth(DragonType.DRAGON_BLUE);

        float hpRate = blueHealth.CurrentHp / blueHealth.MaxHp;

        bool isHpInRange = hpRate >= 0.01 && hpRate <= 0.2f; /* 1 - 20& */

        return isHpInRange;
    }
    bool IsPhase3GimmickConditionMet()
    {
        DragonHealth yellowHealth = monsterController.GetDragonHealth(DragonType.DRAGON_YELLOW);

        float hpRate = yellowHealth.CurrentHp / yellowHealth.MaxHp;

        bool isHpInRange = hpRate >= 0.01 && hpRate <= 0.2f; /* 1 - 20& */

        return isHpInRange;
    }
    #endregion

    void ApplyPhase(GamePhase nextPhase)
    {
        OnPhaseChanged(nextPhase); /* 이벤트 호출 */
        currentPhase = nextPhase;
        currentPhaseState = PhaseState.IDLE;
        monsterController.IsMoving = false;
        monsterController.BodyState = EnemyState.IDLE;

        // --- [핵심 추가] 페이즈별 BGM 변경 로직 ---
        if (SoundManager.Instance != null)
        {
            switch (nextPhase)
            {
                case GamePhase.PHASE_1:
                    // 1페이즈 BGM (SceneBgmController가 이미 재생했을 수 있지만,
                    // 다른 페이즈에서 1페이즈로 돌아올 경우를 대비)
                    SoundManager.Instance.PlayBgm(MusicTrack.BossBattle_Phase1);
                    break;
                case GamePhase.PHASE_2:
                    // 2페이즈 BGM으로 교체
                    SoundManager.Instance.PlayBgm(MusicTrack.BossBattle_Phase2);
                    break;
                case GamePhase.PHASE_3:
                    // 3페이즈 BGM으로 교체
                    SoundManager.Instance.PlayBgm(MusicTrack.BossBattle_Phase3);
                    break;
            }
        }
    }

    public void OnDragonPartDead(DragonType type, DragonHealth deadDragon)
    {
        if (!destroyedParts.Add(type)) return;
        deadDragon.OnDragonDeath -= OnDragonPartDead;
        destroyedCount++;

        switch(currentPhase)
        {
            case GamePhase.PHASE_1:
                {
                    bool hasGreenSword = destroyedParts.Contains(DragonType.DRAGON_GREEN) && destroyedParts.Contains(DragonType.DRAGON_BLACK);

                    if (hasGreenSword && destroyedCount >= 2)
                    {
                        Invoke("CompletePhase1Gimmick", 5f);
                    }
                }
                break;
            case GamePhase.PHASE_2:
                {
                    bool hasBlueRed = destroyedParts.Contains(DragonType.DRAGON_BLUE) && destroyedParts.Contains(DragonType.DRAGON_RED);

                    if (hasBlueRed && destroyedCount >= 4)
                        ApplyPhase(GamePhase.PHASE_3);
                    monsterController.IsYellowAwake = true;
                   
                }
                break;
            case GamePhase.PHASE_3:
                {
                    UIManager.Instance.Popup.GameMessageUI.ShowMessageFor(GameMessages.MESSAGE_PHASE3_END);
                }
                break;
        }
    }


    public void NotifyDragonDeath(DragonType type, DragonHealth health)
    {
        //UIManager.Instance.Popup.ShowGimmickText("Gimmick Done", true);
        phaseStopped = false;
        monsterController.BodyState = EnemyState.IDLE;
    }
}
