using System;
using static Define;
using static EnemyDefine;

[Serializable]
public class MonsterSkillData 
{
    public int monsterSkillId;
    public EnemySkillType skillType;
    public DragonType skillOwner;
    public EnemyCheckPivot checkPivot;
    public bool isParallel;
    public int skillPriority;
    public float checkRadius;
    public float cooldownTime;
}

