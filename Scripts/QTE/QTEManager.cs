using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : SingletonMonobehaviour<QTEManager>
{
    [SerializeField] Transform phase1QteTarget;

    // 테스트용으로 DataManager로 옮길 예정
    public Dictionary<string,QTEBase> qteDictionary = new Dictionary<string, QTEBase>();

    Coroutine coQte;
    bool isActive = false;

    public bool IsActive { get { return isActive; } }
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        QTESinglePress newQte = new QTESinglePress
        {
            qteName = "SinglePressQTE",
            keyToPress = KeyCode.Q,
            duration = 3,
            successAction = "Dodge",
            failureAction = "Hurt"
        };
        QTERepeatTap newQte2 = new QTERepeatTap
        {
            qteName = "RepeatQTE",
            keyToTap = KeyCode.Q,
            duration = 7,
            gaugeUpPerTap = 0.1f,
            gaugeDownSpeed = 0.4f,
            successAction = "Dodge",
            failureAction = "Hurt",
        };
        QTEMultiKey newQte3 = new QTEMultiKey
        {
            qteName = "MultikeyQTE",
            keys = new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R },
            duration = 8,
            successAction = "Dodge",
            failureAction = "Hurt",
            timeBetweenKeys = 1.5f,
        };

        QTEMouseClickTarget newQte4 = new QTEMouseClickTarget
        {
            qteName = "MouseClickTargetQTE",
            mouseButtonToPress = 1,
            target = phase1QteTarget,
            duration = 10,
        };

        QTEHoldPress holdQte = new QTEHoldPress
        {
            qteName = "HoldPressQTE",
            keyToHold = KeyCode.Q,
            duration = 7.0f,
            chargeSpeed = 0.4f,
            drainSpeed = 0.2f,
            successAction = "OpenDoor",
            failureAction = "Trap"
        };
        qteDictionary.Add(newQte.qteName, newQte);
        qteDictionary.Add(newQte2.qteName, newQte2);
        qteDictionary.Add(newQte3.qteName, newQte3);
        qteDictionary.Add(newQte4.qteName, newQte4);
        qteDictionary.Add(holdQte.qteName, holdQte);

        //// 이벤트 구독 예 (필요 시)
        //singlePressQTE.OnSuccess += () => Debug.Log("SinglePressQTE 성공 처리");
        //singlePressQTE.OnFail += () => Debug.Log("SinglePressQTE 실패 처리");

        //repeatTapQTE.OnSuccess += () => Debug.Log("RepeatTapQTE 성공 처리");
        //repeatTapQTE.OnFail += () => Debug.Log("RepeatTapQTE 실패 처리");
    }

    [ContextMenu("QTE 시작")]
    public void TEST()
    {
        StartQTE("RepeatQTE");
    }
    public void StartQTE(string qteName)
    {
        if (isActive) return;
        

        if (!qteDictionary.TryGetValue(qteName, out QTEBase qte))
            return;

        coQte = StartCoroutine(CoRunQTE(qte));
    }

    public void EndQTE()
    {
        if(isActive)
        {
            StopCoroutine(coQte);
            isActive = false;
            UIManager.Instance.Popup.HideQTE();
        }
    }

    private IEnumerator CoRunQTE(QTEBase qte)
    {
        isActive = true;
        SetSlowMotion(0.3f);
        yield return StartCoroutine(qte.CoExecute());
        isActive = false;
        SetSlowMotion(1f);
    }

    void SetSlowMotion(float scale = 0.5f)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;
    }
}
