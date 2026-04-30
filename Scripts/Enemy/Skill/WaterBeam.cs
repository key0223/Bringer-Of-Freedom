using UnityEngine;

public class WaterBeam : MonoBehaviour
{
    [SerializeField] GameObject waterBeamFx;
    [SerializeField] GameObject waterBeamDamage;
    public void Shoot(params object[] pos)
    {
        Transform position = (Transform)pos[0];

        // Effect 
        GameObject effect = Instantiate(waterBeamFx);
        effect.transform.SetParent(position, false);

        // Damage
        GameObject damage = Instantiate(waterBeamDamage);
        damage.transform.SetParent(position, false);
    }
    public float GetEffectDuration()
    {
        SkillDamage damage = waterBeamDamage.GetComponent<SkillDamage>();

        return damage.Duration;
    }
}
