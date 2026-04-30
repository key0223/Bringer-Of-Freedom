using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

public class FakeFireball : MonoBehaviour
{
    Rigidbody rigid;

    [SerializeField] float moveFactor;
    [SerializeField] float lifeTime;
    [SerializeField] bool useGravity;

    Vector3 shootDir;
    float expiredTimer = 0f;
    Transform reloadTransform;


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
        bool hitGround = other.gameObject.layer == LayerMask.NameToLayer("Ground");

        Vector3 contactPoint = other.ClosestPoint(transform.position);
        contactPoint.y = 0.5f;

        if(hitGround)
        {
            PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.Fireball_Indicator);
        }
    }
}
