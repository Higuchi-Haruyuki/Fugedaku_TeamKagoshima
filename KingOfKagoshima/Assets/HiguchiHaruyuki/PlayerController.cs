using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputSystem))]
[RequireComponent(typeof(PlayerItemSystem))]

public class PlayerController : MonoBehaviour
{
    //<SerializeField>
    [Header("プレイヤーのステータス")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _minCharge = 10f;
    [SerializeField] private float _maxCharge = 20f;
    [SerializeField] private float _jumpPower = 1.2f;
    [SerializeField] private float _minJumpAngle = 30f;
    [SerializeField] private float _maxJumpAngle = 70f;

    [Header("壁にぶつかったときの反射")]
    //壁反射時の反発係数（1.0で勢いを維持、0.8などで少し減速）
    [SerializeField] private float _bounciness = 1f;

    [Header("氷の地面を歩いたときの滑り度")]
    [SerializeField] [Range(0f,1.0f)] private float _slipperiness = 0.97f;

    [Header("アイテム")]
    //ジャンプ飛距離上昇のアイテムが二段ジャンプに影響を与えるか 
    [SerializeField] private bool _isJumpPowerUpEffectOnDoubleJump = false;
    [SerializeField] private float _doubleJumpPower = 10f;

    [Header("落下判定を出す高さ")]
    [SerializeField] private float _fallDistance = 6.0f;

    //<フラグ>
    //二段ジャンプが許可されているか(アイテムの所持状況は考慮しない)
    private bool _isEnableDoubleJump = false;
    //地面に触れているか
    private bool _isGround = true;
    //今ジャンプをためているか
    private bool _isChargeJump = false;
    //今落下中か
    private bool _isFallen = false;
    //氷の地面にたっているか
    private bool _isIceGround = false;

    //<コンポーネントの変数>
    private PlayerItemSystem _itemSystem;
    private PlayerInputSystem _inputSystem;   
    private Rigidbody2D _rb;
    private Collider2D _col;

    //<イベント>
    public Action OnJump;
    public Action OnFall;

    //<その他>
    // 地面と判定するレイヤー
    private LayerMask _groundLayer;
    //最後に立っていた地面の高さ
    private float _lastGroundY;

    private const float _jumpPowerModifier = 0.20f;
    private float _jumpChargeX = 0f;
    private float _chargePower;
    Vector2 _velocityBeforeFlame = Vector2.zero;

    void Start()
    {
        Application.targetFrameRate = 60;
        _rb = GetComponent<Rigidbody2D>();
        _itemSystem = GetComponent<PlayerItemSystem>();
        _inputSystem = GetComponent<PlayerInputSystem>();
        _col = GetComponent<Collider2D>();
        _groundLayer = LayerMask.NameToLayer("Ground");
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        //地面の上に立っているとき
        if(_isGround)
        {
            CheckChargeing();
            Move();
            //氷の地面に立っているとき
            if(_isIceGround) SlipIceGround();
            ChargeJump();

            //落下回数に関する処理
            _lastGroundY = transform.position.y;
            _isFallen = false;
        }
        //地面の上に立っていないとき
        else
        {
            //二段ジャンプが許可されているとき
            if(_isEnableDoubleJump) DoubleJump();
        }

        //落下していないフラグがたっているときかつ地面から指定した分落下しているとき
        if(!_isFallen && transform.position.y < (_lastGroundY - _fallDistance))
        {
            _isFallen = true;
            OnFall?.Invoke();
        }

        //前のフレームでの速度を保存しておく
        _velocityBeforeFlame = _rb.linearVelocity;

    }
    void CheckChargeing()
    {
        if (_inputSystem.IsPressedJumpKey())
        {
            _chargePower++;
            _chargePower = Mathf.Clamp(_chargePower, 0, _maxCharge);
            _isChargeJump = true;
            return;
        }
        _isChargeJump = false;
    }

    void Move()
    {
        float x = 0f;

        if (_inputSystem.IsPressedLeftKey()) x = -1;
        if (_inputSystem.IsPressedRightKey()) x = 1f;

        //ジャンプため中は入力無効
        if(_isChargeJump) x = 0f;

        //氷の地面に入力なしでたっているとき以外
        if(!_isIceGround || x !=0)
        {
            _rb.linearVelocity = new Vector2(x * _moveSpeed, _rb.linearVelocity.y);
        }

    }
    void Jump(float chargePower,float jumpPowerModifier)
    {
        //ジャンプ方向入力の受付
        SetJumpChargeX();
        //ジャンプ力の計算
        var power = chargePower + _minCharge;
        float jumpPower = Mathf.Min(power,_maxCharge) * _jumpPower * jumpPowerModifier * _jumpPowerModifier;

        //ジャンプ角度の計算(溜めた割合に応じてジャンプ角度が上昇する)
        //線形補間で角度を出す
        var temp = chargePower / _maxCharge;
        var angle = Mathf.Lerp(_minJumpAngle, _maxJumpAngle, temp);

        //正規化されたジャンプ方向ベクトルを出す
        var cos = Mathf.Cos(angle * Mathf.Deg2Rad) * _jumpChargeX;
        var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        var targetVec = new Vector2(cos, sin).normalized;

        //ジャンプ力と乗算してベクトルをだす
        var jumpVec = targetVec * jumpPower;

        _rb.linearVelocity = jumpVec;

        _jumpChargeX = 0f;
        OnJump?.Invoke();
    }

    void ChargeJump()
    {
        //ジャンプキーを離したときまたはチャージ値が最大チャージ値を超えたときにジャンプする
        if (_inputSystem.IsReleasedJumpKey() || _chargePower >= _maxCharge)
        {
            SetJumpChargeX();
            //ジャンプ力上昇アイテムの処理 所持しているなら使用して補正をかける
            float jumpPowerModifier = 1f;
            jumpPowerModifier = Item_JumpPowerup.UseItem(_itemSystem);

            Jump(_chargePower, jumpPowerModifier);

            _chargePower = 0.0f;
            //ジャンプ後に二段ジャンプのフラグを戻す
            _isEnableDoubleJump = true;
        }
    }
    /// <summary>
    /// ジャンプため中にジャンプの向きの入力を受け付ける関数
    /// </summary>
    void SetJumpChargeX()
    {
        if (_inputSystem.IsPressedLeftKey()) _jumpChargeX = -1;
        if (_inputSystem.IsPressedRightKey()) _jumpChargeX = 1f;
    }
    //地面を滑る処理
    void SlipIceGround()
    {
        //キー入力をしていないとき
        if (!_inputSystem.IsPressedMoveKey())
        {
            var playerVelocityOnSlip = _velocityBeforeFlame;
            // 現在の速度にslipperinessを掛け算して、じわじわとしか減速させない
            playerVelocityOnSlip.x *= _slipperiness;

            // 速度を更新（ジャンプ力などのY軸の速度は変えない）
            _rb.linearVelocity = playerVelocityOnSlip;
        }
    }
    //二段ジャンプの処理
    void DoubleJump()
    {
        if (_inputSystem.IsPressedThisFlameJumpKey())
        {
            //二段ジャンプアイテムを持っているとき
            if (Item_DoubleJump.UseItem(_itemSystem))
            {
                float jumpPowerModifier = 1f;
                //ジャンプ飛距離上昇のアイテムが二段ジャンプに影響を与えるとき
                if (_isJumpPowerUpEffectOnDoubleJump)
                {
                    jumpPowerModifier = Item_JumpPowerup.UseItem(_itemSystem);
                }
                Jump(_doubleJumpPower, jumpPowerModifier);

                _isEnableDoubleJump = false;
            }
        }
    }
    /// <summary>
    /// 押し戻す処理
    /// </summary>
    /// <param name="contactPoint">衝突点情報が入った変数</param>
    /// <param name="contactNormal">法線ベクトル</param>
    private void PushBack(Vector3 normal, float distance)
    {
        Debug.Log($"2D 法線: {normal}, めり込み量: {distance}");
        transform.position += distance * normal.normalized;
        //下方向に落下しているとき かつ プレイヤーから伸びる法線ベクトルが下方向を向いているとき 速度を0にする
        if(_rb.linearVelocityY < 0.0f && normal.y < 0.0f)
        {
            _rb.linearVelocityY = 0.0f;
        }

    }
    //Triggerオンにしてるとき用
    private void OnTriggerStay2D(Collider2D collision)
    {
        ColliderDistance2D distance2D = Physics2D.Distance(_col, collision);

        Vector3 normal = distance2D.normal;
        float distance = distance2D.distance;

        if (distance2D.isOverlapped)
        {
            //押し戻し処理
            PushBack(normal,distance);
        }
        //ぶつかったオブジェクトが地面のとき
        if (collision.gameObject.layer == _groundLayer)
        {
            //法線ベクトルが下方向を向いているとき
            if (normal == new Vector3(0, -1.0f, 0))
            {
                _isGround = true;
                if (collision.gameObject.CompareTag("IceGround"))
                {
                    _isIceGround = true;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _groundLayer)
        {
            _isGround = false;
            if (collision.gameObject.CompareTag("IceGround"))
            {
                _isIceGround = false;
            }
        }
    }
    // ★新規追加：壁に衝突した瞬間の処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColliderDistance2D distance2D = Physics2D.Distance(_col, collision);

        Vector3 normal = distance2D.normal;
        float distance = distance2D.distance;

        if (distance2D.isOverlapped)
        {
            //押し戻し処理
            PushBack(normal, distance);
        }

        //地面の上にいるときはこれ以降の処理をしない
        if (_isGround) return;

        // 床は除外する
        if (normal.y < 0.0f) return;

        Vector2 reflectDir = Vector2.Reflect(_velocityBeforeFlame, normal);
        _rb.linearVelocity = reflectDir * _bounciness;
    }

    //Triggerオフにしてるとき用
#if true
    private void OnCollisionStay2D(Collision2D collision)
    {

        // Debug.Log($"衝突したオブジェクトのレイヤー: {collision.gameObject.layer}, 地面レイヤー: {groundLayer}");
        if (collision.gameObject.layer == _groundLayer)
        {
            Vector3 contactNormal = collision.contacts[0].normal;
            if (contactNormal == new Vector3(0, 1, 0))
            {
                _isGround = true;
                if (collision.gameObject.CompareTag("IceGround"))
                {
                    _isIceGround = true;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == _groundLayer)
        {
            _isGround = false;
            if (collision.gameObject.CompareTag("IceGround"))
            {
                _isIceGround = false;
            }
        }
    }
    // ★新規追加：壁に衝突した瞬間の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (_isGround) return;

        // ★ここを追加：もし衝突した点（contacts）が1つも無ければ、処理をスキップする
        if (collision.contacts == null || collision.contacts.Length == 0) return;

        // 安全が確認できたら、今まで通り処理を行う
        ContactPoint2D contact = collision.contacts[0];
        Vector2 wallNormal = contact.normal;

        // 天井や床（上向きの面）は除外する
        if (wallNormal.y > 0.7f) return;

        Vector2 reflectDir = Vector2.Reflect(_velocityBeforeFlame, wallNormal);
        _rb.linearVelocity = reflectDir * _bounciness;
    }
#endif
}
