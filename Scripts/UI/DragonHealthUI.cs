using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static EnemyDefine;

[Serializable]
public class DragonPartUI
{
    public DragonType dragonType;
    public Image valueImage;
    public List<Sprite> stateSprites; /* 0 = Grey, 1 = Green, 2 = Yellow, 3 = Red */
}

public class DragonHealthUI : MonoBehaviour
{
    MainMonsterController mainMonsterController;

    [Header("HP bar Settings")]
    [SerializeField] Image hpBarBGImage;
    [SerializeField] Image healthValueImage;

    
    [Header("Parts Settings")]
    //[SerializeField] List<DragonPartUI> dragonParts;

    float lerpDuration = 2f;

    void Awake()
    {
        mainMonsterController = FindAnyObjectByType<MainMonsterController>();
    }

    void OnEnable()
    {
        if (mainMonsterController.PhaseManager == null) return;
        mainMonsterController.PhaseManager.OnPhaseChanged -= Init_DragonHealthUI;
        mainMonsterController.PhaseManager.OnPhaseChanged += Init_DragonHealthUI;
    }
    void OnDisable()
    {
        mainMonsterController.PhaseManager.OnPhaseChanged -= Init_DragonHealthUI;
    }
    void Start()
    {
        mainMonsterController.PhaseManager.OnPhaseChanged -= Init_DragonHealthUI;
        mainMonsterController.PhaseManager.OnPhaseChanged += Init_DragonHealthUI;
    }
    public void Init_DragonHealthUI(GamePhase gamePhase)
    {
        DragonHealth health;

        //foreach(DragonPartUI part in dragonParts)
        //{
        //    // 먼저 모두 비활성화
        //    part.valueImage.sprite = part.stateSprites[0];
        //}

        if(gamePhase == GamePhase.PHASE_1)
        {
            health = mainMonsterController.GetDragonHealth(DragonType.DRAGON_GREEN);
        }
        else if(gamePhase == GamePhase.PHASE_2)
        {
            health = mainMonsterController.GetDragonHealth(DragonType.DRAGON_BLUE);
        }
        else
        {
            health = mainMonsterController.GetDragonHealth(DragonType.DRAGON_YELLOW);
        }
        //SetDragonPartUIByPhase(gamePhase,1);
        health.OnDragonDamaged += UpdateDragonHpUI;
        health.OnDragonDeath += OnDragonDeath;
        StartCoroutine(CoBlendValue(0,1));
    }

    void UpdateDragonHpUI(DragonType type, DragonHealth damagedDragon)
    {
        float healthRatio = (float)damagedDragon.CurrentHp / damagedDragon.MaxHp;
        healthValueImage.fillAmount = healthRatio;

        //int index;
        //if (healthRatio > 0.7f) index = 1;
        //else if (healthRatio>0.3f) index = 2;
        //else index = 3;

        //SetDragonPartUIState(type,index);
    }
    void OnDragonDeath(DragonType dragon, DragonHealth dragonHealth)
    {
        dragonHealth.OnDragonDamaged -= UpdateDragonHpUI;
        dragonHealth.OnDragonDeath -= OnDragonDeath;
    }

    void SetDragonPartUIState(DragonType dragonType, int stateIndex)
    {
        //DragonPartUI part = GetDragonPartUI(dragonType);
        //part.valueImage.sprite = part.stateSprites[stateIndex];
    }
    void SetDragonPartUIByPhase(GamePhase gamePhase, int stateIndex)
    {
        if (gamePhase == GamePhase.PHASE_1)
        {
            SetDragonPartUIState(DragonType.DRAGON_GREEN, stateIndex);
            SetDragonPartUIState(DragonType.DRAGON_BLACK, stateIndex);
            SetDragonPartUIState(DragonType.DRAGON_RED, stateIndex);
        }
        else if (gamePhase == GamePhase.PHASE_2)
        {
            SetDragonPartUIState(DragonType.DRAGON_RED, stateIndex);
            SetDragonPartUIState(DragonType.DRAGON_BLUE, stateIndex);
        }
        else
        {
            SetDragonPartUIState(DragonType.DRAGON_YELLOW, stateIndex);
        }
    }
    //DragonPartUI GetDragonPartUI(DragonType find)
    //{
    //    for (int i = 0; i < dragonParts.Count; i++)
    //    {
    //        if (dragonParts[i].dragonType == find)
    //            return dragonParts[i];
    //    }
    //    return null;
    //}
    IEnumerator CoBlendValue(float start, float end)
    {
        float elasped = 0;

        while(elasped < lerpDuration)
        {
            elasped += Time.deltaTime;
            float time = Mathf.Clamp01(elasped / lerpDuration);
            float currentValue = Mathf.Lerp(start, end, time);

            healthValueImage.fillAmount = currentValue;
            yield return null;
        }
    }

    
}
