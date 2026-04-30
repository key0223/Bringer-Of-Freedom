using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class EnemySkill 
{
    MainMonsterController mainController;
    EnemySkillType skillType;
    DragonType dragonType;
    bool isParallel;
    int skillPriority;
    EnemySkillState currentSkillState = EnemySkillState.IDLE;
    float cooldownTime;
    float cooldownTimer = 0;

    public EnemySkillType SkillType { get { return skillType; } }
    public DragonType DragonType { get { return dragonType; } }
    public bool IsParallel { get { return isParallel; } }
    public int SkillPriority { get { return skillPriority; } }
   

    public EnemySkill(MainMonsterController controller,MonsterSkillData skillData)
    {
        mainController = controller;
        skillType = skillData.skillType;
        dragonType = skillData.skillOwner;
        isParallel = skillData.isParallel;
        skillPriority = skillData.skillPriority;
        cooldownTime = skillData.cooldownTime;
    }
    
    public void Update(float deltaTime)
    {
        if(currentSkillState == EnemySkillState.COOLINGDOWN)
        {
            cooldownTimer -= deltaTime;
            if(cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                SetState(EnemySkillState.IDLE);
            }
        }
    }
    public void SetState(EnemySkillState bodyState)
    {
        currentSkillState = bodyState;
    }

    public void Execute()
    {
        mainController.StartCoroutine(mainController.SkillController.ExecuteSkillCoroutine(skillType));
    }
    public void StartCooldown(float time)
    {
        cooldownTimer = time; 
        currentSkillState = EnemySkillState.COOLINGDOWN;
    }
   
    public bool IsReady()
    {
        return currentSkillState == EnemySkillState.IDLE && cooldownTimer <= 0f;
    }
}
