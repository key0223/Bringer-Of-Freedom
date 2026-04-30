public enum PlayerInputAction
{
    RUN,
    JUMP,
    MOUSE_LEFT_CLICK,
    MOUSE_RIGHT_CLICK,
    CHARGE_ATTACK,
    EVASION,
    LOCK_ON,
    HOOK_TOGGLE,
    STANCE_CHANGE,
    SKILL_1,
    SKILL_2,
    SKILL_3,
    SKILL_4,
    INTERACT,
    ULTIMATE_SKILL,
    USE_HEALTH_POTION,
    USE_MANA_POTION,
    PAUSE,
    QTE,
}
public interface IInputController
{
    bool GetKeyUp(PlayerInputAction action);
    bool GetKeyDown(PlayerInputAction action);
    bool GetKey(PlayerInputAction action);
    float GetAxis(string axisName);
    float GetAxisRaw(string axisName);
}
