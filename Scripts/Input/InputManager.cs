
using UnityEngine;

public class InputManager : SingletonMonobehaviour<InputManager>
{
    [SerializeField] GameObject joystickUI;
    public IInputController InputController { get; private set; }

    protected override void Awake()
    {
        base.Awake();

#if UNITY_ANDROID || UNITY_IOS
        InputController = GetComponent<MobileController>();
        joystickUI.gameObject.SetActive(true);
#elif UNITY_STANDALONE || UNITY_WEBPLAYER
        InputController = GetComponent<PCController>();
        joystickUI.gameObject.SetActive(false);
#endif
    }
}
