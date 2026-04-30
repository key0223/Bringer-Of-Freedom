using System.Collections;
using UnityEngine;

public class QTEHoldPress : QTEBase
{
    public KeyCode keyToHold;
    public float chargeSpeed = 0.5f; // 초당 상승량
    public float drainSpeed = 0.3f;  // 초당 감소량
    public float targetGauge = 1f;

    public override IEnumerator CoExecute()
    {
        float timer = 0f;
        float currentGauge = 0f;
        bool isSuccess = false;

        UIManager.Instance.Popup.ShowQTE();
        UIManager.Instance.Popup.QteUI.SetQTE();

        while (timer < duration)
        {
            // Input.GetKey를 사용하여 누르고 있는 상태를 확인합니다.
            if (InputManager.Instance.InputController.GetKey(PlayerInputAction.QTE))
            {
                currentGauge += chargeSpeed * Time.unscaledDeltaTime;

                // 이펙트
                //if (Input.GetKeyDown(keyToHold))
                //{
                //    UIManager.Instance.Popup.QteUI.OnKeyPressedEffect();
                //}
            }
            else
            {
                currentGauge -= drainSpeed * Time.unscaledDeltaTime;
            }

            // 2. 게이지 클램핑 및 UI 업데이트
            currentGauge = Mathf.Clamp(currentGauge, 0f, targetGauge); 
            UIManager.Instance.Popup.QteUI.UpdateTimer(currentGauge / targetGauge); 

            // 3. 성공 조건 체크
            if (currentGauge >= targetGauge)
            {
                isSuccess = true;
                break;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // 4. 결과 처리
        if (isSuccess)
        {
            Debug.Log("Hold QTE Success");
            InvokeSuccess(); 
        }
        else
        {
            Debug.Log("Hold QTE Fail");
            InvokeFail();
        }

        UIManager.Instance.Popup.HideQTE();
    }
}
