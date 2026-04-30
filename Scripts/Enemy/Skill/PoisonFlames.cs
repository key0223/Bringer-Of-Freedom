using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class PoisonFlames : MonoBehaviour
{
    [Header("After FX Settings")]
    [SerializeField] float afterFXOffsetX;

    public void Shoot(params object[] pos)
    {
        Transform position = (Transform)pos[0];

        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.PoisonFlames);
        effect.transform.SetParent(position, false);

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.Poison);
        damage.transform.SetParent(position, false);

        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Poison, position.position);
    }

    public void MakeAfterFX(Transform pos)
    {
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.AfterBreath_1);
        Vector3 dir = pos.forward * afterFXOffsetX;

        Vector3 spawnPos = pos.position + dir;
        effect.transform.position = spawnPos;
    }
}
