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
    [SerializeField] private Haruyuki_PlayerController _playerContorller;
    [SerializeField] private int _stageNumber = 1;
    [SerializeField] private Image _fadePanel;
    [SerializeField] private float _fadeDuration = 1;
    private Score _score;
    private bool _isPressdEscapeKeyBeforeFlame = false;
    private PauseMenu _pauseMenu;
    private bool _isTimerStop;
    private int _fastestDataIndex;
    private SaveData _loadFastestData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCanvas.enabled = true;
        _pauseCanvas.enabled = false;
        _pauseMenu = _pauseCanvas.GetComponent<PauseMenu>();
        _score = new();
        _mainCanvas.GetComponent<MainUIDirector>()?.SetScoreTime(_score);
        //Giveupイベントの購読
        _pauseMenu.OnGiveup += OnGiveup;
        _pauseMenu.OnBreakGame += OnBreakGame;
        _clearManager.OnClear += OnClear;
        _playerContorller.OnJump += OnJump;
        _playerContorller.OnFall += OnFall;
        //最速タイムをロードする
        _fastestDataIndex = _stageNumber - 1 + 2;
        _loadFastestData = _saveManager.LoadJson(_fastestDataIndex);

    }
    private void OnDisable()
    {
        _pauseMenu.OnGiveup -= OnGiveup;
        _pauseMenu.OnBreakGame -= OnBreakGame;
        _clearManager.OnClear -= OnClear;
        _playerContorller.OnJump -= OnJump;
        _playerContorller.OnFall -= OnFall;
        Time.timeScale = 1.0f;
    }
    // Update is called once per frame
    void Update()
    {
        if(_isTimerStop) return;

        //タイマーに時間を追加
        _score.AddTime(Time.deltaTime);

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
            StageNum = _stageNumber,
            Time = _score.GetSeconds(),
            PlayerPos = transform.position,
            FallCount = _score.FallCount,
            JumpCount = _score.JumpCount,
        };
        _saveManager.SaveJson(data,_stageNumber - 1);
        StartCoroutine(FadeOutAndLoadScene("StageSelect"));
    }
    void OnClear()
    {
        _isTimerStop = true;
        //時間の保存
        _score.SaveAll();
        //Debug.Log($"TIME: {_score.ToString()},JUMP: {_score.JumpCount}, FALL: {_score.FallCount}");
        //最速タイムの保存
        //今回のほうが早いとき
        //最速タイムに値が入っていないとき(最初のクリア)も考慮しておく
        float currentFastestTime = _loadFastestData.Time;
        if(_score.GetSeconds() < currentFastestTime && currentFastestTime <= 0.0f)
        {
            SaveData data = new() {
                StageNum = _stageNumber,
                Time = _score.GetSeconds(),
                JumpCount = _score.JumpCount,
                FallCount = _score.FallCount,
            };
            currentFastestTime = _score.GetSeconds();
            _saveManager.SaveJson(data, _fastestDataIndex);
        }
        
        Score fastestTime = new();
        fastestTime.AddTime(currentFastestTime);
        fastestTime.SaveFastestTime();
        //シーンのロード
        StartCoroutine(FadeOutAndLoadScene("HaruyukiResultScene"));
    }
    void OnJump() 
    {
        _score.JumpCount++;
        Debug.Log($"ジャンプ回数{_score.JumpCount}");
    }
    void OnFall() 
    { 
        _score.FallCount++;
        Debug.Log($"落下回数{_score.FallCount}");
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
