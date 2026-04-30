using UnityEngine;

public class PCController :MonoBehaviour,IInputController
{
    [Header("Move")]
    [SerializeField] KeyCode runKey = KeyCode.LeftShift;

    [Header("Actions")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode chargeAttackKey = KeyCode.LeftControl;
    [SerializeField] KeyCode evasionKey = KeyCode.LeftAlt;
    [SerializeField] KeyCode lockOnKey = KeyCode.Plus;
    [SerializeField] KeyCode hookToggleKey = KeyCode.C;
    [SerializeField] KeyCode stanceChangeKey = KeyCode.Tab;

    [Header("Skills")]
    [SerializeField] KeyCode skill1Key = KeyCode.Alpha1;
    [SerializeField] KeyCode skill2Key = KeyCode.Alpha2;
    [SerializeField] KeyCode skill3Key = KeyCode.Alpha3;
    [SerializeField] KeyCode skill4Key = KeyCode.Alpha4;
    [SerializeField] KeyCode ultimateSkillKey = KeyCode.V;

    [Header("Items & Interaction")]
    [SerializeField] KeyCode interactKey = KeyCode.F;
    [SerializeField] KeyCode useHealthPotionKey = KeyCode.E;
    [SerializeField] KeyCode useManaPotionKey = KeyCode.R;
    [SerializeField] KeyCode qteKey = KeyCode.Q;

    [Header("UI")]
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    
    public bool GetKeyUp(PlayerInputAction action)
    {
        KeyCode key = GetEffectiveKey(action);
        return key != KeyCode.None && Input.GetKeyUp(key);

    }

    public bool GetKeyDown(PlayerInputAction action)
    {
        KeyCode key = GetEffectiveKey(action);
        return key != KeyCode.None ? Input.GetKeyDown(key) :
              action == PlayerInputAction.MOUSE_LEFT_CLICK ? Input.GetMouseButtonDown(0) :
              action == PlayerInputAction.MOUSE_RIGHT_CLICK ? Input.GetMouseButtonDown(1) : false;
    }
    public bool GetKey(PlayerInputAction action)
    {
        KeyCode key = GetEffectiveKey(action);
        return key != KeyCode.None && Input.GetKey(key);
    }
    public float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }
    public float GetAxisRaw(string axisName)
    {
        return Input.GetAxisRaw(axisName);
    }

    /* 실제로 사용할 키 반환 */
    KeyCode GetEffectiveKey(PlayerInputAction action)
    {
        /* csv 키 우선 */
        if(DataManager.Instance != null)
        {
            if (DataManager.Instance.RuntimeKeyDict.TryGetValue(action, out KeyCode key))
                return key;
        }

        /* 인스펙터 기본값 */

        switch (action)
        {
            case PlayerInputAction.RUN:  return runKey;
            case PlayerInputAction.JUMP: return jumpKey;
            case PlayerInputAction.EVASION: return evasionKey;
            case PlayerInputAction.LOCK_ON: return lockOnKey;
            case PlayerInputAction.HOOK_TOGGLE: return hookToggleKey;
            case PlayerInputAction.STANCE_CHANGE: return stanceChangeKey;
            case PlayerInputAction.SKILL_1: return skill1Key;
            case PlayerInputAction.SKILL_2: return skill2Key;
            case PlayerInputAction.SKILL_3: return skill3Key;
            case PlayerInputAction.SKILL_4: return skill4Key;
            case PlayerInputAction.CHARGE_ATTACK: return chargeAttackKey;
            case PlayerInputAction.INTERACT: return interactKey;
            case PlayerInputAction.ULTIMATE_SKILL: return ultimateSkillKey;
            case PlayerInputAction.USE_HEALTH_POTION: return useHealthPotionKey;
            case PlayerInputAction.USE_MANA_POTION: return useManaPotionKey;
            case PlayerInputAction.PAUSE: return pauseKey;
            case PlayerInputAction.QTE: return qteKey;
            default: return KeyCode.None;
        }
    }


}
