using UnityEngine;
using UnityEngine.InputSystem;

public class UIDirector : MonoBehaviour
{
    [SerializeField] private Canvas m_mainCanvas;
    [SerializeField] private Canvas m_pauseCanvas;
    [SerializeField] private const int STAGENUMBER = 1;
    private ScoreTime m_scoreTime;
    private bool m_isPressdEscapeKeyBeforeFlame = false;
    private PauseMenu m_pauseMenu;
    private bool m_isTimerStop = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_mainCanvas.enabled = true;
        m_pauseCanvas.enabled = false;
        m_pauseMenu = m_pauseCanvas.GetComponent<PauseMenu>();
        m_scoreTime = new();
        var mainScoreTime = m_mainCanvas.GetComponent<MainUIDirector>().GetScoreTime();
        SetScoreTime(ref mainScoreTime);
        var pauseScoreTime = m_pauseMenu.GetScoreTime();
        SetScoreTime(ref pauseScoreTime);
        //Giveupイベントの購読
        m_pauseMenu.OnGiveup.AddListener(() => 
        { 
            m_isTimerStop = true;
            m_scoreTime.SaveTime(STAGENUMBER);
        });

    }
    //引数の変数に代入する
    public void SetScoreTime(ref ScoreTime scoreTime)
    {
        scoreTime = m_scoreTime;
    } 
    // Update is called once per frame
    void Update()
    {
        if(m_isTimerStop) return;

        //タイマーに時間を追加
        m_scoreTime.AddTime(Time.deltaTime);

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
