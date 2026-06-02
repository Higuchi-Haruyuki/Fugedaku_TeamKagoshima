using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class Yamashitacontorore : MonoBehaviour
{
    Rigidbody2D rb;


    public float moveSpeed = 5f;
    public float maxCharge = 20f;
    public float jumpPower = 1.2f;
    public float jumpHeight = 5f;
    private float chargePower;
    private bool isGround = true;
    private bool isIceGround = false;
    public float slipperiness = 0.97f;
    private Vector2 jumpLimit;
    // ★追加：壁反射時の反発係数（1.0で勢いを維持、0.8などで少し減速）
    [Range(0f, 1.5f)] public float bounciness = 1f;
    Vector2 velocityBeforeFlame = Vector2.zero;
    [Header("接地判定の設定")]
    private LayerMask groundLayer; // 地面と判定するレイヤー
    private bool isIceGround = false;
    public float slipperiness = 0.97f;

    private PlayerItemSystem m_playerItemSystem;
    private float m_doubleJumpPower = 10f;
    private bool m_isPressdDoubleJump = false;
    private bool m_isPressdSpaceKeyBeforeFlame = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        m_playerItemSystem = GetComponent<PlayerItemSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;



        Move(keyboard);
        ChargeJump(keyboard);
        velocityBeforeFlame = rb.linearVelocity;

        if (isGround)
        {
            spriteRenderer.color = Color.red;

        }
        else
        {
            spriteRenderer.color = Color.green;
        }
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
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            x = -1f; 
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) 
        {
            x = 1f; 
        }
        //キー入力をしていないとき
        if(!(Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ||
                    Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed))
        {
            //かつ　氷の地面に立っているとき
            if(isIceGround)
            {
                Debug.Log($"滑っているよ{rb.linearVelocity}");
                var playerVelocityOnSlip = rb.linearVelocity;
                // 現在の速度にslipperinessを掛け算して、じわじわとしか減速させない
                playerVelocityOnSlip.x *= slipperiness;

                // 速度を更新（ジャンプ力などのY軸の速度は変えない）
                rb.linearVelocity = playerVelocityOnSlip;
            }
            //かつ　立っていないとき
            else
            {
                rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
            }
        }
        //キー入力をしているとき
        else
        {
            rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
        }
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
                Debug.Log($"ジャンプ方向: {jumpDir}, チャージ力: {chargePower}, ジャンプ力補正{jumpPowerModifier}");
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
    private void OnCollisionStay2D(Collision2D collision)
    {

        // Debug.Log($"衝突したオブジェクトのレイヤー: {collision.gameObject.layer}, 地面レイヤー: {groundLayer}");
        if (collision.gameObject.layer == groundLayer)
        {
            Vector3 contactNormal = collision.contacts[0].normal;
            if (contactNormal == new Vector3(0, 1, 0))
            {
                //    Debug.Log("aaaa");
                isGround = true;
                if(collision.gameObject.GetComponent<IceFloor>())
                {
                    isIceGround = true;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            isGround = false;
            if (collision.gameObject.GetComponent<IceFloor>())
            {
                isIceGround = false;
            }
        }
    }
    // ★新規追加：壁に衝突した瞬間の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {



        if (isGround) return;

        // ★ここを追加：もし衝突した点（contacts）が1つも無ければ、処理をスキップする
        if (collision.contacts == null || collision.contacts.Length == 0) return;

        // 安全が確認できたら、今まで通り処理を行う
        ContactPoint2D contact = collision.contacts[0];
        Vector2 wallNormal = contact.normal;

        // 天井や床（上向きの面）は除外する
        if (wallNormal.y > 0.7f) return;

        Vector2 reflectDir = Vector2.Reflect(velocityBeforeFlame, wallNormal);
        rb.linearVelocity = reflectDir * bounciness;
    }
}