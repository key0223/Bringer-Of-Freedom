
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

[Serializable]
public class BodySkillSettings
{
    /* 짓밟기 */
    [Header("Stomp")]
    public float stompCheckRadius;
    public float stompDirectRadius; // 직격 범위
    public float stompAoeRadius; //  주변 범위
    public int stompMainDamage;
    public int stompAoeDamage ;

    /* 검 내려치기 */
    [Header("Sword")]
    public float swordCheckRadius;
    public Transform blackAttackPoint;
    public float swordDirectRadius; // 검 직격 범위
    public float swordAoeRadius; //  검 주변 범위
    public float swordMainDamage ;
    public float swordAoeDamage;
    public GameObject sword; // 공격용 검
    public GameObject wall1; // 낙하 방지 벽
    public GameObject wall2; 
    public GameObject cutsceneSword; // 컷씬용 검

    [Header("Dead Materials")]
    public Material[] bodyDeadMat;
}

[Serializable]
public class GreenSkillSettings
{
    [Header("Poison")]
    public float poisonCheckRadius;
    public float poisonMaxHeight;
    public Transform greenAttackPoint;
    public PoisonFlames poisonFlameSkill;
    [Header("Pounce")]
    public float pounceCheckRadius;
    public float pounceMaxHeight;
    public float pounceSpeed;
    public Pounce pounceSkill;
    [Header("Poison Gimmick")]
    public PoisonFlames_Gimmick poisonFlamesGimmickSkill;

    [Header("Dead Materials")]
    public Material[] greenDeadMat;
}

[Serializable]
public class BlueSkillSettings
{
    public float breathCheckRadius;
    public float blueMaxHeight;

    [Header("Right (ICE)")]
    public Transform blueRightAttackPoint;
    public IceBreath iceBreathSkill;
    [Header("Left (WATER)")]
    public Transform blueLeftAttackPoint;
    public WaterBreath waterBreathSkill;

    [Header("Dead Materials")]
    public Material[] blueDeadMat;
}

[Serializable]
public class RedSkillSettings
{
    public Transform redAttackPoint;
    [Header("Fireball")]
    public float fireballCheckRadius;
    public GameObject fireBallPrefab;
    public GameObject fireBallReadyFX;
    public float fireBallMaxHeight; // 스킬 사용 가능한 최대 높이
    public int fireBallToCreate ;
    public FakeFireball fakefireball;
    [Header("Punch")]
    public float punchCheckRadius;
    public float punchMaxHeight;
    public GameObject punchDamage;

    [Header("Dead Materials")]
    public Material[] redDeadMat;
}

[Serializable]
public class YellowSkillSettings
{
    public Transform yellowAttackPoint;
    [Header("Laser Beam")]
    public float laserCheckRadius;
    public float laserMinHeight;
    public float laserMaxHeight;
    public LaserBeam laserBeamSkill;

    [Header("Lightning Strike")]
    public float lightningStrikeCheckRadius;
    [Tooltip("타겟 지점을 중심으로 스킬이 생성되는 범위")]
    public float lightningStrikeRange;
    public float lightningStrikeMaxHeight;
    public LightningStrike lightningStrikeSkill;

    [Header("Light Of Judgment")]
    public float lightOfJudgmentCheckRadius;
    public int lightToCreate;
    [Tooltip("타겟 지점을 중심으로 스킬이 생성되는 범위")]
    public float lightOfJudgmentRange;
    public LightOfJudgment lightOfJudgmentSkill;

    [Header("Head Attack")]
    public float headCheckRadius;
    public float headMinHeight;
    public float headMaxHeight;
    public Transform headAttackPivot;
    public HeadAttack headAttackSkill;

    [Header("Radiate")]
    public float radiateCheckRadius;
    public float radiateMinHeight;
    public float radiateMaxHeight;
    public Transform radiateSkillPoint;
    public Radiate radiateSkill;

    [Header("Light Cylinder")]
    public int lightCylinderToCreate;
    public float lightCylinderCheckRadius;
    public float lightCylinderMinHeight;
    public float lightCylinderMaxHeight;
    public float chaseDuration;
    public float moveSpeed;
    public float rotateLerp;
    public float stoppingDistance;
    public float explodeDelay;
    public GameObject lightCylinderPrefab;
}

[Serializable]
public class IKSettings
{
    public bool rigResolved = false;
    [Header("Green")]
    public GameObject greenRig;
    public ChainIKConstraint greenChainIK;
    public MultiPositionConstraint greenMPos;
    public MultiRotationConstraint greenMRot;
    [Header("Red")]
    public GameObject redRig;
    public MultiPositionConstraint redMPos;
    public MultiRotationConstraint redMRot;
    [Header("Blue")]
    public GameObject blueRightRig;
    public GameObject blueLeftRig;
    public MultiPositionConstraint blueRightMPos;
    public MultiRotationConstraint blueRightMRot;
    public MultiPositionConstraint blueLeftMPos;
    public MultiRotationConstraint blueLeftMRot;
    [Header("Black")]
    public GameObject blackRig;
    public MultiPositionConstraint blackMPos;
    public MultiRotationConstraint blackMRot;
    [Header("Yellow")]
    public GameObject yellowRig;
    public MultiPositionConstraint yellowMPos;
    public MultiRotationConstraint yellowMRot;
}