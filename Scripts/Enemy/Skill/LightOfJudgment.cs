using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class LightOfJudgment : MonoBehaviour
{
    public void ShootIndicator(Vector3 pos)
    {
        // Effect 
        GameObject indicator = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.Lightning_Indicator);
        indicator.transform.position = pos;
    }
    public void Shoot(Vector3 pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.Judgement);
        effect.transform.position = pos;

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.LightOfJudgment);
        damage.transform.position = pos;
    }

}
