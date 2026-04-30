using SimpleInputNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : MonoBehaviour
{
    BuffUI buffUI;
    DragonHealthUI dragonHealthUI;
    [SerializeField] private GroggyIndicator groggyIndicator;

    [Header("PC UI References")]
    [SerializeField] GameObject skills;
    [SerializeField] GameObject items;
    [SerializeField] GameObject hookToggle;
    [SerializeField] GameObject keyGuide;

    [Header("Mobile UI References")]
    [SerializeField] GameObject joystickUI;
    [SerializeField] GameObject playerHpbarMobile;


    public BuffUI BuffUI { get { return buffUI; } }
    public DragonHealthUI DragonHealthUI { get { return dragonHealthUI; } }
    public GroggyIndicator GroggyIndicator { get { return groggyIndicator; } } 

    void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        SetPCUI(false);
        SetMobileUI(true);
#elif UNITY_STANDALONE || UNITY_WEBPLAYER
        SetMobileUI(false);
        SetPCUI(true);
#endif

        buffUI = GetComponentInChildren<BuffUI>(true);
        dragonHealthUI = GetComponentInChildren<DragonHealthUI>(true);
        if (groggyIndicator != null) groggyIndicator.Hide();
    }   
    void Start()
    {

    }

    void SetPCUI(bool show)
    {
        skills.SetActive(show);
        items.SetActive(show);
        hookToggle.SetActive(show);
        keyGuide.SetActive(show);
    }
 
    void SetMobileUI(bool show)
    {
        joystickUI.SetActive(show);
        playerHpbarMobile.SetActive(show);
    }
    
}
