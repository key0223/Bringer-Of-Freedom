using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

[Serializable]
public struct EnemySkillConfig
{
    public EnemySkillType type;
    public GameObject prefab;
    public int count;
}
public class SkillDamagePooler : MonoBehaviour
{

   [SerializeField] EnemySkillConfig[] enemySkillConfigs;
    
    Dictionary<EnemySkillType, Queue<GameObject>> enemySkillPool = new Dictionary<EnemySkillType, Queue<GameObject>>();
    Dictionary<EnemySkillType,GameObject> skillPrefabDict = new Dictionary<EnemySkillType, GameObject>();


    void Awake()
    {
        InitSkillObject();
    }
    void InitSkillObject()
    {
        foreach(EnemySkillConfig config in enemySkillConfigs)
        {
            if(!skillPrefabDict.ContainsKey(config.type))
                skillPrefabDict.Add(config.type,config.prefab);

            Queue<GameObject> queue = new Queue<GameObject>();

            for(int i = 0; i < config.count; i++)
            {
                GameObject skill = Instantiate(config.prefab, transform);
                skill.SetActive(false);
                queue.Enqueue(skill);
            }

            enemySkillPool.Add(config.type, queue);
        }
    }

    public GameObject GetEnemySkill(EnemySkillType type)
    {
        if(enemySkillPool.ContainsKey(type)&& enemySkillPool[type].Count>0)
        {
            GameObject obj = enemySkillPool[type].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject prefab = GetPrefabByType(type);
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(true);
            return newObj;
        }
    }

    public void ReturnEnemySkill(EnemySkillType type, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        enemySkillPool[type].Enqueue(obj);
    }
    GameObject GetPrefabByType(EnemySkillType type)
    {
        if(skillPrefabDict.TryGetValue(type, out GameObject prefab))
            return prefab;

        return null;
    }
}
