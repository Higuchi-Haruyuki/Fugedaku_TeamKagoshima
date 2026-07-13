using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
///ステージ選択の全体を操作する 
///カーソルで選んでステージを選択、実行
///文字テキストを左右にスライドさせる
/// </summary>

///<summary>
///上下キーでカーソルを移動させる管理クラス
///</summary>
public class StageSelectManeger : MonoBehaviour
{
    [SerializeField] StageSelectCursor[,] _cursors;//Inspectorでステージの数だけ登録SAVEも含める

    [SerializeField] RectTransform[] _stageTextRects; // StageText, StageText2のRectTransformをInspectorで登録
    [SerializeField] float _slideDuration = 0.25f;
    [SerializeField] float _slideDistance = 300f;

    Vector2[] _stageTextHomePositions; // 各テキストの本来の位置を保存
    Coroutine _slideCoroutine;



    int _cursorIndex = 0;//初めからと途中で選択中
    int _stageIndex = 0;//左右選択中
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cursors = new StageSelectCursor[3, 3];
        // Findで目当てのオブジェクトを取得して保存しとく
        _cursors[0, 0] = GameObject.Find("Stage1Text").GetComponent<StageSelectCursor>();
        _cursors[0, 1] = GameObject.Find("StageText").GetComponent<StageSelectCursor>();
        _cursors[0, 2] = GameObject.Find("Save1").GetComponent<StageSelectCursor>();

        _cursors[1, 0] = GameObject.Find("Stage2Text").GetComponent<StageSelectCursor>();
        _cursors[1, 1] = GameObject.Find("StageText2").GetComponent<StageSelectCursor>();
        _cursors[2, 2] = GameObject.Find("Save2").GetComponent<StageSelectCursor>();



        //最初は0番目を選択状態にしておく
        _cursors[0, _cursorIndex].Selected();

        _stageTextHomePositions = new Vector2[_stageTextRects.Length];
        for (int i = 0; i < _stageTextRects.Length; i++)
        {
            _stageTextHomePositions[i] = _stageTextRects[i].anchoredPosition;
        }

        // 最初はStageIndex=0のテキストだけ中央に、それ以外は画面外に置いておく
        for (int i = 0; i < _stageTextRects.Length; i++)
        {
            if (i != _stageIndex)
            {
                _stageTextRects[i].anchoredPosition =
                    _stageTextHomePositions[i] + new Vector2(_slideDistance, 0);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

        //下キー入力で次のステージの選択
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            MoveCursorIndex(1);
        }

        //上キーで決定
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            MoveCursorIndex(-1);
        }
        //左キー入力でステージの選択
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            //ステージを切り替える
            MoveStageIndex(-1);
        }
        //右キーでステージ選択
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            //ステージを切り替える
            MoveStageIndex(1);
        }

        //スペースキーで決定
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _cursors[_stageIndex, _cursorIndex].Pressed();
        }
    }

    /// <summary>
    /// 選択インデックスを動かす
    /// </summary>
    /// <param name="direction"></param>
    void MoveCursorIndex(int direction)
    {
        //今選ばれているカーソルの色をもとに戻す
        _cursors[_stageIndex, _cursorIndex].Deselected();

        int oldStageIndex = _stageIndex;



        //インデックスを移動させる
        _cursorIndex += direction;
        if (_cursorIndex < 0)
        {
            _cursorIndex = _cursors.GetLength(1) - 1;//先頭から前に行ったら末尾に移動
        }
        else if (_cursorIndex >= _cursors.GetLength(1))
        {
            _cursorIndex = 0;//末尾から次に行ったら先頭へ
        }
        //新しく選ばれたカーソルの色を変える
        _cursors[_stageIndex, _cursorIndex].Selected();

        if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
        _slideCoroutine = StartCoroutine(SlideStageText(oldStageIndex, _stageIndex, direction));
        IEnumerator SlideStageText(int oldIndex, int newIndex, int direction)
        {
            RectTransform oldRect = _stageTextRects[oldIndex];
            RectTransform newRect = _stageTextRects[newIndex];

            Vector2 oldHome = _stageTextHomePositions[oldIndex];
            Vector2 newHome = _stageTextHomePositions[newIndex];

            Vector2 oldOutPos = oldHome + new Vector2(-direction * _slideDistance, 0);
            Vector2 newInPos = newHome + new Vector2(direction * _slideDistance, 0);

            // 新しいテキストを反対側にジャンプさせておく
            newRect.anchoredPosition = newInPos;

            float t = 0f;
            while (t < _slideDuration)
            {
                t += Time.deltaTime;
                float ratio = Mathf.SmoothStep(0f, 1f, t / _slideDuration);

                oldRect.anchoredPosition = Vector2.Lerp(oldHome, oldOutPos, ratio);
                newRect.anchoredPosition = Vector2.Lerp(newInPos, newHome, ratio);

                yield return null;
            }

            oldRect.anchoredPosition = oldOutPos;
            newRect.anchoredPosition = newHome;
        }

    }

    /// <summary>
    /// ステージインデックスを動かす
    /// </summary>
    /// <param name="direction"></param>
    void MoveStageIndex(int direction)
    {
        //今選ばれているカーソルの色をもとに戻す
        _cursors[_stageIndex, _cursorIndex].Deselected();

        //インデックスを移動させる
        _stageIndex += direction;
        if (_stageIndex < 0)
        {
            _stageIndex = _cursors.GetLength(0) - 1;//先頭から前に行ったら末尾に移動
        }
        else if (_stageIndex >= _cursors.GetLength(0))
        {
            _stageIndex = 0;//末尾から次に行ったら先頭へ
        }
        //新しく選ばれたカーソルの色を変える
        _cursors[_stageIndex, _cursorIndex].Selected();
    }


    StageSelectCursor GetNowcursor()
    {
        //現在選んでいるインデックスから目的のカーソルを探す処理
        return _cursors[_stageIndex, _cursorIndex];
    }


}
