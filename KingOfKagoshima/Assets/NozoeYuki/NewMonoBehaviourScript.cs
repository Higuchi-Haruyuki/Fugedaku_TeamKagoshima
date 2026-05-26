using UnityEngine;

public class IntervalWindZone2D : MonoBehaviour
{
    [Header("風の設定")]
    public float windStrength = 5f;
    public Vector2 windDirection = Vector2.right;

    [Header("時間の設定（秒）")]
    public float windOnDuration = 3f;  // 風が吹いている時間
    public float windOffDuration = 2f; // 風が止まっている時間

    [Header("連動させるパーティクル")] // 【追加】
    public ParticleSystem windParticle; // 【追加】 インスペクターで設定

    private bool isWindBlowing = true; // 現在風が吹いているか
    private float timer = 0f;

    void Start()
    {
        // 最初にパーティクルの状態を初期化
        UpdateParticleState();
    }

    void Update()
    {
        // タイマーを進める
        timer += Time.deltaTime;

        // 風が吹いている時
        if (isWindBlowing)
        {
            if (timer >= windOnDuration)
            {
                isWindBlowing = false;
                timer = 0f;
                Debug.Log("風が止まりました");
                UpdateParticleState(); // 【追加】
            }
        }
        // 風が止まっている時
        else
        {
            if (timer >= windOffDuration)
            {
                isWindBlowing = true;
                timer = 0f;
                Debug.Log("風が吹き始めました");
                UpdateParticleState(); // 【追加】
            }
        }
    }

    // 【追加】 パーティクルの再生・停止を切り替える関数
    private void UpdateParticleState()
    {
        if (windParticle != null)
        {
            if (isWindBlowing)
            {
                windParticle.Play(); // 風が吹いているときは再生
            }
            else
            {
                windParticle.Stop(); // 風が止まっているときは停止
            }
        }
    }

    // エリア内にオブジェクトがいる間の処理
    private void OnTriggerStay2D(Collider2D other)
    {
        // 風が吹いていない時は何もしない
        if (!isWindBlowing) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 force = windDirection.normalized * windStrength;
            rb.AddForce(force, ForceMode2D.Force);
        }
    }
}