using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePoisonFlames_Gimmick : SkillDamage
{
    [SerializeField] float damage = 300f; // 맞으면 즉사
    [SerializeField] GameObject poisonFlower;

    [SerializeField] GameObject groggyPointFX;

    bool hasDamagedPlayer = false;
     GameObject playerInTrigger = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           playerInTrigger = other.gameObject;
            hasDamagedPlayer = false;

            GameObject GroggyPointFX = Instantiate(groggyPointFX);
            Vector3 groggyPointPos = other.transform.position;
            groggyPointPos.y = 0;

            groggyPointFX.transform.position = groggyPointPos;
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

            Debug.DrawRay(origin, direction * distance, Color.red);
            LayerMask obstacleMask = LayerMask.GetMask("Obstacle");
            bool blocked = Physics.Raycast(origin, direction, out hitInfo, distance, obstacleMask);

            // 장애물이 없으면 공격 처리
            if (!blocked)
            {
                hasDamagedPlayer = true;

                Debuff poison = new Debuff(Define.DebuffType.DEBUFF_POISON, 10f, 1f);
                BuffManager.Instance.AddDebuff(poison);

                IDamageable damageable = playerInTrigger.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage((int)damage);
                }
            }
        }
    }
}
