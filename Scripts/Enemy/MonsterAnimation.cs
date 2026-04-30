using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    MainMonsterController monsterController;


    public void Init(MainMonsterController controller)
    {
        monsterController = controller;
    }

    void LateUpdate()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        monsterController.Anim.SetBool("isMoving", monsterController.IsMoving);
        monsterController.Anim.SetBool("isStomping", monsterController.IsStomping);
        monsterController.Anim.SetBool("isSwordAttacking", monsterController.IsSwordAttacking);

        monsterController.Anim.SetBool("isGreenPoisonBreathing", monsterController.IsGreenPoisonBreathing);
        monsterController.Anim.SetBool("isPouncing", monsterController.IsPouncing);

        monsterController.Anim.SetBool("isShootingFireball", monsterController.IsShootingFireball);
        monsterController.Anim.SetBool("isPunching", monsterController.IsPunching);

        monsterController.Anim.SetBool("isIceBreathing", monsterController.IsIceBreathing);
        monsterController.Anim.SetBool("isWaterBreathing", monsterController.IsWaterBreathing);

        monsterController.Anim.SetBool("isLaserBeaming", monsterController.IsLaserBeaming);
        monsterController.Anim.SetBool("isHeadAttack",monsterController.IsHeadAttack);
        monsterController.Anim.SetBool("isRadiate",monsterController.IsRadiateAttack);
        monsterController.Anim.SetBool("isPerformingLightOfJudgment", monsterController.IsPerformingLightOfJudgment);
        monsterController.Anim.SetBool("isPerformingLightningStrike", monsterController.IsMoving);
        monsterController.Anim.SetBool("isSwordAttacking_Gimmick", monsterController.IsSwordAttacking_Gimmick); 
        monsterController.Anim.SetBool("isPoisonAttacking_Gimmick", monsterController.IsPoisonAttacking_Gimmick);

        monsterController.Anim.SetBool("isBlackDown", monsterController.IsBlackDown);
        monsterController.Anim.SetBool("isBlueAwake", monsterController.IsBlueAwake);
        monsterController.Anim.SetBool("isYellowAwake", monsterController.IsYellowAwake);
        monsterController.Anim.SetBool("isYellowDown", monsterController.IsYellowDown);

        monsterController.Anim.SetBool("isGreenDead", monsterController.IsGreenDead);
        monsterController.Anim.SetBool("isBlackDead", monsterController.IsBlackDead);
        monsterController.Anim.SetBool("isBlueDead", monsterController.IsBlueDead);
        monsterController.Anim.SetBool("isRedDead", monsterController.IsRedDead);
        monsterController.Anim.SetBool("isYellowDead", monsterController.IsYellowDead);
        monsterController.Anim.SetBool("isYiSunShinDead", monsterController.IsYiSunShinDead);
    }
}
