using TMPro;
using UnityEngine;

public  class ShowResult : MonoBehaviour
{
    public int stageNum = 1;
    // Unityの画面上のテキスト（TextMeshPro）を割り当てる枠
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI timeText;


    void Start()
    {
        // 過去に保存されたデータをPlayerPrefsから直接読み込む
        // (※一度もジャンプしていない場合は 0 が表示されます)
        int totalJumps = PlayerPrefs.GetInt("TotalJumpCount", 0);

        Score scoreTime = new();
        scoreTime.LoadTime(stageNum);

        // 画面のテキストを書き換える
        if (jumpText != null)
        {
            jumpText.text = "ジャンプ回数: " + totalJumps + " 回";
        }
        if (timeText != null)
        {
            timeText.text = "タイム: " + scoreTime.ToString() + " s";
        }
    }
}





