using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DamageIceTornado : SkillDamage
{
    [SerializeField] float damagePerSec = 2f;
    [SerializeField] float damageInterval = 1f;

    IDamageable damageable;
    Coroutine coDamage;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debuff freez = new Debuff(DebuffType.DEBUFF_FREEZE, duration:15f, value:0.3f);
            BuffManager.Instance.AddDebuff(freez);

            damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
                coDamage = StartCoroutine(CoDamage());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(coDamage != null)
            {
                StopCoroutine(coDamage);
                coDamage = null;
            }
            damageable = null;
        }
    }

    IEnumerator CoDamage()
    {
        while(true)
        {
            damageable.TakeDamage((int)damagePerSec);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
