using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnShoulderTrigger : MonoBehaviour
{
    GameObject playerInTrigger = null;

    MainMonsterController monsterController;
    void Awake()
    {
        monsterController = FindObjectOfType<MainMonsterController>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = other.gameObject;
            monsterController.SkillController.IsPlayerOnShoulder = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerInTrigger)
        {
            playerInTrigger = null;
            monsterController.SkillController.IsPlayerOnShoulder = false;
        }
    }
    
}
