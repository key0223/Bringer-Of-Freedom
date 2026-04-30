using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase1GimmickTrigger : MonoBehaviour, IDamageable
{
    bool isTriggered = false;
    public bool IsTriggered { get { return isTriggered; } }

    public void TakeDamage(int damage)
    {
        if (isTriggered) return;
        isTriggered = true;
    }

    public void TakeDamage(float damage, EnemyDefine.EnemyHitType hitType)
    {
        if (isTriggered) return;
        isTriggered = true;
    }
}
