using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class SkillDamage : MonoBehaviour
{
    [SerializeField] EnemySkillType type;
    [SerializeField] protected float duration;

    public float Duration { get { return duration; } }
    private void OnEnable()
    {
        Invoke("StopDamage", duration);
    }

    protected virtual void StopDamage()
    {
        PoolManager.Instance.SkillDamagePooler.ReturnEnemySkill(type,gameObject);
    }

}
