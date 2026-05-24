using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D col; // プレイヤーのサイズを取得するために追加

    public float moveSpeed = 5f;
    public float maxCharge = 20f;
    public float jumpPower = 1.2f;

    private float chargePower;
    private bool isGround = true;

    // ★追加：壁反射時の反発係数（1.0で勢いを維持、0.8などで少し減速）
    [Range(0f, 1.5f)] public float bounciness = 0.8f;
    Vector2 velocityBeforeFlame = Vector2.zero;
    [Header("接地判定の設定")]
    public LayerMask groundLayer; // 地面と判定するレイヤー
    public float rayLength = 0.2f; // 足元からどれくらい下にレイを伸ばすか

    private PlayerItemSystem m_playerItemSystem;
    private float m_doubleJumpPower = 10f;
    private bool m_isPressdDoubleJump = false;
    private bool m_isPressdSpaceKeyBeforeFlame = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>(); // プレイヤーのコライダーを取得
        m_playerItemSystem = GetComponent<PlayerItemSystem>();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // ★毎フレーム、足元にレイを飛ばして接地状態を更新する
        isGround = CheckGrounded();

        Move(keyboard);
        ChargeJump(keyboard);
        velocityBeforeFlame = rb.linearVelocity;
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

        // ジャンプ溜め中は左右の移動を制限する
        //いったん仮の条件
        if (keyboard.spaceKey.isPressed && rb.linearVelocityY <= 0.1f)
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
        if (!isGround)
        {
            //2段ジャンプの処理
            //既に二段ジャンプをしているときは二段ジャンプできないようにする
            if (keyboard.spaceKey.isPressed && !m_isPressdDoubleJump)
            {
                //前のフレームでスペースキーが押されてないとき
                if (!m_isPressdSpaceKeyBeforeFlame)
                {
                    ItemBase itembase = m_playerItemSystem.CheckItem("二段ジャンプ");
                    //二段ジャンプアイテムをもっているとき
                    if (itembase != null)
                    {
                        var doubleJump = itembase as Item_DoubleJump;
                        //キャストに成功したとき
                        if (doubleJump != null)
                        {
                            doubleJump.Use();
                            DoubleJump(keyboard);
                            Debug.Log($"二段ジャンプの残り回数{doubleJump.UseCount}");
                            m_isPressdDoubleJump = true;
                        }
                    }
                }
            }
        }
        else//地面に触れているとき
        {
            if (keyboard.spaceKey.isPressed)
            {
                chargePower += Time.deltaTime * 10f;
                chargePower = Mathf.Clamp(chargePower, 0, maxCharge);
            }

            if (keyboard.spaceKey.wasReleasedThisFrame || (keyboard.spaceKey.isPressed && chargePower >= maxCharge))
            {
                float x = 0f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    x = -1f;
                }
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    x = 1f;
                }

                Vector2 jumpDir = new Vector2(x, 1f).normalized;

                float jumpPowerModifier = 1f;
                ItemBase itembase = m_playerItemSystem.CheckItem("ジャンプ力上昇");
                //ジャンプ力上昇アイテムをもっているとき
                if (itembase != null)
                {
                    var jumpPowerUp = itembase as Item_JumpPowerup;
                    //キャストに成功したとき
                    if (jumpPowerUp != null)
                    {
                        jumpPowerUp.Use();
                        Debug.Log($"ジャンプ力上昇の残り回数{jumpPowerUp.UseCount}");
                        jumpPowerModifier = jumpPowerUp.m_jumpPower;
                    }
                }
                //Debug.Log($"ジャンプ方向: {jumpDir}, チャージ力: {chargePower}, ジャンプ力補正{jumpPowerModifier}");
                rb.linearVelocity = jumpDir * chargePower * jumpPower * jumpPowerModifier;
                //Debug.Log($"ジャンプ後の速度: {rb.linearVelocity}");
                chargePower = 0f;
                //ジャンプ後に二段ジャンプのフラグを戻す
                m_isPressdDoubleJump = false;

                if (JumpCounter.instance != null)
                {
                    JumpCounter.instance.AddJump();
                }
            }
        }
        if (keyboard.spaceKey.isPressed)
        {
            m_isPressdSpaceKeyBeforeFlame = true;
        }
        else
        {
            m_isPressdSpaceKeyBeforeFlame = false;
        }

    }
    //二段ジャンプのジャンプ
    private void DoubleJump(Keyboard keyboard)
    {
        float x = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            x = -1f;
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            x = 1f;
        }
        Vector2 jumpDir = new Vector2(x, 1f).normalized;

        float jumpPowerModifier = 1f;
        ItemBase itembase = m_playerItemSystem.CheckItem("ジャンプ力上昇");
        //ジャンプ力上昇アイテムをもっているとき
        if (itembase != null)
        {
            var jumpPowerUp = itembase as Item_JumpPowerup;
            //キャストに成功したとき
            if (jumpPowerUp != null)
            {
                jumpPowerUp.Use();
                jumpPowerModifier = jumpPowerUp.m_jumpPower;
            }
        }

        chargePower = m_doubleJumpPower;
        //Debug.Log($"ジャンプ方向: {jumpDir}, チャージ力: {chargePower}, ジャンプ力補正{jumpPowerModifier}");
        rb.linearVelocity = jumpDir * chargePower * jumpPower * jumpPowerModifier;
        chargePower = 0f;

        if (JumpCounter.instance != null)
        {
            JumpCounter.instance.AddJump();
        }
    }
    // ★新規追加：壁に衝突した瞬間の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 地面にいるときは反射しない（壁や天井にぶつかったときだけ）
        if (isGround) return;

        // 衝突した面（最初の接触点）の情報を取得
        ContactPoint2D contact = collision.contacts[0];

        // 壁の法線ベクトル（壁が向いている正面の向き）
        Vector2 wallNormal = contact.normal;

        // 【重要】真下を向いている法線（＝床）や、斜めすぎる床は除外する
        // wallNormal.y が 0.7 以上の場合は「ほぼ床」とみなして反射処理をスキップ
        if (wallNormal.y > 0.7f) return;
        //Debug.Log($"壁の法線: {wallNormal}, 速度: {velocityBeforeFlame}");
        // 現在の速度ベクトルを、壁の法線を基準に反射させる
        Vector2 reflectDir = Vector2.Reflect(velocityBeforeFlame, wallNormal);

        // 反射ベクトルに勢い（bounciness）をかけて速度を再設定
        rb.linearVelocity = reflectDir * bounciness;
    }
}