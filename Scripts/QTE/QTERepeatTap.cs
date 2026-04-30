using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTERepeatTap : QTEBase
{
    public KeyCode keyToTap;
    public float gaugeUpPerTap;          // 한 번 눌렀을 때 증가량
    public float gaugeDownSpeed;         // 초당 감소량
    public float targetGauge = 1f;

    public override IEnumerator CoExecute()
    {
        float timer = 0f;
        float gauge = 0f;

        UIManager.Instance.Popup.ShowQTE();
        UIManager.Instance.Popup.QteUI.SetQTE();

        while(timer <duration &&  gauge < targetGauge)
        {

            if (InputManager.Instance.InputController.GetKeyDown(PlayerInputAction.QTE))
            {
                gauge += gaugeUpPerTap;
                UIManager.Instance.Popup.QteUI.OnKeyPressedEffect();
            }
            gauge -= gaugeDownSpeed * Time.unscaledDeltaTime;
            gauge = Mathf.Clamp01(gauge);

            UIManager.Instance.Popup.QteUI.UpdateTimer(gauge);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (gauge >= targetGauge)
        {
            Debug.Log("Success");
            InvokeSuccess();
        }
        else
        {
            Debug.Log("Fail");
            InvokeFail();
        }

        UIManager.Instance.Popup.HideQTE();

    }
}
