using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;

public class OperatorUI : MonoBehaviour
{
    [Header("Operator UI Settings")]
    [SerializeField] RectTransform operatorRect;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float animationDuration = 0.5f;
    [SerializeField] bool useUnscaledTime = true; //  실시간 / 게임 시간 (true면 일시정지나 슬로모션의 영향을 받지 않음
    Vector3 originalSize;

    [Header("Operator Text Settings")]
    [SerializeField] RectTransform dialogueRect;
    [SerializeField] TextMeshProUGUI dialogueText;

    Coroutine coOperatorImage;
    Coroutine coOperatorUI;

    void Awake()
    {
        originalSize = operatorRect.sizeDelta;
        operatorRect.sizeDelta = Vector3.zero;

        dialogueRect.gameObject.SetActive(false);
    }

    public void ShowOperatorUI(string text)
    {
        if(coOperatorUI != null)
        {
            StopCoroutine(coOperatorUI);
            coOperatorUI = null;
        }
        coOperatorUI = StartCoroutine(CoShowOperatorUI(text));
    }

    public void HideOperatorUI()
    {
        if (coOperatorUI != null)
        {
            StopCoroutine(coOperatorUI);
            coOperatorUI = null;
        }
        coOperatorUI = StartCoroutine(CoHideOperatorUI());
    }
    IEnumerator CoShowOperatorUI(string text)
    {
        ShowOperator();
        if(coOperatorImage != null )
        {
            yield return coOperatorImage;
        }
        ShowDialogueText(text);
    }

    IEnumerator CoHideOperatorUI()
    {
        HideDialogueText();
        yield return new WaitForSeconds(animationDuration);
        HideOperator();
    }
    void ShowOperator()
    {
        operatorRect.gameObject.SetActive(true);
        if(coOperatorImage != null )
        {
            StopCoroutine(coOperatorImage);
            coOperatorImage = null;
        }
        coOperatorImage = StartCoroutine(CoPopupFromCenter(operatorRect.sizeDelta.y, originalSize.y,true));
    }
    void HideOperator()
    {
        if (coOperatorImage != null)
        {
            StopCoroutine(coOperatorImage);
            coOperatorImage = null;
        }
        coOperatorImage = StartCoroutine(CoPopupFromCenter(operatorRect.sizeDelta.y, 0, false));
    }
    
    void ShowDialogueText(string text)
    {
        dialogueRect.gameObject.SetActive(true);
        dialogueText.text = text;
    }
    void HideDialogueText()
    {
        dialogueText.text = "";
        dialogueRect.gameObject.SetActive(false);
    }
    IEnumerator CoPopupFromCenter(float start, float end, bool on)
    {
        float time = 0f;
        float width = originalSize.x;
        operatorRect.sizeDelta = new Vector2(width, start);

        while (time < animationDuration)
        {
            time += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);

            float t = Mathf.Clamp01(time / animationDuration);            
            float curveValue = curve.Evaluate(t);                           

            float y = Mathf.LerpUnclamped(start, end, curveValue);         
            operatorRect.sizeDelta = new Vector2(width, y);
            yield return null;
        }

        operatorRect.sizeDelta = new Vector2(width, end);           

        if(!on)
        {
            operatorRect.gameObject.SetActive(false);
        }

        coOperatorImage = null;
    }
}
   
