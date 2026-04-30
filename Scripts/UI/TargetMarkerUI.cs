using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarkerUI : MonoBehaviour
{
    [Header("Cameras/Canvas")]
    [SerializeField] Camera worldCamera;
    [SerializeField] Canvas onScreenCanvas;
    [SerializeField] Canvas offScreenCanvas;   

    [Header("Prefabs")]
    [SerializeField] RectTransform onScreenMarkerPrefab;
    [SerializeField] RectTransform offScreenArrowPrefab;

    [Header("Settings")]
    [SerializeField] Vector3 worldOffset = new Vector3(0, 1.8f, 0); // 월드 기준 위치
    [SerializeField] float edgePadding = 32f;
    [SerializeField] int farSamplingInterval = 3;// 업데이트 주기

    readonly Dictionary<Transform, MarkerPair> map = new();
    readonly Queue<RectTransform> onPool = new();
    readonly Queue<RectTransform> offPool = new();

    RectTransform onCanvasRect;
    RectTransform offCanvasRect;
    int frameIndex = 0;

    void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
        onCanvasRect = onScreenCanvas.GetComponent<RectTransform>();
        offCanvasRect = offScreenCanvas.GetComponent<RectTransform>();
    }

    public void AddTarget(Transform target)
    {
        if (map.ContainsKey(target)) return;

        var on = GetFromPool(onPool, onScreenMarkerPrefab, onCanvasRect);
        var off = GetFromPool(offPool, offScreenArrowPrefab, offCanvasRect);

        on.gameObject.SetActive(true);
        off.gameObject.SetActive(false);

        map[target] = new MarkerPair { on = on, off = off, sampleOffset = map.Count % farSamplingInterval };
    }

    public void RemoveTarget(Transform target)
    {
        if (!map.TryGetValue(target, out var pair)) return;
        ReturnToPool(pair.on, onPool);
        ReturnToPool(pair.off, offPool);
        map.Remove(target);
    }

    void LateUpdate()
    {
        frameIndex++;

        foreach (var kv in map)
        {
            var target = kv.Key;
            var p = kv.Value;
            if (target == null) { RemoveTarget(target); continue; }

            bool skip = ((frameIndex + p.sampleOffset) % farSamplingInterval) != 0;

            Vector3 wp = target.position + worldOffset;

            Vector3 sp = worldCamera.WorldToScreenPoint(wp); // 픽셀 좌표 
            Vector3 vp = worldCamera.WorldToViewportPoint(wp); // 뷰 포트 좌표로 변환
            bool behind = sp.z < 0f;
            bool offscreen = behind || vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f;

            if (!offscreen)
            {
                // 온스크린 표시
                if (!skip)
                {
                    if (onScreenCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        p.on.position = sp; // Overlay는 픽셀 좌표 직접 대입 
                    }
                    else
                    {
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            onCanvasRect, sp, null, out Vector2 local)) 
                        {
                            p.on.anchoredPosition = local;
                        }
                    }
                }
                if (!p.on.gameObject.activeSelf) p.on.gameObject.SetActive(true);
                if (p.off.gameObject.activeSelf) p.off.gameObject.SetActive(false);
            }
            else
            {
                // 오프스크린 표시
                Vector2 clamped = ClampToScreen(sp, behind);
                if (!skip)
                {
                    p.off.position = clamped; // Overlay 기준 배치
                    // 화살표 방향 회전
                    Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
                    Vector2 dir = clamped - center;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    //p.off.rotation = Quaternion.Euler(0, 0, angle);
                    float rotation = angle - 90f;
                    p.off.rotation = Quaternion.Euler(0, 0, rotation);
                }
                if (p.on.gameObject.activeSelf) p.on.gameObject.SetActive(false);
                if (!p.off.gameObject.activeSelf) p.off.gameObject.SetActive(true);
            }
        }
    }

    Vector2 ClampToScreen(Vector3 sp, bool behind)
    {
        float x = Mathf.Clamp(sp.x, edgePadding, Screen.width - edgePadding);
        float y = Mathf.Clamp(sp.y, edgePadding, Screen.height - edgePadding);
        if (behind) { x = Screen.width - x; y = Screen.height - y; }
        return new Vector2(x, y);
    }

    RectTransform GetFromPool(Queue<RectTransform> pool, RectTransform prefab, RectTransform parent)
    {
        if (pool.Count > 0)
        {
            var r = pool.Dequeue();
            r.SetParent(parent, false);
            return r;
        }
        return Instantiate(prefab, parent);
    }

    void ReturnToPool(RectTransform rect, Queue<RectTransform> pool)
    {
        rect.gameObject.SetActive(false);
        rect.SetParent(transform, false);
        pool.Enqueue(rect);
    }

    class MarkerPair
    {
        public RectTransform on;
        public RectTransform off;
        public int sampleOffset; // 프레임 분산용 
    }
}
