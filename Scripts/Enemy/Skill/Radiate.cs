using UnityEngine;
using static EnemyDefine;

public class Radiate : MonoBehaviour
{
    public void Shoot(Transform pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.Radiate);
        effect.transform.SetParent(pos, false);
        Vector3 offset = new Vector3(0, 0, -13f);
        effect.transform.localPosition= offset;

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemySkillType.Radiate);
        damage.transform.SetParent(pos, false);
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Radiate,pos.position);
    }
    public void MakeBeforeFX(Transform pos)
    {
        // Effect 
        GameObject effect = PoolManager.Instance.EffectPooler.GetEnemyEffect(EnemyEffectType.PreRadiate);
        effect.transform.SetParent(pos, false);
    }
 
}
