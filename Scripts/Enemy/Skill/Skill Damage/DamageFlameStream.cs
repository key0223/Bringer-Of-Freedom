using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlameStream : SkillDamage
{
    [SerializeField] float damage = 10f;
    //[SerializeField] float damagePerSec = 5f;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debuff defense = new Debuff(Define.DebuffType.DEBUFF_DEFENSE, duration: 10f, value: 0.3f);
            BuffManager.Instance.AddDebuff(defense);

            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                Debug.Log("Damaged");
                damageable.TakeDamage((int)damage);
            }
        }
    }
}
