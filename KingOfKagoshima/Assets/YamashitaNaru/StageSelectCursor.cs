using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// ステージカーソルなどの実行
/// テキストの色とサイズを変える
/// </summary>
public class StageSelectCursor : MonoBehaviour
{
    TextMeshPro _textMeshPro;

    [SerializeField] string _sceneName;//ロードするシーン
    [SerializeField] SaveFile _saveFile;
    [SerializeField] bool _isNewGame;
    [SerializeField] float _selectedSizeMultiplier = 1.2f;//選択時のサイズ倍率
    [SerializeField] float _animationDuration = 0.15f;//変化にかかる時間

    Color _defaultColor;//元の色を保存
    float _defaultFontSize;//元の文字サイズを保存
    Coroutine _animationCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
        _defaultColor = _textMeshPro.color;
        _defaultFontSize = _textMeshPro.fontSize;//現在設定されているサイズを保存
    }

    /// <summary>
    /// 選択されたときに色とサイズを変える
    /// </summary>
    public void Selected()
    {
        StartAnimation(Color.yellow, _defaultFontSize * _selectedSizeMultiplier);
    }

    ///<summary>
    ///選択されたときに元の色とサイズに戻す
    ///</summary>
    public void Deselected()
    {
        StartAnimation(_defaultColor, _defaultFontSize);
    }

    /// <summary>
    /// 色とサイズを滑らかに変化させるアニメーションを開始する
    /// </summary>
    void StartAnimation(Color targetColor, float targetFontSize)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCoroutine = StartCoroutine(AnimateTextChange(targetColor, targetFontSize));
    }

    /// <summary>
    /// 色とサイズを指定時間かけて変化させるコルーチン
    /// </summary>
    System.Collections.IEnumerator AnimateTextChange(Color targetColor, float targetFontSize)
    {
        Color startColor = _textMeshPro.color;
        float startFontSize = _textMeshPro.fontSize;
        float elapsedTime = 0f;

        while (elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _animationDuration;

            _textMeshPro.color = Color.Lerp(startColor, targetColor, t);
            _textMeshPro.fontSize = Mathf.Lerp(startFontSize, targetFontSize, t);

            yield return null;
        }

        //最終的に目標値へぴったり合わせる
        _textMeshPro.color = targetColor;
        _textMeshPro.fontSize = targetFontSize;
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