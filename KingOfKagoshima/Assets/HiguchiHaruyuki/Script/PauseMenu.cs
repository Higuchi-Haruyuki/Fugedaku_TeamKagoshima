using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class PauseMenu : MonoBehaviour
{
    //SerializeField付きprivateメンバ変数
    [SerializeField] private PlayerController _playerContorller;
    [SerializeField] private GameObject _infoWindow;
    [SerializeField] private GameObject _giveupMenu;
    [SerializeField] private GameObject _keyConfigMenu;
    [SerializeField] private GameObject _breakGameMenu;
    [SerializeField] private List<GameObject> _choiceMenuCheckBoxList;
    [SerializeField] private List<GameObject> _giveupMenuCheckBoxList;
    [SerializeField] private List<GameObject> _breakGameMenuCheckBoxList;
    //privateメンバ変数
    private Canvas _pauseCanvas;
    private char _currentMenu = 'c';
    private List<bool> _isChoiceMenuCheckBoxList = new();
    private List<bool> _isGiveupMenuCheckBoxList = new();
    private List<bool> _isBreakGameMenuCheckBoxList = new();
    private TextMeshProUGUI _timeText, _jumpCountText, _fallCountText;
    private AudioSource _audioSource;
    //
    public Action OnGiveup;

    /// <summary>
    /// 0 ステージシーンへ
    /// 1 ゲームを閉じる
    /// </summary>
    public Action<int> OnBreakGame;
    void Start()
    {
        Application.targetFrameRate = 60;
        _pauseCanvas = GetComponent<Canvas>();
        _audioSource = GetComponent<AudioSource>();
        _giveupMenu.SetActive(false);
        _timeText = _infoWindow.transform.Find("Time").GetComponent<TextMeshProUGUI>();
        _jumpCountText = _infoWindow.transform.Find("JumpCount").GetComponent<TextMeshProUGUI>();
        _fallCountText = _infoWindow.transform.Find("FallCount").GetComponent<TextMeshProUGUI>();

        //choiceメニューに関する初期化処理
        for (int i = 0; i < _choiceMenuCheckBoxList.Count; i++)
        {
            if (i == 0)
            {
                //最初の要素だけtrue
                _isChoiceMenuCheckBoxList.Add(true);
                //最初の要素だけアクティブにする
                _choiceMenuCheckBoxList[i].SetActive(true);
                continue;
            }
            _isChoiceMenuCheckBoxList.Add(false);
            _choiceMenuCheckBoxList[i].SetActive(false);
        }
        //giveupメニューに関する初期化処理
        for (int i = 0; i < _giveupMenuCheckBoxList.Count; i++)
        {
            if (i == 0)
            {
                //最初の要素だけtrue
                _isGiveupMenuCheckBoxList.Add(true);
                //最初の要素だけアクティブにする
                _giveupMenuCheckBoxList[i].SetActive(true);
                continue;
            }
            _isGiveupMenuCheckBoxList.Add(false);
            _giveupMenuCheckBoxList[i].SetActive(false);
        }
        //breakGameメニューに関する初期化処理
        for (int i = 0; i < _breakGameMenuCheckBoxList.Count; i++)
        {
            if (i == 0)
            {
                //最初の要素だけtrue
                _isBreakGameMenuCheckBoxList.Add(true);
                //最初の要素だけアクティブにする
                _breakGameMenuCheckBoxList[i].SetActive(true);
                continue;
            }
            _isBreakGameMenuCheckBoxList.Add(false);
            _breakGameMenuCheckBoxList[i].SetActive(false);
        }
    }
    void Update()
    {
        if (!_pauseCanvas.enabled) return;
        //Wキーが押されたとき、一つ上の項目に入力が移動する処理
        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            //choiceメニューがアクティブなとき
            if (_currentMenu == 'c')
            {
                var indexTrue = FindIndexTrue(_isChoiceMenuCheckBoxList);
                //今選択されている項目が一番上にないとき
                if (indexTrue != 0)
                {
                    _isChoiceMenuCheckBoxList[indexTrue] = false;
                    _isChoiceMenuCheckBoxList[indexTrue - 1] = true;
                    _choiceMenuCheckBoxList[indexTrue].SetActive(false);
                    _choiceMenuCheckBoxList[indexTrue - 1].SetActive(true);
                }

            }
            //giveupメニューがアクティブなとき
            else if (_currentMenu == 'g')
            {
                var indexTrue = FindIndexTrue(_isGiveupMenuCheckBoxList);
                //今選択されている項目が一番上にないとき
                if (indexTrue != 0)
                {
                    _isGiveupMenuCheckBoxList[indexTrue] = false;
                    _isGiveupMenuCheckBoxList[indexTrue - 1] = true;
                    _giveupMenuCheckBoxList[indexTrue].SetActive(false);
                    _giveupMenuCheckBoxList[indexTrue - 1].SetActive(true);
                }
            }
            //breakGameメニューがアクティブなとき
            else if (_currentMenu == 'b')
            {
                var indexTrue = FindIndexTrue(_isBreakGameMenuCheckBoxList);
                //今選択されている項目が一番上にないとき
                if (indexTrue != 0)
                {
                    _isBreakGameMenuCheckBoxList[indexTrue] = false;
                    _isBreakGameMenuCheckBoxList[indexTrue - 1] = true;
                    _breakGameMenuCheckBoxList[indexTrue].SetActive(false);
                    _breakGameMenuCheckBoxList[indexTrue - 1].SetActive(true);
                }
            }

        }

        //Sキーが押されたとき、一つ下の項目に入力が移動する処理
        if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            //choiceメニューがアクティブなとき
            if (_currentMenu == 'c')
            {
                var indexTrue = FindIndexTrue(_isChoiceMenuCheckBoxList);
                //今選択されている項目が一番下にないとき
                if (indexTrue != _isChoiceMenuCheckBoxList.Count - 1)
                {
                    _isChoiceMenuCheckBoxList[indexTrue] = false;
                    _isChoiceMenuCheckBoxList[indexTrue + 1] = true;
                    _choiceMenuCheckBoxList[indexTrue].SetActive(false);
                    _choiceMenuCheckBoxList[indexTrue + 1].SetActive(true);
                }

            }
            //giveupメニューがアクティブなとき
            else if (_currentMenu == 'g')
            {
                var indexTrue = FindIndexTrue(_isGiveupMenuCheckBoxList);
                //今選択されている項目が一番下にないとき
                if (indexTrue != _isGiveupMenuCheckBoxList.Count - 1)
                {
                    _isGiveupMenuCheckBoxList[indexTrue] = false;
                    _isGiveupMenuCheckBoxList[indexTrue + 1] = true;
                    _giveupMenuCheckBoxList[indexTrue].SetActive(false);
                    _giveupMenuCheckBoxList[indexTrue + 1].SetActive(true);
                }
            }
            //breakGameメニューがアクティブなとき
            else if (_currentMenu == 'b')
            {
                var indexTrue = FindIndexTrue(_isBreakGameMenuCheckBoxList);
                //今選択されている項目が一番下にないとき
                if (indexTrue != _isBreakGameMenuCheckBoxList.Count - 1)
                {
                    _isBreakGameMenuCheckBoxList[indexTrue] = false;
                    _isBreakGameMenuCheckBoxList[indexTrue + 1] = true;
                    _breakGameMenuCheckBoxList[indexTrue].SetActive(false);
                    _breakGameMenuCheckBoxList[indexTrue + 1].SetActive(true);
                }
            }
        }

        //SPACEキーが押されたとき、今選択されている項目を決定する
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _audioSource.Play();
            //現在のメニューの状態や選択状態で呼び出す関数を変える
            if (_currentMenu == 'c')
            {
                int indexTrue = FindIndexTrue(_isChoiceMenuCheckBoxList);
                switch (indexTrue)
                {
                    case 0:
                        {
                            Continue();
                            break;
                        }
                    case 1:
                        {
                            KeyConfig();
                            break;
                        }
                    case 2:
                        {
                            OpenBreakGameMenu();
                            break;
                        }
                    case 3:
                        {
                            Giveup();
                            break;
                        }
                    default:
                        break;
                }
            }
            else if (_currentMenu == 'g')
            {
                int indexTrue = FindIndexTrue(_isGiveupMenuCheckBoxList);
                if (indexTrue == 0) No();
                else if (indexTrue == 1) Yes();
            }
            else if (_currentMenu == 'b')
            {
                int indexTrue = FindIndexTrue(_isBreakGameMenuCheckBoxList);
                if (indexTrue == 0) ReturnPauseMenu();
                else if (indexTrue == 1) BreakGame(0);
                else if (indexTrue == 2) BreakGame(1);
            }
            else if (_currentMenu == 'k')
            {
                //ポーズメニューに戻る
                _keyConfigMenu.SetActive(false);
                _currentMenu = 'c';
            }
        }
    }
    //List<bool>の中で1番最初のtrueのインデックスを返す
    int FindIndexTrue(List<bool> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i]) return i;
        }
        return -1;
    }
    public void SetInfoWindowText(Score score)
    {
        _timeText.SetText(score.ToString());
        _jumpCountText.SetText($"ジャンプ回数 : {score.JumpCount}回");
        _fallCountText.SetText($"落ちた回数 : {score.FallCount}回");
    }
    //choiceメニューでのcontinue選択肢
    void Continue()
    {
        //pauseメニューを閉じる処理
        _pauseCanvas.enabled = false;
        _giveupMenu.SetActive(false);
        Time.timeScale = 1.0f;
        _playerContorller.IsEnableInput = true;
        _currentMenu = 'c';
    }
    //choiceメニューでのKeyConfig選択肢
    void KeyConfig()
    {
        //操作設定確認する
        _keyConfigMenu.SetActive(true);
        _currentMenu = 'k';
    }
    //choiceメニューでのBreakGame選択肢
    void OpenBreakGameMenu()
    {
        //ゲームをセーブして中断する
        _breakGameMenu.SetActive(true);
        _currentMenu = 'b';
    }
    //choiceメニューでのgiveup選択肢
    void Giveup()
    {
        //giveupメニューを開く
        _giveupMenu.SetActive(true);
        _currentMenu = 'g';
    }

    //giveupメニューでのYes選択肢
    void Yes()
    {
        //giveupしてメインシーンを終了する
        OnGiveup?.Invoke();
    }

    //giveupメニューでのNo選択肢
    void No()
    {
        //giveupメニューを閉じてchoiceメニューに戻る
        _giveupMenu.SetActive(false);
        _currentMenu = 'c';
    }
    //breakGameメニューでのpauseに戻る選択肢
    void ReturnPauseMenu()
    {
        _breakGameMenu.SetActive(false);
        _currentMenu = 'c';
    }
    //breakGameメニューでのゲームを中断する選択肢
    void BreakGame(int num)
    {
        OnBreakGame?.Invoke(num);
    }

}
