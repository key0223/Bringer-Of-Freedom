using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAttack : MonoBehaviour
{
    [SerializeField] GameObject headAttackDamage;

    public void Shoot(Transform pos)
    {
        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemyDefine.EnemySkillType.HeadAttack);
        damage.transform.SetParent(pos, false);
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_HeadAttack);
    }
}
