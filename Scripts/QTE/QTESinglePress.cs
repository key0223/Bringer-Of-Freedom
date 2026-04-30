using System.Collections;
using UnityEngine;

public class QTESinglePress : QTEBase
{
    public KeyCode keyToPress;
    public override IEnumerator CoExecute()
    {
        float timer = 0;
        bool pressed = false;

        UIManager.Instance.Popup.ShowQTE();
        UIManager.Instance.Popup.QteUI.SetQTE();

        while (timer < duration && !pressed)
        {
            float ratio = timer / duration;
            UIManager.Instance.Popup.QteUI.UpdateTimer(ratio);
            if (Input.GetKeyDown(keyToPress))
            {
                Debug.Log("Success");
                pressed = true;
                UIManager.Instance.Popup.QteUI.OnKeyPressedEffect();
                InvokeSuccess();
                break;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!pressed)
        {
            Debug.Log("Fail");
            InvokeFail();
        }
        UIManager.Instance.Popup.HideQTE();
    }
}
