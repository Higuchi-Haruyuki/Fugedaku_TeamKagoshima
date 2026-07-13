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
    [SerializeField] SaveFile _saveFile;
    [SerializeField] bool _isNewGame;
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
        //新しく始めるときは新しくファイルを上書きする。
        if (_isNewGame) 
        {
            SaveData saveData = new();
            SaveManager.SaveJson(saveData, SaveManager.GetPath(_saveFile));
        }
        Debug.Log("Pressed呼ばれた。sceneName = " + _sceneName);
        SceneManager.LoadScene(_sceneName);
    }
}