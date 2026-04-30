using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class DamageWaterImpact : SkillDamage
{
    [SerializeField] float damage = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage((int)damage);
            }
        }
    }
}
