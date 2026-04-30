public class EnemyDefine 
{
    public enum PhaseState
    {
        IDLE,
        GIMMICK_EXECUTING,
        GIMMICK_COMPLETED,
        GIMMICK_AWAITING_COMPLETION,
        PHASE2_GIMMICK_READY // [추가] 2페이즈 기믹 준비 상태
    }

    public enum EnemySkillState
    {
        IDLE,
        PREPARING,
        EXECUTING,
        COOLINGDOWN,
    }
    public enum DragonType
    {
        BODY,
        DRAGON_GREEN,
        DRAGON_RED,
        DRAGON_BLUE,
        DRAGON_BLACK,
        DRAGON_YELLOW,
    }
    public enum EnemyCheckPivot
    {
        BODY,
        LEFT,
        RIGHT,
    }
    public enum EnemyGrade
    {
        EPIC = 1,
        LEGENDARY = 2,
        MYTHIC = 3,
        TRANSCENDENT = 4,
    }

    public enum EnemyAttackType
    {
        MELEE =1,
        RANGED =2,
        BOTH =3,
    }
    public enum EnemySkillType
    {
        Stomp,
        Sword,
        Poison,
        Pounce,
        FireBall,
        IceBreath, // Right
        WaterBreath, // Left
        LaserBeam,
        FlameStream,
        LightOfJudgment,
        LightningStrike,
        Punch,
        HeadAttack,
        Radiate,
        LightCylinder,
        Poison_Gimmick,
        FireBall_Obj,
        //HeavyRain,
    }
    public enum EnemyEffectType
    {
        AfterBreath_1,
        AfterBreath_2,
        ChargeBreath,
        Fireball_Explosion,
        Fireball_Indicator,
        Fireball_Ready,
        IceBreath,
        IceBreath_Hit,
        Judgement,
        LaserBeam,
        LaserBeam_Hit,
        LightCylinder_Explosion,
        LightCylinder_Indicator,
        Lightning_Indicator,
        LightningStrike,
        LightningStrike_Indicator,
        LightOfJudgement,
        Poison,
        PoisonFlames,
        PreRadiate,
        Radiate,
        WaterBreath,
    }

    public enum EnemyState
    {
        NONE,
        IDLE,
        MOVE,
        GROGGY,
        ATTACK,
        DAMAGE,
        GIMMICK,
        DEAD,
    }

    public enum GreenDragonState
    {
        NONE,
        IDLE,
        ATTACK,
        DEAD,
    }
    public enum BlackDragonState
    {
        NONE,
        IDLE,
        ATTACK,
        DEAD,
    }
    public enum RedDragonState
    {
        NONE,
        IDLE,
        ATTACK,
        DEAD,
    }
    public enum BlueDragonState
    {
        NONE,
        IDLE,
        ATTACK,
        DEAD,
    }
    public enum YellowDragonState
    {
        NONE,
        IDLE,
        ATTACK,
        GROGGY,
        DEAD,
    }
    public enum EnemyMoveType
    {
        FOLLOW,
        PATROL,
    }

    public enum EnemyHitType
    {
        NORMAL,
        WEAKPOINT,
    }
}
