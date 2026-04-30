using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class DamageFireGround : SkillDamage
{
    [SerializeField] float damage = 10f;
    //[SerializeField] float damagePerSec = 5f;
    void OnTriggerStay(Collider other)
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
