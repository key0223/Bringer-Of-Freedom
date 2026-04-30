using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEMultiKey : QTEBase
{
    public KeyCode[] keys;
    public float timeBetweenKeys;
    public int currentKeyIndex;

    public override IEnumerator CoExecute()
    {
        currentKeyIndex = 0;
        bool success = true;
        float timeForCurrentKey = 0f;

        UIManager.Instance.Popup.ShowQTE();
        UIManager.Instance.Popup.QteUI.SetQTE();

        while (currentKeyIndex < keys.Length)
        {
            bool keyPressed = false;
            timeForCurrentKey = 0f;

            UIManager.Instance.Popup.QteUI.SetMultikey(keys,currentKeyIndex);

            while (timeForCurrentKey < timeBetweenKeys && !keyPressed)
            {
                timeForCurrentKey += Time.unscaledDeltaTime;

                if (Input.GetKeyDown(keys[currentKeyIndex]))
                {
                    keyPressed = true;
                    currentKeyIndex++;

                    // Key Press Effect

                    UIManager.Instance.Popup.QteUI.OnKeyPressedEffect();
                }

                float ratio = 1f - (timeForCurrentKey / timeBetweenKeys);
                UIManager.Instance.Popup.QteUI.UpdateTimer(ratio);

                yield return null;
            }

            // 시간 경과
            if (!keyPressed)
            {
                success = false;
                break;
            }
        }

        if (success)
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


