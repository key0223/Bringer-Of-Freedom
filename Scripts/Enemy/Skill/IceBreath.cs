using UnityEngine;
using static EnemyDefine;

public class IceBreath : MonoBehaviour
{
    [SerializeField] GameObject iceBreathFx;
    [SerializeField] GameObject iceBreathDamage;

    [Header("FX Settings")]
    [SerializeField] GameObject breathBeforeFX;
    [SerializeField] GameObject breathAfterFX;
    [SerializeField] float afterFXOffsetX;

    public void Shoot(Transform pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.IceBreath);
        effect.transform.SetParent(pos, false);

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.IceBreath);
        damage.transform.SetParent(pos, false);
    }
    public void MakeBeforeFX(Transform pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.ChargeBreath);
        effect.transform.SetParent(pos, false);
    }
    public void MakeAfterFX(Transform pos, Transform target)
    {
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.AfterBreath_2);
        Vector3 dir = (target.position - pos.position).normalized;

        Vector3 spawnPos = pos.position + dir * afterFXOffsetX;
        effect.transform.position = spawnPos;
    }
    public float GetEffectDuration()
    {
        SkillDamage damage = iceBreathDamage.GetComponent<SkillDamage>();

        return damage.Duration;
    }
}
