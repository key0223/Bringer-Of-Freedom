using UnityEngine;
using static Define;

public class DamageLightningStrike : SkillDamage
{
    [SerializeField] float damage = 10f;
    [SerializeField] GameObject groggyPointFX;

    bool hasHit = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        hasHit = true;

        bool hitPlayer = other.CompareTag("Player");
        bool hitGround = other.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (hitPlayer)
        {
            Debuff stun = new Debuff(DebuffType.DEBUFF_STUN, duration: 3f);
            BuffManager.Instance.AddDebuff(stun);

            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
                damageable.TakeDamage((int)damage);
        }
        else if(hitGround)
        {
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            contactPoint.y = 0.5f;

            GameObject GroggyPointFX = Instantiate(groggyPointFX,contactPoint,Quaternion.identity);
        }
    }
}
