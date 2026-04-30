using UnityEngine;
using static EnemyDefine;
public enum DamageGroupMode 
{ 
    None, 
    GroupOncePerSwing
}
public class DragonHitBox : MonoBehaviour,IDamageable
{
    MainMonsterController monsterController;

    [SerializeField] DragonType dragonType;
    [SerializeField] bool isWeakness;
    [SerializeField] float addedDamageRatio = 0.5f;

    [Header("Hit Settings")]
    [SerializeField] GameObject rootDamageable;
    [SerializeField] DamageGroupMode groupMode = DamageGroupMode.None;
    [SerializeField] string groupKey = "";
    [SerializeField] int priority = 100;

    public IDamageable RootDamageable { get { return rootDamageable.gameObject.GetComponent<IDamageable>(); } }
    public string GroupKey { get { return groupKey; } }
    public DamageGroupMode GroupMode { get { return groupMode; } }
    public int Priority { get { return priority; } }

    void Awake()
    {
        monsterController = GetComponentInParent<MainMonsterController>();
    }

    public void TakeDamage(int damage)
    {
        if (monsterController == null) return;

        float finalDamage = damage;

        if(dragonType == DragonType.DRAGON_YELLOW)
        {
            float damageModifier = damage * addedDamageRatio;
            finalDamage = isWeakness ? damage+damageModifier : damage - damageModifier;
        }
        monsterController.OnDamageDragon(dragonType,finalDamage);
    }

    public void AddMonsterDebuff(Debuff debuff)
    {
        monsterController.AddMonsterDebuff(dragonType, debuff);
    }
}
