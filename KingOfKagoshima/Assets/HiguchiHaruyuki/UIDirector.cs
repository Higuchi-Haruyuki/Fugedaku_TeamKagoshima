using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class UIDirector : MonoBehaviour
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Canvas _pauseCanvas;
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private ClearManager _clearManager;
    [SerializeField] private int _stageNumber = 1;
    [SerializeField] private Image _fadePanel;
    [SerializeField]private float _fadeDuration = 1;
    private ScoreTime _scoreTime;
    private bool _isPressdEscapeKeyBeforeFlame = false;
    private PauseMenu _pauseMenu;
    private bool _isTimerStop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCanvas.enabled = true;
        _pauseCanvas.enabled = false;
        _pauseMenu = _pauseCanvas.GetComponent<PauseMenu>();
        _scoreTime = new();
        _mainCanvas.GetComponent<MainUIDirector>()?.SetScoreTime(_scoreTime);
        //Giveupイベントの購読
        _pauseMenu.OnGiveup += OnGiveup;
        _pauseMenu.OnBreakGame += OnBreakGame;
        _clearManager.OnClear += OnClear;

    }
    private void OnDisable()
    {
        _pauseMenu.OnGiveup -= OnGiveup;
        _pauseMenu.OnBreakGame -= OnBreakGame;
        _clearManager.OnClear -= OnClear;
    }
    // Update is called once per frame
    void Update()
    {
        if(_isTimerStop) return;

        //タイマーに時間を追加
        _scoreTime.AddTime(Time.deltaTime);

        //ESCAPEキーが押されたとき、ポーズ画面を表示する処理
        if (Keyboard.current.escapeKey.isPressed)
        {
            //1つ前のフレームで入力されていなかったとき
            if (!_isPressdEscapeKeyBeforeFlame)
            {
                _isPressdEscapeKeyBeforeFlame = true;
                //Canvasの状態を入れ替える
                _pauseCanvas.enabled = !_pauseCanvas.enabled;

                //ポーズキャンバスが有効なら時間を止めておく
                if(_pauseCanvas.enabled) Time.timeScale = 0.0f;
                else Time.timeScale = 1.0f;
            }
        }
        else
        {
            //1つ前のフレームで入力されていたらフラグをfalseに戻す
            if (_isPressdEscapeKeyBeforeFlame) _isPressdEscapeKeyBeforeFlame = false;
        }
    }
    void OnGiveup()
    {
        _isTimerStop = true;
        StartCoroutine(FadeOutAndLoadScene("StageSelect"));
    }
    void OnBreakGame()
    {
        _isTimerStop = true;
        SaveData data = new()
        {
            Time = _scoreTime.GetSeconds(),
            PlayerPos = transform.position,
            FallCount = 10,//kari
            JumpCount = 10 // kari
        };
        _saveManager.SaveJson(data);
        StartCoroutine(FadeOutAndLoadScene("StageSelect"));
    }
    void OnClear()
    {
        _isTimerStop = true;
        //時間の保存
        _scoreTime.SaveTime(_stageNumber);
        //最速タイムの保存
        _scoreTime.SaveFastestTime(_stageNumber);
        //シーンのロード
        StartCoroutine(FadeOutAndLoadScene("HaruyukiResultScene"));
    }
    private void OnDestroy()
    {
        OnBreakGame();
    }
    public IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        _fadePanel.enabled = true;               // パネルを有効化
        float elapsedTime = 0.0f;               // 経過時間を初期化
        Color color = _fadePanel.color;
        while (elapsedTime < _fadeDuration)
        {
            Debug.Log("coroutine");
            elapsedTime += Time.unscaledDeltaTime;                           // 経過時間を増やす                        
            float t = Mathf.Clamp01(elapsedTime / _fadeDuration);     // フェードの進行度を計算  
            float i = Mathf.Lerp(0, 1, t);   // パネルの色を変更してフェードアウト
            color.a = i;
            _fadePanel.color = color;
            yield return null;                                       // 1フレーム待機
        }
        color.a = 1;
        _fadePanel.color = color;
        SceneManager.LoadScene(sceneName);
    }
}
