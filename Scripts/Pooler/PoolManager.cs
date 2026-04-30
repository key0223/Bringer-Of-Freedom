using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    SkillDamagePooler skillDamagePooler;
    EffectPooler effectPooler;

    public SkillDamagePooler SkillDamagePooler { get { return skillDamagePooler; } }
    public EffectPooler EffectPooler { get { return effectPooler; } }
    protected override void Awake()
    {
        base.Awake();
        skillDamagePooler = GetComponentInChildren<SkillDamagePooler>();
        effectPooler = GetComponentInChildren<EffectPooler>();
    }


}
