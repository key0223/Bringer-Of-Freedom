using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ApplyDebuff : MonoBehaviour
{
    [SerializeField] DebuffType debuffType;

    bool hasDamagedPlayer = false;
    GameObject playerInTrigger = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = other.gameObject;
            hasDamagedPlayer = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerInTrigger)
        {
            playerInTrigger = null;
            hasDamagedPlayer = false;
        }
    }

    void Update()
    {
        if (playerInTrigger != null && !hasDamagedPlayer)
        {
            hasDamagedPlayer = true;

            Debuff debuff = new Debuff(debuffType, duration: 2f, value: 1);
            BuffManager.Instance.AddDebuff(debuff);
        }
    }
}
