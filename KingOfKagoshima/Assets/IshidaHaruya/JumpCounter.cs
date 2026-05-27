using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpCounter : MonoBehaviour
{
    public static JumpCounter instance; // どこからでも呼び出せるようにする合言葉
    public int jumpCount { get; private set; } // 現在のジャンプ回数を数えるカウンター
    private string currentStageKey = "Stage1_Jumps"; // 保存する箱の名前（初期値）

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ★超重要：次のステージ（シーン）に切り替わっても、このオブジェクトを消さないで残す！

            // シーンが切り替わったときに、自動でお掃除（リセット）する機能をUnityに登録する
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // すでに基地があるなら、2つ目は邪魔なので消す
        }
    }
    // 新しいステージ（シーン）が読み込まれた瞬間に、Unityが自動でここを実行します
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. 今開いたステージ名に合わせて、保存する箱の名前を新しく作り直す
        // （例：Stage1を開いたら「Stage1_Jumps」、Stage2を開いたら「Stage2_Jumps」）
        currentStageKey = scene.name + "_Jumps";

        // 2. 新しいステージが始まったので、手元のカウンターの数字を「0」にリセットする！
        jumpCount = 0;

        // 3. パソコンに保存されているデータも、一度「0」に上書きしてキレイにする
        PlayerPrefs.SetInt(currentStageKey, 0);
        PlayerPrefs.Save(); // 確実にセーブを確定させる

        Debug.Log($"【{scene.name}】が開始されました。このステージのカウントを0から開始します。");
    }
    // プレイヤーがジャンプした瞬間に、外（PlayerController）から呼び出される命令
    public void AddJump()
    {
        jumpCount++; // 手元のカウンターの数字を 1 つ増やす（足し算）

        // 今のステージ専用の箱（currentStageKey）に、増えた数字を書き込んでパソコンにセーブする
        PlayerPrefs.SetInt(currentStageKey, jumpCount);
        PlayerPrefs.Save();

        Debug.Log($"{currentStageKey} のジャンプ回数: {jumpCount} 回 (保存完了)");
    }
    void OnDestroy()
    {
        // このスクリプト（基地）が万が一消されるときは、登録したお掃除の監視を解除する
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}