using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameStream : MonoBehaviour
{
    [SerializeField] GameObject flameStreamFx;
    [SerializeField] GameObject FlameStreamDamage;

    public void Shoot(params object[] pos)
    {
        // Effect 
        GameObject effect = Instantiate(flameStreamFx);
        Transform position = (Transform)pos[0];
        effect.transform.SetParent(position);
        effect.transform.position = position.position;
        effect.transform.rotation = position.rotation;

        // Damage
        GameObject damage = Instantiate(FlameStreamDamage);
        damage.transform.SetParent(position);
        damage.transform.position = position.position;
        damage.transform.rotation = position.rotation;
    }
}
