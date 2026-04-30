using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DamageWaterBreath : SkillDamage
{
    [SerializeField] float damagePerSec = 2f;
    [SerializeField] float damageInterval = 1f;

    [SerializeField] GameObject groggyPointFX;

    IDamageable damageable;
    Coroutine coDamage;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debuff freez = new Debuff(DebuffType.DEBUFF_FREEZE, duration: 15f, value: 0.3f);
            //BuffManager.Instance.AddDebuff(freez);

            // TODO : 화상 상태 제거
            damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
                coDamage = StartCoroutine(CoDamage());

            //GameObject GroggyPointFX = Instantiate(groggyPointFX);
            //Vector3 groggyPointPos = other.transform.position;
            //groggyPointFX.transform.position = groggyPointPos;
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GameObject GroggyPointFX = Instantiate(groggyPointFX);
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            Vector3 groggyPointPos = contactPoint;
            groggyPointPos.y = 0;

            groggyPointFX.transform.position = groggyPointPos;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (coDamage != null)
            {
                StopCoroutine(coDamage);
                coDamage = null;
            }
            damageable = null;
        }
    }

    IEnumerator CoDamage()
    {
        while (true)
        {
            damageable.TakeDamage((int)damagePerSec);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
