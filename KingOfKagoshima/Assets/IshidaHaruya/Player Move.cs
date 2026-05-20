using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerItemSystem))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D col; // プレイヤーのサイズを取得するために追加

    public float moveSpeed = 5f;
    public float maxCharge = 20f;

    private float chargePower;
    private bool isGround = true;

    // ★追加：壁反射時の反発係数（1.0で勢いを維持、0.8などで少し減速）
    [Range(0f, 1.5f)] public float bounciness = 0.8f;
    Vector2 velocityBeforeFlame = Vector2.zero;
    [Header("接地判定の設定")]
    public LayerMask groundLayer; // 地面と判定するレイヤー
    public float rayLength = 0.2f; // 足元からどれくらい下にレイを伸ばすか

    //haruyukiが追加 アイテム処理用変数
    private PlayerItemSystem m_playerItemSystem;
    private bool m_isDoubleJumped = false;
    [SerializeField] private float m_doubleJumpPower = 5f; 

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
        if (!isGround)
        {
            //既に二段ジャンプしているときは二段ジャンプしない
            if(m_isDoubleJumped) return;
            //二段ジャンプの処理
            if (keyboard.spaceKey.wasReleasedThisFrame)
            {
                Debug.Log("二段ジャンプ！");
                ItemBase itembase = m_playerItemSystem.CheckItem("二段ジャンプ");
                //二段ジャンプアイテムをもっているとき
                if (itembase != null)
                {
                    var item_DoubleJump = itembase as Item_DoubleJump;
                    //キャストに成功したとき
                    if (item_DoubleJump != null)
                    {
                        DoubleJump(keyboard);
                        item_DoubleJump.Use();
                        Debug.Log($"二段ジャンプ残り回数:{item_DoubleJump.UseCount}");
                        m_isDoubleJumped = true;
                    }
                }
            }
            return; 
        }

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

            float jumpPowerupModifier = 1;
            //ジャンプ力上昇処理
            ItemBase itembase = m_playerItemSystem.CheckItem("ジャンプ力上昇");
            //二段ジャンプアイテムをもっているとき
            if (itembase != null)
            {
                var item_JumpPowerup = itembase as Item_JumpPowerup;
                //キャストに成功したとき
                if (item_JumpPowerup != null)
                {
                    DoubleJump(keyboard);
                    item_JumpPowerup.Use();
                    Debug.Log($"ジャンプ力上昇残り回数:{item_JumpPowerup.UseCount}");
                    jumpPowerupModifier = item_JumpPowerup.m_jumpPower;
                }
            }

            rb.linearVelocity = jumpDir * chargePower * jumpPowerupModifier;

            chargePower = 0f;
            m_isDoubleJumped = false;
        }
    }

    //二段ジャンプ用のジャンプ（長押しではなく単押し）
    private void DoubleJump(Keyboard keyboard)
    {
        float x = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x = -1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x = 1f;

        Vector2 jumpDir = new Vector2(x, 1f).normalized;

        rb.linearVelocity = jumpDir * m_doubleJumpPower;
    }
    // ★新規追加：壁に衝突した瞬間の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 地面にいるときは反射しない（壁や天井にぶつかったときだけ）
        if (isGround) return;
        Debug.Log("壁に当たったよ");

        // 衝突した面（最初の接触点）の情報を取得
        ContactPoint2D contact = collision.contacts[0];

        // 壁の法線ベクトル（壁が向いている正面の向き）
        Vector2 wallNormal = contact.normal;

        // 【重要】真下を向いている法線（＝床）や、斜めすぎる床は除外する
        // wallNormal.y が 0.7 以上の場合は「ほぼ床」とみなして反射処理をスキップ
        if (wallNormal.y > 0.7f) return;
        Debug.Log($"壁の法線: {wallNormal}, 速度: {velocityBeforeFlame}");
        // 現在の速度ベクトルを、壁の法線を基準に反射させる
        Vector2 reflectDir = Vector2.Reflect(velocityBeforeFlame, wallNormal);

        // 反射ベクトルに勢い（bounciness）をかけて速度を再設定
        rb.linearVelocity = reflectDir * bounciness;
    }
}