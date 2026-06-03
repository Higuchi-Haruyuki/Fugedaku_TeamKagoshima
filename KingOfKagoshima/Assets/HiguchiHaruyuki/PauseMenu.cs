using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Canvas))]
public class PauseMenu : MonoBehaviour
{
    //SerializeField付きprivateメンバ変数
    [SerializeField] private GameObject m_giveupMenu;
    [SerializeField] private List<GameObject> m_choiceMenuCheckBoxList;
    [SerializeField] private List<GameObject> m_giveupMenuCheckBoxList;

    //privateメンバ変数
    private Canvas m_pauseCanvas;
    private bool m_isPressdWKeyBeforeFlame = false;
    private bool m_isPressdSKeyBeforeFlame = false;
    private bool m_isPressdSpaceKeyBeforeFlame = false;
    private char m_currentMenu = 'c';
    private List<bool> m_isChoiceMenuCheckBoxList = new();
    private List<bool> m_isGiveupMenuCheckBoxList = new();
    private ScoreTime m_scoreTime;
    //
    public Action OnGiveup;
    public Action OnBreakGame;
    void Start()
    {
        Application.targetFrameRate = 60;
        m_pauseCanvas = GetComponent<Canvas>();
        m_giveupMenu.SetActive(false);

        //choiceメニューに関する初期化処理
        for (int i = 0; i < m_choiceMenuCheckBoxList.Count; i++)
        {
            if (i == 0)
            {
                //最初の要素だけtrue
                m_isChoiceMenuCheckBoxList.Add(true);
                //最初の要素だけアクティブにする
                m_choiceMenuCheckBoxList[i].SetActive(true);
                continue;
            }
            m_isChoiceMenuCheckBoxList.Add(false);
            m_choiceMenuCheckBoxList[i].SetActive(false);
        }
        //giveupメニューに関する初期化処理
        for (int i = 0; i < m_giveupMenuCheckBoxList.Count; i++)
        {
            if (i == 0)
            {
                //最初の要素だけtrue
                m_isGiveupMenuCheckBoxList.Add(true);
                //最初の要素だけアクティブにする
                m_giveupMenuCheckBoxList[i].SetActive(true);
                continue;
            }
            m_isGiveupMenuCheckBoxList.Add(false);
            m_giveupMenuCheckBoxList[i].SetActive(false);

        }
    }
    void Update()
    {
        if (!m_pauseCanvas.enabled) return;
        //Wキーが押されたとき、一つ上の項目に入力が移動する処理
        if (Keyboard.current.wKey.isPressed)
        {
            //1つ前のフレームで入力されていなかったとき
            if (!m_isPressdWKeyBeforeFlame)
            {
                m_isPressdWKeyBeforeFlame = true;
                //choiceメニューがアクティブなとき
                if (m_currentMenu == 'c')
                {
                    var indexTrue = FindIndexTrue(m_isChoiceMenuCheckBoxList);
                    //今選択されている項目が一番上にないとき
                    if (indexTrue != 0)
                    {
                        m_isChoiceMenuCheckBoxList[indexTrue] = false;
                        m_isChoiceMenuCheckBoxList[indexTrue - 1] = true;
                        m_choiceMenuCheckBoxList[indexTrue].SetActive(false);
                        m_choiceMenuCheckBoxList[indexTrue - 1].SetActive(true);
                    }

                }
                //giveupメニューがアクティブなとき
                else if (m_currentMenu == 'g')
                {
                    var indexTrue = FindIndexTrue(m_isGiveupMenuCheckBoxList);
                    //今選択されている項目が一番上にないとき
                    if (indexTrue != 0)
                    {
                        m_isGiveupMenuCheckBoxList[indexTrue] = false;
                        m_isGiveupMenuCheckBoxList[indexTrue - 1] = true;
                        m_giveupMenuCheckBoxList[indexTrue].SetActive(false);
                        m_giveupMenuCheckBoxList[indexTrue - 1].SetActive(true);
                    }
                }
            }
        }
        else
        {
            //1つ前のフレームで入力されていたらフラグをfalseに戻す
            if (m_isPressdWKeyBeforeFlame) m_isPressdWKeyBeforeFlame = false;
        }

        //Sキーが押されたとき、一つ下の項目に入力が移動する処理
        if (Keyboard.current.sKey.isPressed)
        {
            //1つ前のフレームで入力されていなかったとき
            if (!m_isPressdSKeyBeforeFlame)
            {
                m_isPressdSKeyBeforeFlame = true;
                //choiceメニューがアクティブなとき
                if (m_currentMenu == 'c')
                {
                    var indexTrue = FindIndexTrue(m_isChoiceMenuCheckBoxList);
                    //今選択されている項目が一番下にないとき
                    if (indexTrue != m_isChoiceMenuCheckBoxList.Count - 1)
                    {
                        m_isChoiceMenuCheckBoxList[indexTrue] = false;
                        m_isChoiceMenuCheckBoxList[indexTrue + 1] = true;
                        m_choiceMenuCheckBoxList[indexTrue].SetActive(false);
                        m_choiceMenuCheckBoxList[indexTrue + 1].SetActive(true);
                    }

                }
                //giveupメニューがアクティブなとき
                else if (m_currentMenu == 'g')
                {
                    var indexTrue = FindIndexTrue(m_isGiveupMenuCheckBoxList);
                    //今選択されている項目が一番下にないとき
                    if (indexTrue != m_isGiveupMenuCheckBoxList.Count - 1)
                    {
                        m_isGiveupMenuCheckBoxList[indexTrue] = false;
                        m_isGiveupMenuCheckBoxList[indexTrue + 1] = true;
                        m_giveupMenuCheckBoxList[indexTrue].SetActive(false);
                        m_giveupMenuCheckBoxList[indexTrue + 1].SetActive(true);
                    }
                }
            }
        }
        else
        {
            //1つ前のフレームで入力されていたらフラグをfalseに戻す
            if (m_isPressdSKeyBeforeFlame) m_isPressdSKeyBeforeFlame = false;
        }

        //SPACEキーが押されたとき、今選択されている項目を決定する
        if (Keyboard.current.spaceKey.isPressed)
        {
            //1つ前のフレームで入力されていなかったとき
            if (!m_isPressdSpaceKeyBeforeFlame)
            {
                m_isPressdSpaceKeyBeforeFlame = true;

                //現在のメニューの状態や選択状態で呼び出す関数を変える
                if (m_currentMenu == 'c')
                {
                    int indexTrue = FindIndexTrue(m_isChoiceMenuCheckBoxList);
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
                                BreakGame();
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
                else if (m_currentMenu == 'g')
                {
                    int indexTrue = FindIndexTrue(m_isGiveupMenuCheckBoxList);
                    if (indexTrue == 0) No();
                    else if (indexTrue == 1) Yes();
                }
            }
        }
        else
        {
            //1つ前のフレームで入力されていたらフラグをfalseに戻す
            if (m_isPressdSpaceKeyBeforeFlame) m_isPressdSpaceKeyBeforeFlame = false;
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
    public void SetScoreTime(ScoreTime scoreTime)
    {
        m_scoreTime = scoreTime;
    }

    //choiceメニューでのcontinue選択肢
    void Continue()
    {
        //pauseメニューを閉じる処理
        m_pauseCanvas.enabled = false;
        m_giveupMenu.SetActive(false);
        Time.timeScale = 1.0f;
        m_currentMenu = 'c';
    }
    //choiceメニューでのKeyConfig選択肢
    void KeyConfig()
    {
        //操作設定確認する
        Debug.Log("KeyConfig");
    }
    //choiceメニューでのBreakGame選択肢
    void BreakGame()
    {
        //ゲームをセーブして中断する
        Debug.Log("BreakGame");
    }
    //choiceメニューでのgiveup選択肢
    void Giveup()
    {
        //giveupメニューを開く
        m_giveupMenu.SetActive(true);
        m_currentMenu = 'g';
    }

    //giveupメニューでのYes選択肢
    void Yes()
    {
        //giveupしてメインシーンを終了する
        Debug.Log("GIVEUP!!!");
        OnGiveup?.Invoke();
        SceneManager.LoadScene("StageSelect");
    }

    //giveupメニューでのNo選択肢
    void No()
    {
        //giveupメニューを閉じてchoiceメニューに戻る
        m_giveupMenu.SetActive(false);
        m_currentMenu = 'c';
    }

}
