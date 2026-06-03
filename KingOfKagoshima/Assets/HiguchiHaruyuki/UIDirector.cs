using UnityEngine;
using UnityEngine.InputSystem;

public class UIDirector : MonoBehaviour
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Canvas _pauseCanvas;
    [SerializeField] private const int STAGENUMBER = 1;
    [SerializeField] private SaveManager _saveManager;
    private ScoreTime _scoreTime;
    private bool _isPressdEscapeKeyBeforeFlame = false;
    private PauseMenu _pauseMenu;
    private bool _isTimerStop = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCanvas.enabled = true;
        _pauseCanvas.enabled = false;
        _pauseMenu = _pauseCanvas.GetComponent<PauseMenu>();
        _scoreTime = new();
        _mainCanvas.GetComponent<MainUIDirector>().SetScoreTime(_scoreTime);
        _pauseMenu.GetComponent<PauseMenu>().SetScoreTime(_scoreTime);
        //Giveupイベントの購読
        _pauseMenu.OnGiveup += (() => 
        { 
            _isTimerStop = true;
            _scoreTime.SaveTime(STAGENUMBER);
        });
        _pauseMenu.OnBreakGame += (() => 
        {
            BreakGame();
        });

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
    void BreakGame()
    {
        _isTimerStop = true;
        SaveData data = new();
        data.Time = _scoreTime.GetSeconds();
        data.FallCount = 10;//kari
        data.JumpCount = 10; // kari
        _saveManager.SaveJson(data);
    }
    private void OnDestroy()
    {
        BreakGame();
    }
}
