using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QTEUI : MonoBehaviour
{
    [SerializeField] Image timerImage; 
    [SerializeField] TextMeshProUGUI keyText;

    Vector3 originalScale;

    Transform qteTarget;
    float allowedRadius = 0;

    void Start()
    {
        originalScale = transform.localScale;
    }
    public void SetQTE()
    {
        timerImage.fillAmount = 0f;
        keyText.text = "";
    }
    public void SetQTE(Transform target, float radius)
    {
        timerImage.fillAmount = 0f;
        timerImage.color = Color.white;

        qteTarget = target;
        allowedRadius = radius;
    }
    public void SetMultikey(KeyCode[] keys, int currentIndex)
    {
        string displayStr = "";

        for (int i = 0; i < keys.Length; i++)
        {
            if (i == currentIndex)
                displayStr += $"<color=yellow>{keys[i]}</color>";
            else
                displayStr += keys[i].ToString();

            if (i < keys.Length - 1)
                displayStr += " → ";
        }

        keyText.text = displayStr;
    }

    public void UpdateTimer(float ratio)
    {
        timerImage.fillAmount = Mathf.Clamp01(ratio);
    }
    
    public void MouseClickTimer(float ratio)
    {
        timerImage.fillAmount = Mathf.Clamp01(ratio);

        if(qteTarget != null)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 targetPos = Camera.main.WorldToScreenPoint(qteTarget.position);

            float dist = Vector2.Distance(mousePos, targetPos);

            timerImage.color = dist <= allowedRadius ? Color.green : Color.white;
        }
    }

    // UI 연출

    public void OnKeyPressedEffect()
    {
        StartCoroutine(CoPunchScale());
        StartCoroutine(FlashColor());
    }

    IEnumerator CoPunchScale()
    {
        float punchSize = 1.3f;
        float maxSize = 1.5f;

        float targetScale = Mathf.Min(punchSize, maxSize);
        Vector3 punchScale = originalScale * targetScale;

        float elapsed = 0f;
        float duration = 0.08f;
        
        while(elapsed < duration)
        {
            float time = Mathf.Clamp01(elapsed/ duration);
            Vector3 size = Vector3.Lerp(originalScale, punchScale, time);
            size.x = Mathf.Clamp(size.x, -maxSize * Mathf.Abs(originalScale.x), maxSize * Mathf.Abs(originalScale.x)); 
            size.y = Mathf.Clamp(size.y, -maxSize * Mathf.Abs(originalScale.y), maxSize * Mathf.Abs(originalScale.y)); 
            size.z = Mathf.Clamp(size.z, -maxSize * Mathf.Abs(originalScale.z), maxSize * Mathf.Abs(originalScale.z)); 
            transform.localScale = size;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = punchScale;
        elapsed = 0f;

        while(elapsed < duration)
        {
            float time = Mathf.Clamp01(elapsed / duration);            
            Vector3 size = Vector3.Lerp(originalScale * maxSize, originalScale, time);
            size.x = Mathf.Clamp(size.x, -maxSize * Mathf.Abs(originalScale.x), maxSize * Mathf.Abs(originalScale.x)); 
            size.y = Mathf.Clamp(size.y, -maxSize * Mathf.Abs(originalScale.y), maxSize * Mathf.Abs(originalScale.y)); 
            size.z = Mathf.Clamp(size.z, -maxSize * Mathf.Abs(originalScale.z), maxSize * Mathf.Abs(originalScale.z)); 
            transform.localScale = size;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    IEnumerator FlashColor()
    {
        Color original = timerImage.color;
        timerImage.color = Color.yellow;
        yield return new WaitForSecondsRealtime(0.05f);
        timerImage.color = original;
    }
}
