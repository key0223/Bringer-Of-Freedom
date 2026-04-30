using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using static EnemyDefine;

public class DragonHealth 
{
    public event Action<DragonType,DragonHealth> OnDragonDamaged;
    public event Action<DragonType,DragonHealth> OnDragonDeath;

    MainMonsterController monsterController;

    List<Debuff> activeDebuffs = new List<Debuff>();


    public IReadOnlyList<Debuff> ActiveDebuffs { get { return activeDebuffs; } }
    public DragonType DragonType { get; private set; }
    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public bool IsDead { get; private set; }


    public DragonHealth(MainMonsterController monsterController, DragonType type, float maxHp)
    {
        this.monsterController = monsterController;
        DragonType = type;
        MaxHp = maxHp;
        CurrentHp = MaxHp;
        IsDead = false;
        OnDragonDamaged?.Invoke(type, this);
    }

    public void TakeDamage(DragonType type, float damage)
    {
        if(monsterController.PhaseManager.CurrentPhaseState == PhaseState.IDLE)
            CurrentHp = Mathf.Max(CurrentHp - damage, 1); // 기믹 시작 전 최소 체력 보장
        else
            CurrentHp = Mathf.Max(CurrentHp - damage, 0);

        OnDragonDamaged?.Invoke(type,this);

        Debug.Log($"{DragonType} damaged : {damage}, currentHp : {CurrentHp}");

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    #region Manage Debuff
    public void Update()
    {
        DebuffUpdate();
    }
    public void AddDebuff(Debuff newDebuff)
    {
        Debuff existingDebuff = activeDebuffs.FirstOrDefault(d => d.DebuffType == newDebuff.DebuffType);

        if (existingDebuff != null)
            return; // 이미 걸려있으면 무시

        activeDebuffs.Add(newDebuff);
    }

    void DebuffUpdate()
    {
        if (activeDebuffs.Count <= 0) return;

        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            if (activeDebuffs[i].Tick(Time.deltaTime))
            {
                activeDebuffs.RemoveAt(i);
            }
        }
    }

    public bool HasDebuff(DebuffType type, out Debuff found)
    {
        foreach (Debuff debuff in activeDebuffs)
        {
            if (debuff.DebuffType == type)
            {
                found = debuff;  
                return true;
            }
        }
        found = null; 
        return false;
    }
    #endregion
    void Die()
    {
        IsDead = true;
        OnDragonDeath?.Invoke(DragonType,this);
    }
}
