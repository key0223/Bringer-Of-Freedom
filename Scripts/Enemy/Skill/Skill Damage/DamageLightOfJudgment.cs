using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLightOfJudgment : SkillDamage
{
    [SerializeField] float damage = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debuff freez = new Debuff(DebuffType.DEBUFF_FREEZE, duration: 15f, value: 0.3f);
            //BuffManager.Instance.AddDebuff(freez);

            // TODO : ¸¶ºñ
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
                damageable.TakeDamage((int)damage);
        }
    }
}
