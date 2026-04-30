using UnityEngine;
using static EnemyDefine;

public class LightCylinder : MonoBehaviour
{
    [SerializeField] GameObject indicatorFX;

    Transform target;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateLerp = 0.2f;
    [SerializeField] float stoppingDistance = 0.5f;
    [Space(5f)]
    [SerializeField] float explodeDelay = 0.2f;
    float chaseDuration = 3f;

    float chaseTimer = 0f;
    float explodeTimer = 0f;
    bool isChasing = true;
    bool exploded = false;

    public void Init(Transform target, float chaseDuration)
    {
        this.target = target;
        this.chaseDuration = chaseDuration;
    }
    void Update()
    {
        if (exploded || target == null) return;
        if (isChasing)
        {
            chaseTimer += Time.deltaTime;
            if (target != null)
            {
                Vector3 to = target.position - transform.position;
                float dist = to.magnitude;

                if (dist > stoppingDistance)
                {
                    transform.position = Vector3.MoveTowards(
                    transform.position,
                    target.position,
                    moveSpeed * Time.deltaTime
                ); 
                }
            }

            // 정지
            if (chaseTimer >= chaseDuration)
            {
                isChasing = false;
                explodeTimer = 0f;
            }
        }
        else
        {
            explodeTimer += Time.deltaTime;
            if(explodeTimer >= explodeDelay)
                Explode();
        }
    }

    void Explode()
    {
        if(exploded) return;
        exploded = true;

        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.LightCylinder_Explosion);
        effect.transform.position = transform.position;

        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.LightCylinder);
        damage.transform.position = transform.position;

        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_LightCylinder,transform.position);

        Destroy(gameObject);
    }

}
