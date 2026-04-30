using UnityEngine;

public class Pounce : MonoBehaviour
{
    [SerializeField] GameObject pounceFx;
    [SerializeField] GameObject pounceDamage;

    public void Shoot(params object[] pos)
    {
        Transform position = (Transform)pos[0];

        // Effect 
        //GameObject effect = Instantiate(pounceFx);
        //effect.transform.SetParent(position, false);

        // Damage
        GameObject damage = PoolManager.Instance.SkillDamagePooler.GetEnemySkill(EnemyDefine.EnemySkillType.Pounce);
        damage.transform.SetParent(position, false);

    }
}
