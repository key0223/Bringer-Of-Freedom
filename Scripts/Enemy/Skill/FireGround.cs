using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGround : MonoBehaviour
{
    [SerializeField] GameObject fireGroundFX;
    [SerializeField] GameObject fireGroundDamage;

    public void Shoot(params object[] pos)
    {
        // Damage
        GameObject newWaterSkill = Instantiate(fireGroundDamage);
        Vector3 position = (Vector3)pos[0];

        newWaterSkill.transform.position = position;

        GameObject newEffect = Instantiate(fireGroundFX);
        newEffect.transform.position = position;
    }
}
