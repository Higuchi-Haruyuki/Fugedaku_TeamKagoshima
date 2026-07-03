using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// ステージカーソルなどの実行
/// ステージカーソルで選択されているときにテキストの色を変える
/// </summary>
public class StageSelectCursor : MonoBehaviour
{
    TextMeshPro _textMeshPro;

    [SerializeField] string _SteageScene2tamari;

    Color _defaultColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
        _defaultColor = _textMeshPro.color; //最初の色を記憶させる
    }

    /// <summary>
    /// 選択されたとき色が変わる
    /// </summary>    
    public void Selected()
    {
        //テキストの色を黄色に変える
        _textMeshPro.color = Color.yellow;
    }

   ///<summary>
   ///選択が外れたとき元の色に戻す
   ///<summary>
   public void Deselected()
    {
        _textMeshPro.color = _defaultColor;
    }
   

    /// <summary>
    /// スペースキーを押されたときにシーンのロード
    /// </summary>
    public void Pressed()
{
        
        SceneManager.LoadScene(_SteageScene2tamari);
}


    // Update is called once per frame
    void Update()
    {
        
    }
}
