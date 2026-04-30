
public class Define 
{
    public const string MESSAGE_PLAYER_SKILL_COOLDOWN = "재사용 대기시간이 남아있습니다.";

    public enum GamePhase
    {
        PHASE_1,
        PHASE_2,
        PHASE_3,
    }
    public enum QTEType
    {
        QTE_SINGLE,
        QTE_REPEAT,
        QTE_MULTI,
    }
    public enum DebuffType
    {
        // 지속 피해
        DEBUFF_BURN,      // 화상 (불)
        DEBUFF_POISON,    // 중독 (독)
                          // 행동 제한
        DEBUFF_PARALYSIS, // 마비 (전기)
        DEBUFF_FREEZE,    // 빙결 (얼음/슬로우)
                          // 경직/기절/넉백
        DEBUFF_STIFFNESS, // 경직
        DEBUFF_STUN,      // 기절
        DEBUFF_KNOCKBACK, // 넉백
        DEBUFF_KNOCKDOWN, // 다운
                          // 능력치 감소
        DEBUFF_SLOW,
        DEBUFF_DEFENSE,
        DEBUFF_ATTACK,

        /* ------ Monster Debuff ----- */
        DEBUFF_GROGGY,
        DEBUFF_DAMAGE,
    }
}
