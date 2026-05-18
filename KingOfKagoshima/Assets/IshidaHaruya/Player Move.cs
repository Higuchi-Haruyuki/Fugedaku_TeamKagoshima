using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D col; // プレイヤーのサイズを取得するために追加

    public float moveSpeed = 5f;
    public float maxCharge = 20f;

    private float chargePower;
    private bool isGround = true;

    [Header("接地判定の設定")]
    public LayerMask groundLayer; // 地面と判定するレイヤー
    public float rayLength = 0.2f; // 足元からどれくらい下にレイを伸ばすか

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>(); // プレイヤーのコライダーを取得
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // ★毎フレーム、足元にレイを飛ばして接地状態を更新する
        isGround = CheckGrounded();

        Move(keyboard);
        ChargeJump(keyboard);
    }

    // ★新規追加：レイキャストによる接地判定メソッド
    bool CheckGrounded()
    {
        // プレイヤーの足元の中心座標を計算
        Vector2 footPos = new Vector2(col.bounds.center.x, col.bounds.min.y);

        // 崖っぷち対策：プレイヤーの横幅の半分のサイズを取得
        float halfWidth = col.bounds.extents.x;

        // 左足と右足の位置を計算（少し内側にずらすとより安定します）
        Vector2 leftFoot = footPos + Vector2.left * (halfWidth * 0.8f);
        Vector2 rightFoot = footPos + Vector2.right * (halfWidth * 0.8f);

        // デバッグ用：UnityのScene画面に見えないレーザーを赤色で表示する
        Debug.DrawRay(leftFoot, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rightFoot, Vector2.down * rayLength, Color.red);

        // 左足、または右足の真下に「groundLayer」があるかチェック
        bool hitLeft = Physics2D.Raycast(leftFoot, Vector2.down, rayLength, groundLayer);
        bool hitRight = Physics2D.Raycast(rightFoot, Vector2.down, rayLength, groundLayer);

        // どちらか一方が地面に当たっていれば true を返す
        return hitLeft || hitRight;
    }

    void Move(Keyboard keyboard)
    {
        if (!isGround) return;

        if (keyboard.spaceKey.isPressed)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float x = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x = -1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x = 1f;

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    void ChargeJump(Keyboard keyboard)
    {
        if (!isGround) return;

        if (keyboard.spaceKey.isPressed)
        {
            chargePower += Time.deltaTime * 10f;
            chargePower = Mathf.Clamp(chargePower, 0, maxCharge);
        }

        if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            float x = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x = 1f;

            Vector2 jumpDir = new Vector2(x, 1f).normalized;

            rb.linearVelocity = jumpDir * chargePower;

            chargePower = 0f;
            // ※isGround = false は、次のフレームの CheckGrounded() で自動計算されるため不要になります
        }
    }
}