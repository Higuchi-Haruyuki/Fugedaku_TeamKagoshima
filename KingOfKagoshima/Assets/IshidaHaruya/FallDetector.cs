using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [Header("監視対象のプレイヤー")]
    public Transform playerTransform;

    [Header("落下判定の設定")]
    public float fallDistance = 6f; // 現在位置からどれくらい落ちたら落下にするか

    private Rigidbody2D playerRb;
    private float lastGroundedY;
    private bool isFallen = false;

    void Start()
    {
        // プレイヤーを自動で探す
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (playerTransform != null)
        {
            playerRb = playerTransform.GetComponent<Rigidbody2D>();
            lastGroundedY = playerTransform.position.y;
        }
    }

    void Update()
    {
        if (playerTransform == null || playerRb == null) return;

        // プレイヤーの上下の速度がほぼゼロ（＝地面にいる）のとき、高さを記憶する
        if (Mathf.Abs(playerRb.linearVelocity.y) < 0.01f)
        {
            lastGroundedY = playerTransform.position.y;
            isFallen = false; 
        }

       
        if (playerTransform.position.y < (lastGroundedY - fallDistance) && !isFallen)
        {
            isFallen = true; // 1回の落下で1回だけカウントする

            
            if (FallCounter.instance != null)
            {
                FallCounter.instance.AddFall();
            }
        }
    }
}