using UnityEngine;
using UnityEngine.InputSystem;

public class IceFloor : MonoBehaviour
{
    [Header("滑りやすさの設定")]
    [Tooltip("値が1に近いほど摩擦が少なく、いつまでも滑ります (0.90 ～ 0.99 くらいがおすすめ)")]
    [Range(0.8f, 0.999f)]
    public float slipperiness = 0.97f;
    private Vector3 playerVelocityOnSlip = Vector3.zero;
    // プレイヤーが氷の床に乗っている間、毎フレーム実行される
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 接触したオブジェクトがプレイヤーかどうかを判定 (Tagが"Player"の場合)
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // プレイヤーの入力がない（＝キーを離した）ときだけ慣性で滑らせる
                if(!(Keyboard.current.aKey.isPressed||Keyboard.current.leftArrowKey.isPressed || 
                    Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed))
                {
                    Debug.Log($"滑っているよ{playerRb.linearVelocity}");
                    playerVelocityOnSlip = playerRb.linearVelocity;
                    // 現在の速度にslipperinessを掛け算して、じわじわとしか減速させない
                    playerVelocityOnSlip.x *= slipperiness;

                    // 速度を更新（ジャンプ力などのY軸の速度は変えない）
                    playerRb.linearVelocity = playerVelocityOnSlip;
                }
            }
        }
    }
}