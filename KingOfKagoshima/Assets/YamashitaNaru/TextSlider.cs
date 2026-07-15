using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TextSlider : MonoBehaviour
{
    public RectTransform[] texts;
    public RectTransform[] backgrounds; // 追加：背景画像用

    public float slideDistance = 200f;
    public float duration = 0.4f;

    private int currentIndex = 0;
    private bool isAnimating = false;

    private Vector2[] originalPositions;      // テキスト用
    private Vector2[] bgOriginalPositions;     // 背景用

    private CanvasGroup[] canvasGroups;
    private CanvasGroup[] bgCanvasGroups;

    void Awake()
    {
        canvasGroups = new CanvasGroup[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            CanvasGroup cg = texts[i].GetComponent<CanvasGroup>();
            if (cg == null) cg = texts[i].gameObject.AddComponent<CanvasGroup>();
            canvasGroups[i] = cg;
            cg.alpha = 0f;
        }

        if (backgrounds != null && backgrounds.Length > 0)
        {
            bgCanvasGroups = new CanvasGroup[backgrounds.Length];
            for (int i = 0; i < backgrounds.Length; i++)
            {
                CanvasGroup cg = backgrounds[i].GetComponent<CanvasGroup>();
                if (cg == null) cg = backgrounds[i].gameObject.AddComponent<CanvasGroup>();
                bgCanvasGroups[i] = cg;
                cg.alpha = 0f;
            }
        }
    }

    void Start()
    {
        StartCoroutine(InitPositions());
    }

    IEnumerator InitPositions()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        originalPositions = new Vector2[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            originalPositions[i] = texts[i].anchoredPosition;
            if (i != currentIndex)
                texts[i].anchoredPosition = originalPositions[i] + new Vector2(slideDistance, 0);
        }

        if (backgrounds != null && backgrounds.Length > 0)
        {
            bgOriginalPositions = new Vector2[backgrounds.Length];
            for (int i = 0; i < backgrounds.Length; i++)
            {
                bgOriginalPositions[i] = backgrounds[i].anchoredPosition;
                if (i != currentIndex)
                    backgrounds[i].anchoredPosition = bgOriginalPositions[i] + new Vector2(slideDistance, 0);
            }
        }

        for (int i = 0; i < canvasGroups.Length; i++)
            canvasGroups[i].alpha = 1f;

        if (bgCanvasGroups != null)
            for (int i = 0; i < bgCanvasGroups.Length; i++)
                bgCanvasGroups[i].alpha = 1f;
    }

    void Update()
    {
        if (isAnimating) return;
        if (Keyboard.current == null) return;
        if (originalPositions == null) return;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            int nextIndex = (currentIndex + 1) % texts.Length;
            StartCoroutine(SlideTo(nextIndex, 1));
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            int nextIndex = (currentIndex - 1 + texts.Length) % texts.Length;
            StartCoroutine(SlideTo(nextIndex, -1));
        }
    }

    IEnumerator SlideTo(int nextIndex, int direction)
    {
        isAnimating = true;

        RectTransform current = texts[currentIndex];
        RectTransform next = texts[nextIndex];

        Vector2 currentStart = current.anchoredPosition;
        Vector2 currentEnd = originalPositions[currentIndex] - new Vector2(slideDistance * direction, 0);
        Vector2 nextStart = originalPositions[nextIndex] + new Vector2(slideDistance * direction, 0);
        Vector2 nextEnd = originalPositions[nextIndex];
        next.anchoredPosition = nextStart;

        // 背景も同時に用意
        RectTransform bgCurrent = null, bgNext = null;
        Vector2 bgCurrentStart = default, bgCurrentEnd = default, bgNextStart = default, bgNextEnd = default;

        bool hasBg = backgrounds != null && backgrounds.Length == texts.Length;
        if (hasBg)
        {
            bgCurrent = backgrounds[currentIndex];
            bgNext = backgrounds[nextIndex];

            bgCurrentStart = bgCurrent.anchoredPosition;
            bgCurrentEnd = bgOriginalPositions[currentIndex] - new Vector2(slideDistance * direction, 0);
            bgNextStart = bgOriginalPositions[nextIndex] + new Vector2(slideDistance * direction, 0);
            bgNextEnd = bgOriginalPositions[nextIndex];
            bgNext.anchoredPosition = bgNextStart;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / duration);

            current.anchoredPosition = Vector2.Lerp(currentStart, currentEnd, ratio);
            next.anchoredPosition = Vector2.Lerp(nextStart, nextEnd, ratio);

            if (hasBg)
            {
                bgCurrent.anchoredPosition = Vector2.Lerp(bgCurrentStart, bgCurrentEnd, ratio);
                bgNext.anchoredPosition = Vector2.Lerp(bgNextStart, bgNextEnd, ratio);
            }

            yield return null;
        }

        current.anchoredPosition = currentEnd;
        next.anchoredPosition = nextEnd;

        if (hasBg)
        {
            bgCurrent.anchoredPosition = bgCurrentEnd;
            bgNext.anchoredPosition = bgNextEnd;
        }

        currentIndex = nextIndex;
        isAnimating = false;
    }
}