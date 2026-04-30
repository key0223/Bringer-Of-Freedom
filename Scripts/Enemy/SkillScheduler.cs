using System;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;
/// <summary>
///  선택,예약,실행,종료,쿨타임,병렬/독점, 드래곤 리소스 점유 관리
/// </summary>
public class SkillScheduler 
{
    readonly MainMonsterController owner;
    readonly Dictionary<EnemySkillType, EnemySkill> skills;
    readonly HashSet<EnemySkillType> running = new();
    readonly HashSet<EnemySkillType> runningNonParallel = new();
    readonly HashSet<DragonType> dragonLocked = new();

    Dictionary<EnemySkillType, MonsterSkillData> cachedSkillData = new();

    public SkillScheduler(MainMonsterController owner, Dictionary<EnemySkillType, EnemySkill> skills)
    {
        this.owner = owner;
        this.skills = skills;

        foreach(var type in skills.Keys)
        {
            if (DataManager.Instance.MonsterSkillDict.TryGetValue(type, out var data))
                cachedSkillData[type] = data;
        }
    }

    bool IsBlockedByParallel(EnemySkillType skillType)
    {
        EnemySkill skill = skills[skillType];
        if (skill.IsParallel) return false;
        return runningNonParallel.Count > 0; // 독점 스킬 진행 중이면 차단
    }
    bool IsDragonBusy(DragonType? dragon)
    {
        if (dragon == null) return false;
        return dragonLocked.Contains(dragon.Value);
    }
    public bool CanSchedule(EnemySkillType skillType, DragonType? dragon, Func<bool> extraCheck)
    {
        if (!skills.ContainsKey(skillType)) return false;
        EnemySkill skill = skills[skillType];
        if (!skill.IsReady()) return false;                  // 쿨다운, 상태 체크
        if (IsBlockedByParallel(skillType)) return false;        // 동시 사용 가능?
        if (IsDragonBusy(dragon)) return false;          // 드래곤이 스킬 사용 중인가?
        if (extraCheck != null && !extraCheck()) return false; // 페이즈, 거리 
        return true;
    }

    public bool TrySchedule(EnemySkillType skillType, DragonType? dragon, Action onStart)
    {
        if (!CanSchedule(skillType, dragon, null)) return false;

        if (!cachedSkillData.TryGetValue(skillType, out var skill)) return false;

        if (!skill.isParallel) runningNonParallel.Add(skillType);

        running.Add(skillType);
        if (dragon != null) dragonLocked.Add(dragon.Value);
        onStart?.Invoke();                               // 애니메이션 트리거 등
        skills[skillType].SetState(EnemySkillState.EXECUTING);  
        return true;
    }
    public void OnSkillEnded(EnemySkillType skillType, DragonType? dragon)
    {
        if (!running.Contains(skillType)) return;                // 중복 종료 방어
        running.Remove(skillType);
        runningNonParallel.Remove(skillType);
        if (dragon != null) dragonLocked.Remove(dragon.Value);

        if (!cachedSkillData.TryGetValue(skillType, out var skill)) return;

        skills[skillType].StartCooldown(skill.cooldownTime);
        skills[skillType].SetState(EnemySkillState.COOLINGDOWN);
    }

    public EnemySkillType? SelectNext(Func<EnemySkillType, bool> phaseFilter, Func<EnemySkillType, bool> extraFilter)
    {
        EnemySkillType? best = null;
        int bestPrio = int.MinValue;
        foreach (var kv in skills)
        {
            var skillType = kv.Key;
            if (!cachedSkillData.TryGetValue(skillType, out var skill)) continue;

            if (phaseFilter != null && !phaseFilter(skillType)) continue;
            if (extraFilter != null && !extraFilter(skillType)) continue;
            if (!CanSchedule(skillType, kv.Value.DragonType, null)) continue;
            int priority = skill.skillPriority;
            if (priority > bestPrio) { bestPrio = priority; best = skillType; }
        }
        return best;
    }

}
