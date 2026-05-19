using UnityEngine;

public class TitleDrop : MonoBehaviour
{
    public Vector3 startPos;   // 開始位置（画面外の上）
    public Vector3 targetPos;  // 停止位置（中央）
    public float speed = 2f;   // 落下速度
    public float smoothTime = 0.3f; // 減速の滑らかさ

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // 初期位置を設定
        transform.localPosition = startPos;
    }

    void Update()
    {
        // ゆっくり減速しながら目的地へ移動
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition,
            targetPos,
            ref velocity,
            smoothTime,
            speed
        );
    }
}