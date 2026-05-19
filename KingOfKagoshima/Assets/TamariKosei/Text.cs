using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public class Text : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float fadeDuration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Fade(0, 1));
    }
    public IEnumerator Fade(int start, int end)
    {
        text.enabled = true;               // パネルを有効化
        float elapsedTime = 0.0f;               // 経過時間を初期化
        Color color = text.color;
        while (elapsedTime < fadeDuration)
        {

            elapsedTime += Time.deltaTime;                           // 経過時間を増やす                        
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);     // フェードの進行度を計算  
            float i = Mathf.Lerp(start, end, t);   // パネルの色を変更してフェードアウト
            color.a = i;
            text.color = color;
            yield return null;                                       // 1フレーム待機
        }
        StartCoroutine(Fade(end, start));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
