using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

[Serializable]
public struct EnemyEffectsConfig
{
    public EnemyEffectType type;
    public GameObject prefab;
    public int count;
}
public class EffectPooler : MonoBehaviour
{
    [SerializeField] EnemyEffectsConfig[] enemyEffectsConfigs;

    Dictionary<EnemyEffectType, Queue<GameObject>> enemyEffectPool = new Dictionary<EnemyEffectType, Queue<GameObject>> ();
    Dictionary<EnemyEffectType,GameObject> effectPrefabDict = new Dictionary<EnemyEffectType, GameObject> ();

    void Awake()
    {
        InitEffectObject();
    }

    void InitEffectObject()
    {
        foreach(EnemyEffectsConfig config in enemyEffectsConfigs)
        {
            if(!effectPrefabDict.ContainsKey(config.type))
                effectPrefabDict.Add(config.type,config.prefab);

            Queue<GameObject> queue = new Queue<GameObject> ();
            for(int i = 0; i < config.count; i++)
            {
                GameObject effect = Instantiate(config.prefab, transform);
                effect.SetActive(false);
                queue.Enqueue(effect);
            }

            enemyEffectPool.Add(config.type, queue);
        }
    }

    public GameObject GetEnemyEffect(EnemyEffectType type)
    {
        if(enemyEffectPool.ContainsKey(type) && enemyEffectPool[type].Count>0)
        {
            GameObject obj = enemyEffectPool[type].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject prefab = GetPrefabByType(type);
            GameObject newObj = Instantiate(prefab,transform);
            newObj.SetActive(true);
            return newObj;
        }
    }

    public void ReturnEnemyEffect(EnemyEffectType type, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        enemyEffectPool[type].Enqueue(obj);
    }

    GameObject GetPrefabByType(EnemyEffectType type)
    {
        if(effectPrefabDict.TryGetValue(type, out GameObject prefab))
            return prefab;

        return null;
    }

    internal GameObject GetEnemyEffect(object enemyEffetyType)
    {
        throw new NotImplementedException();
    }
}
