using UnityEngine;
using UnityEngine.InputSystem;

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

    int _cursorIndex = 0;//初めからと途中で選択中
    int _stageIndex = 0;//左右選択中
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Findで目当てのオブジェクトを取得して保存しとく
        _cursors[0, 0] = GameObject.Find("StageText").GetComponent<StageSelectCursor>();
        _cursors[0, 1] = GameObject.Find("Save1").GetComponent<StageSelectCursor>();
        _cursors[1, 0] = GameObject.Find("StageText2").GetComponent<StageSelectCursor>();
        _cursors[1, 1] = GameObject.Find("Save2").GetComponent<StageSelectCursor>();



        //最初は0番目を選択状態にしておく
        _cursors[0, _cursorIndex] . Selected(); 
    }






    // Update is called once per frame
    void Update()
    {
        //下キー入力で次のステージの選択
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) 
        {
            Move(1);
        }

        //上キーで決定
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            Move(-1);
        }
        //左キー入力でステージの選択
        if (Keyboard.current.leftAltKey.wasPressedThisFrame)
        {
            //ステージを切り替える
         //_stageIndex = _[1, _stageIndex];
        }
        //右キーでステージ選択
        if(Keyboard.current.rightAltKey.wasPressedThisFrame)
        {
            //ステージを切り替える
        }

        //スペースキーで決定
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _cursors[0, _cursorIndex].Pressed();
        }
    }

    /// <summary>
    /// 選択インデックスを動かす
    /// </summary>
    /// <param name="direction"></param>
    void Move(int direction)
    {
        //今選ばれているカーソルの色をもとに戻す
        _cursors[0, _cursorIndex].Deselected();

        //インデックスを移動させる
        _cursorIndex += direction;
        if (_cursorIndex < 0)
        {
            _cursorIndex = _cursors.Length - 1;//先頭から前に行ったら末尾に移動
        }
        else if (_cursorIndex >= _cursors.Length)
        {
            _cursorIndex = 0;//末尾から次に行ったら先頭へ
        }
        //新しく選ばれたカーソルの色を変える
        _cursors[0,_cursorIndex].Selected();
    }

    StageSelectCursor GetNowcursor()
    {
        //現在選んでいるインデックスから目的のカーソルを探す処理
        return _cursors[_stageIndex, _cursorIndex];
    }


}
