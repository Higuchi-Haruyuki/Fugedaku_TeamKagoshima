using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FadeSceneLoader : MonoBehaviour
{
    public Image fadePanel;              // フェード用のUIパネル（Image）
    public float fadeDuration = 5.0f;    // フェードの完了にかかる時間
    public bool isPressdEnterKey = false;                                    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }
    public IEnumerator FadeOutAndLoadScene()
    {
        fadePanel.enabled = true;               // パネルを有効化
        float elapsedTime = 0.0f;               // 経過時間を初期化
        Color color = fadePanel.color;
        while (elapsedTime < fadeDuration)
        {
           
            elapsedTime += Time.deltaTime;                           // 経過時間を増やす                        
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);     // フェードの進行度を計算  
            float i = Mathf.Lerp(0, 1, t);   // パネルの色を変更してフェードアウト
            color.a = i;
            fadePanel.color = color;
            yield return null;                                       // 1フレーム待機
        }
        color.a = 1;
        fadePanel.color = color;
        SceneManager.LoadScene("StageSelect");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.enterKey.isPressed && !isPressdEnterKey)
        {
            StartCoroutine(FadeOutAndLoadScene());
            isPressdEnterKey = true;
        }
    }
}
