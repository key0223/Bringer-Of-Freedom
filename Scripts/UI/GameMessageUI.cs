using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameMessageUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] CanvasGroup group;
    [SerializeField] GameObject messageGO;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] float fadeInTime = 0.5f;
    [SerializeField] float fadeOutTime = 0.5f;

    Coroutine coFade;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        messageGO.SetActive(false);
    }
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    ShowMessageFor("아-아-, 마이크 테스트",3f);
        //}    
    }
    public void ShowGameMessage(string message)
    {
        StopRunning();
        messageText.text = message;
        StartCoroutine(CoFade(true, fadeInTime));
    }
    public void HideGameMessage()
    {
        StopRunning();
        messageText.text = "";
        StartCoroutine(CoFade(false, fadeOutTime));
    }

    IEnumerator CoFade(bool fadeIn, float duration)
    {
        if (!group) yield break;

        if (fadeIn)
            messageGO.SetActive(true);

        float start = group.alpha;
        float target = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        if (Mathf.Approximately(duration, 0f))
        {
            group.alpha = target;
        }
        else
        {
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime; 
                float k = Mathf.Clamp01(elapsed / duration);
                group.alpha = Mathf.Lerp(start, target, k);
                yield return null;
            }
            group.alpha = target;
        }

        if(!fadeIn)
            messageGO.gameObject.SetActive(false);
    }

    // 일정 시간 표시 후 자동 페이드 아웃
    public void ShowMessageFor(string message, float duration)
    {
        StopRunning();
        messageText.text = message;
        coFade = StartCoroutine(ShowForRoutine(duration));
    }
    public void ShowMessageFor(string message)
    {
        StopRunning();
        messageText.text = message;
        coFade = StartCoroutine(ShowForRoutine(GameMessages.MessageDuration));
    }

    IEnumerator ShowForRoutine(float duration)
    {
        // 페이드 인
        yield return CoFade(true, fadeInTime);
        // 유지
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        // 페이드 아웃
        yield return CoFade(false, fadeOutTime);
    }

    void StopRunning()
    {
        if (coFade != null)
        {
            StopCoroutine(coFade);
            coFade = null;
        }
    }
}
