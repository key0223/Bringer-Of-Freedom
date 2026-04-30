using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DamageRadiate : SkillDamage
{
    [SerializeField] float damage;
    bool hasDamagedPlayer = false;
    GameObject playerInTrigger = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = other.gameObject;
            hasDamagedPlayer = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerInTrigger)
        {
            playerInTrigger = null;
            hasDamagedPlayer = false;
        }
    }

    void Update()
    {
        if (playerInTrigger != null && !hasDamagedPlayer)
        {
            hasDamagedPlayer = true;

            IDamageable damageable = playerInTrigger.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage((int)damage);
            }

            Debuff stun = new Debuff(DebuffType.DEBUFF_STUN, duration: 2f);
            BuffManager.Instance.AddDebuff(stun);
        }
    }
}
