using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FadeSceneLoader : MonoBehaviour
{
    public Image fadePanel;              // フェード用のUIパネル（Image）
    public float fadeDuration = 1.0f;    // フェードの完了にかかる時間
                                         // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }
    public IEnumerator FadeOutAndLoadScene()
    {
        fadePanel.enabled = true;               // パネルを有効化
        float elapsedTime = 0.0f;               // 経過時間を初期化
        Color startColor = fadePanel.color;     // フェードパネルの開始色を取得
        Color endColor = new Color(startColor.r, startColor.g , startColor.b, 1.0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;                        
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);  
            fadePanel.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadePanel.color = endColor;  
        SceneManager.LoadScene("StageSelect");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.enterKey.isPressed)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }
}
