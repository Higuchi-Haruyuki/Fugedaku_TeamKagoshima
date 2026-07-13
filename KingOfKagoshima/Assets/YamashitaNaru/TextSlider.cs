using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TextSlider : MonoBehaviour
{
    public RectTransform[] texts;

    public float slideDistance = 200f;
    public float duration = 0.4f;

    private int currentIndex = 0;
    private bool isAnimating = false;

    private Vector2[] originalPositions;

    //  各テキストの表示/非表示を制御するCanvasGroup
    private CanvasGroup[] canvasGroups;

    void Awake()
    {
        //  描画が始まる前(Awakeの時点)で、即座に全テキストを透明にする
        canvasGroups = new CanvasGroup[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            CanvasGroup cg = texts[i].GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = texts[i].gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroups[i] = cg;
            cg.alpha = 0f; //  ここで最初のフレームから見えなくする
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
            {
                texts[i].anchoredPosition = originalPositions[i] + new Vector2(slideDistance, 0);
            }
        }

        // 位置決めがすべて完了した「後」で、まとめて表示状態に戻す
        for (int i = 0; i < canvasGroups.Length; i++)
        {
            canvasGroups[i].alpha = 1f;
        }
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