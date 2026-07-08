using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// ステージカーソルなどの実行
/// テキストの色を変える
/// </summary>
public class StageSelectCursor : MonoBehaviour
{
    TextMeshPro _textMeshPro;

    [SerializeField] string _sceneName;//ロードするシーン

    Color _defaultColor;//元の色を保存

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
        _defaultColor = _textMeshPro.color;
    }

    /// <summary>
    public void Selected()
    {
        //テキストの色を黄色に変える

        _textMeshPro.color = Color.yellow;
    }

    ///<summary>
    ///選択されたときに元の色に変える
    ///</summary>
    public void Deselected()
    {
        _textMeshPro.color = _defaultColor;
    }
    /// <summary>
    /// 押されたときにシーンのロード
    /// スペースキーを押されたときにシーンのロード
    /// </summary>
    public void Pressed()
    {
        Debug.Log("Pressed呼ばれた。sceneName = " + _sceneName);
        SceneManager.LoadScene(_sceneName);
    }
}