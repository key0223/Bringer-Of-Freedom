using System.Collections;
using UnityEngine;

public class QTEMouseClickTarget : QTEBase
{
    //  0 : ¿ÞÂÊ,
    //  1 : ¿À¸¥ÂÊ,
    //  2 : ÈÙ
    public int mouseButtonToPress;
    public Transform target;
    public float allowedRadius = 50f;
    public override IEnumerator CoExecute()
    {
        float timer = 0;
        bool pressed = false;

        UIManager.Instance.Popup.ShowQTE();
        UIManager.Instance.Popup.QteUI.SetQTE(target,allowedRadius);

        while (timer < duration && !pressed)
        {
            float ratio = timer / duration;
            UIManager.Instance.Popup.QteUI.MouseClickTimer(ratio);
            if (Input.GetMouseButtonDown(mouseButtonToPress) && IsCursorOnTarget(target))
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

    bool IsCursorOnTarget(Transform target)
    {
        if (target == null) return false;

        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = Camera.main.WorldToScreenPoint(target.position);

        return Vector2.Distance(mousePos, targetPos) <= allowedRadius;
    }
}
