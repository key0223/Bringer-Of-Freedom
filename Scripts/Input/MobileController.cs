
using UnityEngine;
using static Unity.Collections.Unicode;

public class MobileController : MonoBehaviour, IInputController
{
    [Header("Move")]
    [SerializeField] string runButton = "Run";

    [Header("Actions")]
    [SerializeField] string attackButton = "Attack";
    [SerializeField] string jumpButton = "Jump";
    [SerializeField] string chargeAttackButton = "ChargeAttack";
    [SerializeField] string evasionButton = "Evasion";
    [SerializeField] string lockOnButton = "LockOn";
    [SerializeField] string hookToggleButton = "HookToggle";
    [SerializeField] string stanceChangeButton = "StanceChange";

    [SerializeField] string hookButton = "Hook";

    [Header("Skills")]
    [SerializeField] string skill1Button = "Skill1";
    [SerializeField] string skill2Button = "Skill2";
    [SerializeField] string skill3Button = "Skill3";
    [SerializeField] string skill4Button = "Skill4";
    [SerializeField] string ultimateSkillButton = "UltimateSkill";

    [Header("Items & Interaction")]
    [SerializeField] string interactButton = "Interact";
    [SerializeField] string useHealthPotionButton = "HealthPotion";
    [SerializeField] string useManaPotionButton = "ManaPotion";
    [SerializeField] string qteButton = "Qte";

    [Header("UI")]
    [SerializeField] string pauseButton = "Pause";

    public bool GetKeyUp(PlayerInputAction action)
    {
        return SimpleInput.GetButtonUp(GetButtonName(action));
    }

    public bool GetKeyDown(PlayerInputAction action)
    {
        return SimpleInput.GetButtonDown(GetButtonName(action));
       
    }
    public bool GetKey(PlayerInputAction action)
    {
        return SimpleInput.GetButton(GetButtonName(action));
    }

    public float GetAxis(string axisName)
    {
        return SimpleInput.GetAxis(axisName);
    }

    public float GetAxisRaw(string axisName)
    {
        return SimpleInput.GetAxisRaw(axisName);
    }

    string GetButtonName(PlayerInputAction action)
    {
        switch (action)
        {
            case PlayerInputAction.RUN: return runButton;
            case PlayerInputAction.JUMP: return jumpButton;
            case PlayerInputAction.EVASION: return evasionButton;
            case PlayerInputAction.LOCK_ON: return lockOnButton;
            case PlayerInputAction.HOOK_TOGGLE: return hookToggleButton;
            case PlayerInputAction.STANCE_CHANGE: return stanceChangeButton;
            case PlayerInputAction.SKILL_1: return skill1Button;
            case PlayerInputAction.SKILL_2: return skill2Button;
            case PlayerInputAction.SKILL_3: return skill3Button;
            case PlayerInputAction.SKILL_4: return skill4Button;
            case PlayerInputAction.CHARGE_ATTACK: return chargeAttackButton;
            case PlayerInputAction.INTERACT: return interactButton;
            case PlayerInputAction.ULTIMATE_SKILL: return ultimateSkillButton;
            case PlayerInputAction.USE_HEALTH_POTION: return useHealthPotionButton;
            case PlayerInputAction.USE_MANA_POTION: return useManaPotionButton;
            case PlayerInputAction.PAUSE: return pauseButton;
            case PlayerInputAction.MOUSE_LEFT_CLICK: return attackButton;
            case PlayerInputAction.MOUSE_RIGHT_CLICK: return hookButton;
            case PlayerInputAction.QTE:return qteButton;
            default: return "";
        }
    }

}
