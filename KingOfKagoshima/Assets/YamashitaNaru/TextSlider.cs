using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TextSlider : MonoBehaviour
{
    public RectTransform[] texts;
    public float slideDistance = 1080f;
    public float duration = 0.4f;

    private int currentIndex = 0;
    private bool isAnimating = false;

    // 各テキスト本来の「表示位置(中央にいるときの位置)」を保存しておく
    private Vector2[] originalPositions;

    void Start()
    {
        originalPositions = new Vector2[texts.Length];

        for (int i = 0; i < texts.Length; i++)
        {
            // 今実際に置かれている位置をそのまま「本来の表示位置」として記録
            originalPositions[i] = texts[i].anchoredPosition;

            // currentIndex以外は画面外へ退避
            if (i != currentIndex)
            {
                texts[i].anchoredPosition = originalPositions[i] + new Vector2(slideDistance, 0);
            }
        }
    }

    void Update()
    {
        if (isAnimating) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame && currentIndex + 1 < texts.Length)
        {
            StartCoroutine(SlideTo(currentIndex + 1, 1));
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame && currentIndex - 1 >= 0)
        {
            StartCoroutine(SlideTo(currentIndex - 1, -1));
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

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / duration);
            current.anchoredPosition = Vector2.Lerp(currentStart, currentEnd, ratio);
            next.anchoredPosition = Vector2.Lerp(nextStart, nextEnd, ratio);
            yield return null;
        }

        current.anchoredPosition = currentEnd;
        next.anchoredPosition = nextEnd;

        currentIndex = nextIndex;
        isAnimating = false;
    }
}