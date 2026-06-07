using UnityEngine;

using TMPro;

public class HaruyukiShowResult : MonoBehaviour
{
    public int stageNum = 1;
    // Unityの画面上のテキスト（TextMeshPro）を割り当てる枠
    public TextMeshProUGUI _jumpText;
    public TextMeshProUGUI _timeText;
    public TextMeshProUGUI _fallText;

    void Start()
    {

        Score score = new();
        score.LoadAll();
        Score fastestScore =new();
        fastestScore.LoadFastestTime();
        // 画面のテキストを書き換える
        if (_jumpText != null)
        {
            _jumpText.text = "ジャンプ回数: " + score.JumpCount + " 回";
        }
        if (_timeText != null)
        {
            _timeText.text = "タイム: " + score.ToString() + " s";
        }
        if (_fallText != null)
        {
            _fallText.text = "落下回数: " + score.FallCount + " 回数";
        }
    }
}






