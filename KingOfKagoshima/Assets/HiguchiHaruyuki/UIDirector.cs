using UnityEngine;
using UnityEngine.InputSystem;

public class UIDirector : MonoBehaviour
{
    [SerializeField] private Canvas m_mainCanvas;
    [SerializeField] private Canvas m_pauseCanvas;
    private bool m_isPressdEscapeKeyBeforeFlame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_mainCanvas.enabled = true;
        m_pauseCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //ESCAPEキーが押されたとき、ポーズ画面を表示する処理
        if (Keyboard.current.escapeKey.isPressed)
        {
            //1つ前のフレームで入力されていなかったとき
            if (!m_isPressdEscapeKeyBeforeFlame)
            {
                m_isPressdEscapeKeyBeforeFlame = true;
                //Canvasの状態を入れ替える
                m_pauseCanvas.enabled = !m_pauseCanvas.enabled;

                //ポーズキャンバスが有効なら時間を止めておく
                if(m_pauseCanvas.enabled) Time.timeScale = 0.0f;
                else Time.timeScale = 1.0f;
            }
        }
        else
        {
            //1つ前のフレームで入力されていたらフラグをfalseに戻す
            if (m_isPressdEscapeKeyBeforeFlame) m_isPressdEscapeKeyBeforeFlame = false;
        }
    }
}
