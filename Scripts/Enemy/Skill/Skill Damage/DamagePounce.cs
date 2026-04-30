using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamagePounce : SkillDamage
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
            Vector3 origin = transform.position;
            Vector3 target = playerInTrigger.transform.position;
            Vector3 direction = (target - origin).normalized;
            float distance = Vector3.Distance(origin, target);

            RaycastHit hitInfo;

            LayerMask obstacleMask = LayerMask.GetMask("Obstacle");

            Debug.DrawRay(origin, direction * distance, Color.red);
            bool blocked = Physics.Raycast(origin, direction, out hitInfo, distance, obstacleMask);

            if (!blocked)
            {
                hasDamagedPlayer = true;

                IDamageable damageable = playerInTrigger.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage((int)damage);
                    SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Pounce_Hit_Player, playerInTrigger.transform.position);

                }

                Debuff stiffness = new Debuff(Define.DebuffType.DEBUFF_STIFFNESS, duration: 0.2f);
                BuffManager.Instance.AddDebuff(stiffness);
            }
        }
    }
}
