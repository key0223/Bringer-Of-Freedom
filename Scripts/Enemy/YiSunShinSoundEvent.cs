using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Called by animation event */
public class YiSunShinSoundEvent : MonoBehaviour
{
    [SerializeField] Transform rightToe;
    [SerializeField] Transform leftToe;
    public void Walk()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Walk, rightToe.position);
    }
    public void WalkDrag()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Walk_Drag, leftToe.position);
    }
    public void BlueWakeup()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Blue_Wakeup);
    }
    public void YellowWakeup()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Yellow_Wakeup);
    }
    public void GreenDie()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Green_Die);
    }
    public void BlackDie()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Black_Die);
    }
    public void BlueDie()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Blue_Die);
    }
    public void RedDie()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Red_Die);
    }
    public void YellowDie()
    {
        SoundManager.Instance?.PlaySFX(SoundEffect.Enemy_Yellow_Die);
    }

}
