using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePoison : SkillDamage
{
    //[SerializeField] float damagePerSec = 5f;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
    }
}
