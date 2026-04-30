using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTornado : MonoBehaviour
{
    [SerializeField] GameObject iceTornadoFx;
    [SerializeField] GameObject iceTornadoDamage;

    public void Shoot(params object[] pos)
    {
        // Effect 
        GameObject effect = Instantiate(iceTornadoFx);

        Transform position = (Transform)pos[0];
        effect.transform.SetParent(position, false);

        // Damage
        GameObject damage = Instantiate(iceTornadoDamage);
        damage.transform.SetParent(position, false);
    }

    public float GetEffectDuration()
    {
        SkillDamage damage = iceTornadoDamage.GetComponent<SkillDamage>();

        return damage.Duration;
    }
}
