using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBullet : SkillDamage
{
    [SerializeField] float damage = 1f;

  
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debuff burn = new Debuff(Define.DebuffType.DEBUFF_BURN, duration: 10f, value: 2);
            BuffManager.Instance.AddDebuff(burn);

            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage((int)damage);
            }
        }
    }
}
