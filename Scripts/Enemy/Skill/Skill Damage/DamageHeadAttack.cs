using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHeadAttack : SkillDamage
{
    [SerializeField] float damage;
    bool hasDamagedPlayer = false;
    GameObject playerInTrigger = null;

    [SerializeField] float headAttackDirectRadius = 1; // 직격 범위
    [SerializeField] float headAttackAoeRadius = 3f; // 주변 범위
    [Space(5f)]
    [SerializeField] float headAttackMainDamage = 10f;
    [SerializeField] float headAttackAoeDamage = 5f;

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

            Collider[] directHits = Physics.OverlapSphere(transform.position, headAttackDirectRadius, LayerMask.GetMask("Player"));
            foreach (var col in directHits)
            {
                IDamageable stats = col.GetComponent<IDamageable>();
                if (stats != null)
                    stats.TakeDamage((int)headAttackMainDamage);
            }
            Collider[] aoeHits = Physics.OverlapSphere(transform.position, headAttackAoeRadius, LayerMask.GetMask("Player"));
            foreach (var col in aoeHits)
            {
                // 직격에 포함된 객체는 제외
                bool isDirect = System.Array.Exists(directHits, c => c == col);
                if (isDirect) continue;
                IDamageable stats = col.GetComponent<IDamageable>();
                if (stats != null)
                {
                    stats.TakeDamage((int)headAttackAoeDamage);
                }
            }
            hasDamagedPlayer = true;

        }
    }
}
