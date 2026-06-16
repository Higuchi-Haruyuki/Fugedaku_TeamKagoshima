using UnityEngine;

public class ScreenCameraController : MonoBehaviour
{
    public Transform playerTransform;
    private Vector2 screenSize;
    private Camera cam;

    // 追加：最初のカメラのX座標を固定するための変数
    private float fixedXPosition;

    void Start()
    {
        cam = GetComponent<Camera>();

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        screenSize = new Vector2(width, height);

        // ゲーム開始時のカメラのX座標（中心位置）を記憶
        fixedXPosition = transform.position.x;
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // 縦方向（Y軸）だけ何番目の部屋かを計算する
        int currentY = Mathf.FloorToInt((playerTransform.position.y + screenSize.y / 2f) / screenSize.y);

        // X座標は固定（fixedXPosition）、Y座標だけ計算結果を適用する
        Vector3 targetPosition = new Vector3(fixedXPosition, currentY * screenSize.y, transform.position.z);

        transform.position = targetPosition;
    }
}