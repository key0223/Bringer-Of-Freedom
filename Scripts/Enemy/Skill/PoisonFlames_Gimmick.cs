using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFlames_Gimmick : MonoBehaviour
{
    [SerializeField] GameObject poisonFlameFx;
    [SerializeField] GameObject poisonFlameGimmickDamage;


    public void Shoot(params object[] pos)
    {
        Transform position = (Transform)pos[0];

        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyDefine.EnemyEffectType.PoisonFlames);
        effect.transform.SetParent(position, false);

        // Damage
        //GameObject damage = Instantiate(poisonFlameGimmickDamage);
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemyDefine.EnemySkillType.Poison_Gimmick);
        damage.transform.position = position.position;
        damage.transform.SetParent(position, false);
    }
}
