using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] GameObject laserFx;
    [SerializeField] GameObject laserDamage;
  
    public void Shoot(params object[] pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.LaserBeam);

        Transform position = (Transform)pos[0];
        effect.transform.SetParent(position, false);

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.LaserBeam);
        damage.transform.SetParent(position, false);

        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Laser_Fire);

    }
    public float GetEffectDuration()
    {
        SkillDamage damage = laserDamage.GetComponent<SkillDamage>();

        return damage.Duration;
    }
}
