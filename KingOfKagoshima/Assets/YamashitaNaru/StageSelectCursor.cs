using TMPro;
using UnityEngine;
/// <summary>
/// ステージカーソルなどの実行
/// テキストの色を変える
/// </summary>
public class StageSelectCursor : MonoBehaviour
{
    TextMeshPro _textMeshPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
    }

    /// <summary>
    /// 選択されたとき色が変わる
    /// </summary>    
    public void Selected()
    {
        //テキストの色を黄色に変える

    }

    /// <summary>
    /// 押されたときにシーンのロード
    /// </summary>
    public void Pressed()
{
        //
}


    // Update is called once per frame
    void Update()
    {
        
    }
}
