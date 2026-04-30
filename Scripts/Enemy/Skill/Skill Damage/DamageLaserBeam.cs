using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLaserBeam : SkillDamage
{
    [SerializeField] float damagePerSec = 2f;
    [SerializeField] float damageInterval = 1f;

    [Space(5f)]
    [SerializeField] LayerMask groundMask;
    [SerializeField] GameObject hitFxPrefab;
    [SerializeField] float rayLength = 100f;
    [SerializeField] float loseHitTimeout = 0.2f; // Stay 미명중 지속 시 정리

    GameObject hitFx;
    bool following = false;
    float notHitTimer = 0f;

    IDamageable damageable;
    Coroutine coDamage;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
                coDamage = StartCoroutine(CoDamage());
        }

        bool hitGround = other.gameObject.layer == LayerMask.NameToLayer("Ground");
        if (hitGround)
        {
            CleanupHitFx();
            Vector3 dir = transform.forward;

            if(Physics.Raycast(transform.position, dir, out RaycastHit hit, rayLength, groundMask))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;

                hitFx = Instantiate(hitFxPrefab,point, Quaternion.identity);
                following = true;
                notHitTimer = 0f;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!following || hitFx == null) return;

        bool hitGround = other.gameObject.layer == LayerMask.NameToLayer("Ground");
        if (hitGround)
        {
            Vector3 dir = transform.forward;
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, rayLength, groundMask))
            {
                hitFx.transform.position = hit.point;
                notHitTimer = 0f;
            }
            else /* Ray가 안맞으면 */
            {
                notHitTimer += Time.deltaTime;
                if (notHitTimer >= loseHitTimeout)
                {
                   CleanupHitFx();
                }
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

        bool hitGround = other.gameObject.layer == LayerMask.NameToLayer("Ground");
        if (hitGround)
        {
            CleanupHitFx();
        }
    }
    void CleanupHitFx()
    {
        following = false;
        notHitTimer = 0f;

        if (hitFx != null)
        {
            Destroy(hitFx);
            hitFx = null;
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

    void OnDestroy()
    {
        CleanupHitFx();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;

        Gizmos.DrawLine(origin, origin + dir * rayLength);
        Gizmos.DrawSphere(origin, 0.02f);
    }

}
