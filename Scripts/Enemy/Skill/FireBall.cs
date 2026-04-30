using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyDefine;

public class FireBall : MonoBehaviour
{
    Rigidbody rigid;

    [SerializeField] float moveFactor;
    [SerializeField] float lifeTime;
    [SerializeField] bool useGravity;

    [Space(10)]
    [SerializeField] GameObject groggyPointFX;

    Vector3 shootDir;
    float expiredTimer = 0f;
    Transform reloadTransform;

    bool hasHit = false;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        expiredTimer += Time.deltaTime;

        if (expiredTimer > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void Shooting()
    {
        if (reloadTransform)
            transform.position = reloadTransform.position;

        transform.rotation = Quaternion.LookRotation(transform.forward);

        Debug.DrawRay(transform.position, transform.forward * 5, Color.red, 2f);

        rigid.angularVelocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
        rigid.AddForce(transform.forward * moveFactor, ForceMode.Force);

        if (useGravity)
            rigid.useGravity = true;

    }
    public void SetMuzzleTransform(Transform transform)
    {
        reloadTransform = transform;
    }
    public void SetDirection(Vector3 direction)
    {
        shootDir = direction.normalized;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        bool hitPlayer = other.CompareTag("Player");
        bool hitGround = other.gameObject.layer == LayerMask.NameToLayer("Ground");

        if(!hitPlayer && !hitGround) return;

        hasHit = true;

        Vector3 contactPoint = other.ClosestPoint(transform.position);
        contactPoint.y = 0.5f;

        GameObject newExplosionFX = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.Fireball_Explosion);
        newExplosionFX.transform.position = contactPoint;
        newExplosionFX.transform.rotation = Quaternion.identity;

        if(hitGround)
        {
            GameObject newGroggyPointFX = Instantiate(groggyPointFX,contactPoint,Quaternion.identity);
        }
        else if (hitPlayer)
        {
            GameObject newDamage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.FireBall);
            newDamage.transform.position = contactPoint;
            newDamage.transform.rotation = Quaternion.identity;
        }
        
        Destroy(gameObject);
    }

}
