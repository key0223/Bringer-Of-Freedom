using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DamageLightning : SkillDamage
{
    //[SerializeField] float damage = 10f;
    [SerializeField] ParticleSystem particle;

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debuff stun = new Debuff(DebuffType.DEBUFF_STUN, duration: 3f);
            BuffManager.Instance.AddDebuff(stun);
        }
        //Debug.Log($"Effect Collision : {other.name}");
    }

    protected override void StopDamage()
    {

    }
}
