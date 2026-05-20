using UnityEngine;

public class CameraController : MonoBehaviour
{
    // インスペクターでプレイヤーを割り当ててください
    public Transform player;

    // カメラがプレイヤーを追いかけるスピード
    public float followSpeed = 5f;

    // ゲーム開始時の、カメラとプレイヤーの「前後の距離（Z軸）」だけを記憶する変数
    private float offsetZ;

    void Start()
    {
        if (player != null)
        {
            // 前後の距離（奥行き）だけを計算して覚えておく
            offsetZ = transform.position.z - player.position.z;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 【ここを改造しました】
        // 理想の位置を計算するとき、X座標（左右）はプレイヤーと「完全に同じ（ズレなし）」にします
        Vector3 targetPosition = new Vector3(
            player.position.x,          // 左右は常にプレイヤーの真ん中
            player.position.y,          // 上下はプレイヤーと同じ高さ
            player.position.z + offsetZ // 前後は最初の奥行きをキープ
        );

        // 現在のカメラの位置から、真ん中の理想位置へ滑らかに移動させる
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // カメラに位置を反映
        transform.position = smoothedPosition;
    }
}