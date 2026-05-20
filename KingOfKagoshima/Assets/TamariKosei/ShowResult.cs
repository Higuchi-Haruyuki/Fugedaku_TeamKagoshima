using UnityEngine;
using TMPro;
 public class ShowResult : MonoBehaviour
{
        // Unityの画面上のテキスト（TextMeshPro）を割り当てる枠
        public TextMeshProUGUI jumpText;

        void Start()
        {
            // 過去に保存されたデータをPlayerPrefsから直接読み込む
            // (※一度もジャンプしていない場合は 0 が表示されます)
            int totalJumps = PlayerPrefs.GetInt("TotalJumpCount", 0);

            // 画面のテキストを書き換える
            if (jumpText != null)
            {
                jumpText.text = "ジャンプ回数: " + totalJumps + " 回";
            }
        }
}

 