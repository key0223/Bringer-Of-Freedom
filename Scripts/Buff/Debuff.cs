using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Debuff
{
    public DebuffType DebuffType{ get; private set; }
    public float Value {  get; private set; }
    public float Duration { get; private set; }

    public Debuff(DebuffType debuffType,float duration,float value = 0)
    {
        DebuffType = debuffType;
        Duration = duration;
        Value = value;
    }

    public bool Tick(float deltaTime)
    {
        Duration -= deltaTime;
        return Duration <= 0f;
    }
}
