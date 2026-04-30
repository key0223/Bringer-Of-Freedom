using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotDamage : MonoBehaviour
{
    [SerializeField] float damagePerSec = 1f;
    [SerializeField] float damageInterval = 1f;

    IDamageable damageable;
    Coroutine coDamage;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                coDamage = StartCoroutine(CoDamage());
            }
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
