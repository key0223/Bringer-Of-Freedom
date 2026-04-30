using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class WaterBreath : MonoBehaviour
{

    [Header("FX Settings")]
    [SerializeField] float afterFXOffsetX;

    public void Shoot(Transform pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.WaterBreath);
        effect.transform.SetParent(pos, false);

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.WaterBreath);
        damage.transform.SetParent(pos, false);

    }
    public void MakeBeforeFX(Transform pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.ChargeBreath);
        effect.transform.SetParent(pos, false);
    }
    public void MakeAfterFX(Transform pos, Transform target)
    {
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.AfterBreath_2);
        Vector3 dir =(target.position - pos.position).normalized;

        Vector3 spawnPos = pos.position + dir* afterFXOffsetX;
        effect.transform.position = spawnPos;
    }
}
