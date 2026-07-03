using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
/// <summary>
///ステージ選択の全体を操作する 
///カーソルで選んでステージを選択と選択されているときは文字を黄色にする、実行処理がきちんと選択されているとモノだけに実行処理がされる
///文字テキストを左右にスライドさせる
/// </summary>

public class StageSelectManeger : MonoBehaviour
{
    [SerializeField] StageSelectCursor[] _cursors;

    [SerializeField] float _slideSpeed = 2f;
    [SerializeField] float _slideWidth = 10f;

    int  _currentIndex = 0;//選択中のステージの番号
    Vector3 _basePosition;//選択中のカーソルの元の位置

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //最初はゼロ番目
        _cursors[_currentIndex].Selected();
        _basePosition = _cursors[_currentIndex].transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
          // MovedFromAttribute()
        }
    }
}
