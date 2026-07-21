using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class UIDirector : MonoBehaviour
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Canvas _pauseCanvas;
    [SerializeField] private ClearManager _clearManager;
    [SerializeField] private PlayerController _playerContorller;
    [SerializeField] private int _stageNumber = 1;
    [SerializeField] private Image _fadePanel;
    [SerializeField] private float _fadeDuration = 1;
    private Score _data;
    private PauseMenu _pauseMenu;
    private bool _isTimerStop;
    private SaveData _loadFastestData;
    private AudioSource _pauseAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCanvas.enabled = true;
        _pauseCanvas.enabled = false;
        _pauseMenu = _pauseCanvas.GetComponent<PauseMenu>();
        _pauseAudioSource = _pauseCanvas.GetComponent<AudioSource>();
        if (_data == null )
        {
            _data = new Score();
        }
        _mainCanvas.GetComponent<MainUIDirector>()?.SetScoreTime(_data);
        //Giveupイベントの購読
        _pauseMenu.OnGiveup += OnGiveup;
        _pauseMenu.OnBreakGame += OnBreakGame;
        if(_clearManager) _clearManager.OnClear += OnClear;
        _playerContorller.OnJump += OnJump;
        _playerContorller.OnFall += OnFall;
        //最速タイムをロードする
        if(_stageNumber == 1)
            _loadFastestData = SaveManager.LoadJson(SaveManager.GetPath(SaveFile.Stage1FastestTimeData));
        else if(_stageNumber == 2)
            _loadFastestData = SaveManager.LoadJson(SaveManager.GetPath(SaveFile.Stage2FastestTimeData));

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
        _data.AddTime(Time.deltaTime);

        _pauseMenu.SetInfoWindowText(_data);
        //ESCAPEキーが押されたとき、ポーズ画面を表示する処理
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            //Canvasの状態を入れ替える
            _pauseCanvas.enabled = !_pauseCanvas.enabled;
            _pauseAudioSource.Play();
            //ポーズキャンバスが有効なら時間を止めておく
            if (_pauseCanvas.enabled) 
            {
                _playerContorller.IsEnableInput = false;
                Time.timeScale = 0.0f; 
            }

            else 
            {
                _playerContorller.IsEnableInput = true;
                Time.timeScale = 1.0f; 
            }
        }
    }
    public void SetData(SaveData data)
    {
        _data = new();
        _data.AddTime(data.Time);
        _data.JumpCount = data.JumpCount;
        _data.FallCount = data.FallCount;
    }
    public void SetStageNum(int num)
    {
        _stageNumber = num;
    }
    void OnGiveup()
    {
        _isTimerStop = true;

        StartCoroutine(FadeOutAndLoadScene("StageSelect"));
    }
    void OnBreakGame(int num)
    {
        _isTimerStop = true;
        SaveData data = new()
        {
            StageNum = _stageNumber,
            Time = _data.GetSeconds(),
            PlayerPos = _playerContorller.transform.position,
            FallCount = _data.FallCount,
            JumpCount = _data.JumpCount,
        };
        if (_stageNumber == 1)
            SaveManager.SaveJson(data, SaveManager.GetPath(SaveFile.Stage1SaveData));
        else if (_stageNumber == 2)
            SaveManager.SaveJson(data, SaveManager.GetPath(SaveFile.Stage2SaveData));
        if(num == 0)
        {
            StartCoroutine(FadeOutAndLoadScene("StageSelect"));
        }
        else if(num == 1)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
        Time.timeScale = 1f;
    }
    void OnClear()
    {
        _isTimerStop = true;
        //時間の保存
        _data.SaveAll();
        //Debug.Log($"TIME: {_score.ToString()},JUMP: {_score.JumpCount}, FALL: {_score.FallCount}");
        //最速タイムの保存
        //今回のほうが早いとき
        //最速タイムに値が入っていないとき(最初のクリア)も考慮しておく
        float currentFastestTime = _loadFastestData.Time;
        if((_data.GetSeconds() < currentFastestTime) || (currentFastestTime <= 0.0f))
        {
            SaveData data = new() {
                StageNum = _stageNumber,
                Time = _data.GetSeconds(),
                JumpCount = _data.JumpCount,
                FallCount = _data.FallCount,
            };
            Debug.Log(data.ToString());
            currentFastestTime = _data.GetSeconds();
            if (_stageNumber == 1)
                SaveManager.SaveJson(data,SaveManager.GetPath(SaveFile.Stage1FastestTimeData));
            else if (_stageNumber == 2)
               SaveManager.SaveJson(data,SaveManager.GetPath(SaveFile.Stage2FastestTimeData));
        }
        
        Score fastestTime = new();
        fastestTime.AddTime(currentFastestTime);
        fastestTime.SaveFastestTime();
        Time.timeScale = 1f;
        //シーンのロード
        StartCoroutine(FadeOutAndLoadScene("resultScene"));
    }
    void OnJump()
    {
        if (!_pauseCanvas.enabled)
        {
            _data.JumpCount++;
            //Debug.Log($"ジャンプ回数{_data.JumpCount}");
        }
    }
    void OnFall() 
    {
        if (!_pauseCanvas.enabled)
        { 
            _data.FallCount++;
           // Debug.Log($"落下回数{_data.FallCount}");
        }
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
