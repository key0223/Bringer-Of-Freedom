using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterImpact : MonoBehaviour    
{
    [SerializeField] GameObject waterImpactFx;
    [SerializeField] GameObject waterSkillPrefab; // Damage Prefab

    public void Shoot(params object[] pos)
    {
        Vector3 position = (Vector3)pos[0];

        // Damage
        GameObject newWaterSkill = Instantiate(waterSkillPrefab);
        newWaterSkill.transform.position = position;

        GameObject newEffect = Instantiate(waterImpactFx);
        newEffect.transform.position = position;
    }
}
