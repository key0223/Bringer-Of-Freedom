using PixPlays.ElementalVFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class LightningStrike : MonoBehaviour
{
    public void ShootIndicator(Vector3 pos)
    {
        GameObject indicator = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.LightningStrike_Indicator);
        indicator.transform.position = pos;
    }

    public void Shoot(Vector3 pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.LightningStrike);
        effect.transform.position = pos;

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.LightningStrike);
        damage.transform.position = pos;

        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_LightningStrike);
    }
}
