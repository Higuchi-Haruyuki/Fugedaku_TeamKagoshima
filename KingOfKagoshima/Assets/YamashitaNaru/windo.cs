using UnityEngine;

public class IntervalWindZone2D : MonoBehaviour
{
    [Header("風の設定")]
    public float windStrength = 5f;
    public Vector2 windDirection = Vector2.right;

    [Header("時間の設定（秒）")]
    public float windOnDuration = 3f;  // 風が吹いている時間
    public float windOffDuration = 2f; // 風が止まっている時間

    private bool isWindBlowing = true; // 現在風が吹いているか
    private float timer = 0f;

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
