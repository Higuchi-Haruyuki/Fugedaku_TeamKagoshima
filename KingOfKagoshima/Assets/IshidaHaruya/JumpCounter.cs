using UnityEngine;

public class JumpCounter : MonoBehaviour
{
    // どこからでもこのスクリプトを呼び出せるようにする仕組み（シングルトン）
    public static JumpCounter instance;

    // 現在のジャンプ回数（他のスクリプトからは読み取りだけできるように設定）
    public int jumpCount { get; private set; }

    void Awake()
    {
        // シーンを切り替えても、この保存オブジェクトが消えないようにする
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも残す設定

            // ゲーム開始時に、過去に保存されたデータを読み込む
            LoadJumpCount();
        }
        else
        {
            // すでに存在している場合は重複して増えないように消す
            Destroy(gameObject);
        }
    }

    // ジャンプ回数を1増やして保存するメソッド
    public void AddJump()
    {
        jumpCount++;

        // PlayerPrefsを使ってパソコンやスマホに数値を保存
        PlayerPrefs.SetInt("TotalJumpCount", jumpCount);
        PlayerPrefs.Save(); // 確実に保存を確定させる

        //Debug.Log($"ジャンプ回数: {jumpCount} 回 (シーンをまたいで保存されました)");
    }

    // データを読み込む処理
    private void LoadJumpCount()
    {
        // 過去に一度も保存されていない場合は「0」からスタートします
        jumpCount = PlayerPrefs.GetInt("TotalJumpCount", 0);
        Debug.Log($"過去のデータを読み込みました。現在のジャンプ総数: {jumpCount} 回");
    }

    // テスト用：もし回数を0にリセットしたい時に呼び出す機能
    public void ResetJumpCount()
    {
        jumpCount = 0;
        PlayerPrefs.SetInt("TotalJumpCount", 0);
        PlayerPrefs.Save();
        Debug.Log("ジャンプ回数をリセットしました");
    }
}