using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Haruyuki_PlayerController : MonoBehaviour
{

    [Header("プレイヤーのステータス")]
    public float _moveSpeed = 5f;
    public float _minCharge = 10f;
    public float _maxCharge = 20f;
    public float _jumpPower = 1.2f;
    public float _minJumpAngle = 30f;
    public float _maxJumpAngle = 70f;
    private const float _jumpPowerModifier = 0.20f;
    private float _jumpChargeX = 0f;
    private float _chargePower;
    Vector2 _velocityBeforeFlame = Vector2.zero;


    [Header("壁にぶつかったときの反射")]
    //壁反射時の反発係数（1.0で勢いを維持、0.8などで少し減速）
    [SerializeField] private float _bounciness = 1f;

    [Header("氷の地面を歩いたときの滑り度")]
    [SerializeField] [Range(0f,1.0f)] private float _slipperiness = 0.97f;
    private bool _isIceGround = false;

    //地面に触れているか
    private bool _isGround = true;
    //今ジャンプをためているか
    private bool _isChargeJump = false;
    // 地面と判定するレイヤー
    private LayerMask _groundLayer;
    //前のフレームでスペースキーを押しているか
    private bool _isPressdSpaceKeyBeforeFlame = false;
    //
    private float _lastGroundY;
    //
    private bool _isFallen = false;
    [SerializeField] private float _fallDistance = 6.0f;
    //ジャンプ飛距離上昇のアイテムが二段ジャンプに影響を与えるか
    [Header("アイテム")]
    [SerializeField] private bool _isJumpPowerUpEffectOnDoubleJump = false;
    [SerializeField] private float _doubleJumpPower = 10f;
    private PlayerItemSystem _playerItemSystem;
    private bool _isPressdDoubleJump = false;

    private SpriteRenderer _sr;    
    private Rigidbody2D _rb;

    public Action OnJump;
    public Action OnFall;
    void Start()
    {
        Application.targetFrameRate = 60;
        _rb = GetComponent<Rigidbody2D>();
        _playerItemSystem = GetComponent<PlayerItemSystem>();
        _sr = GetComponent<SpriteRenderer>();
        _groundLayer = LayerMask.NameToLayer("Ground");
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        CheckChargeing(keyboard);
        Move(keyboard);
        SlipIceGround(keyboard);
        ChargeJump(keyboard);
        DoubleJump(keyboard);

        _velocityBeforeFlame = _rb.linearVelocity;
        //落下回数に関する処理
        if(_isGround)
        {
            _lastGroundY = transform.position.y;
            _isFallen = false;
        }
        //落下していないフラグがたっているときかつ地面から指定した分落下しているとき
        if(!_isFallen && transform.position.y < (_lastGroundY - _fallDistance))
        {
            _isFallen = true;
            OnFall?.Invoke();
        }

        //デバッグ用に視覚的に地面にいるかわかりやすくする
        if (_isGround)
        {
            _sr.color = Color.red;
        }
        else
        {
            _sr.color = Color.green;
        }

        if (keyboard.spaceKey.isPressed)
        {
            _isPressdSpaceKeyBeforeFlame = true;
        }
        else
        {
            _isPressdSpaceKeyBeforeFlame = false;
        }
    }

    void Move(Keyboard keyboard)
    {
        if (!_isGround) return;
        //ジャンプをチャージしていないときのみ操作を受け付ける
        if (_isChargeJump) return;

        float x = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            x = -1f;
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            x = 1f;
        }
        if(keyboard.spaceKey.isPressed)
        {
            x = 0f;
        }
        if(!_isIceGround || x !=0)
        {
            _rb.linearVelocity = new Vector2(x * _moveSpeed, _rb.linearVelocity.y);
        }

    }
    void Jump(Keyboard keyboard,float chargePower,float jumpPowerModifier)
    {
        SetJumpChargeX(keyboard);
        var power = chargePower + _minCharge;
        float jumpPower = Mathf.Min(power,_maxCharge) * _jumpPower * jumpPowerModifier * _jumpPowerModifier;
        //Debug.Log($"ジャンプ: {jumpPower}, チャージ力: {chargePower}, ジャンプ力補正{jumpPowerModifier}");
        var temp = chargePower / _maxCharge;
        var angle = Mathf.Lerp(_minJumpAngle, _maxJumpAngle, temp);
        Debug.Log($"power: {power}, angle:{angle}");
        var cos = Mathf.Cos(angle * Mathf.Deg2Rad) * _jumpChargeX;
        var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        Debug.Log($"cos: {cos}, sin: {sin}");
        var targetVec = new Vector2(cos, sin).normalized;
        //Debug.Log($"targetVec: {targetVec}");
        var jumpVec = targetVec * jumpPower;
        _rb.linearVelocity = jumpVec;
        _jumpChargeX = 0f;
        OnJump?.Invoke();
    }
    void CheckChargeing(Keyboard keyboard)
    {
        if(_isGround)
        {
            if (keyboard.spaceKey.isPressed)
            {
                _chargePower ++;
                _chargePower = Mathf.Clamp(_chargePower, 0, _maxCharge);
                _isChargeJump = true;
                return;
            }
        }
        _isChargeJump = false;
    }
    void ChargeJump(Keyboard keyboard)　　　　　　　　 
    {
        //地面に触れているとき
        if (_isGround)
        {
            //ジャンプキーを離したときまたはチャージ値が最大チャージ値を超えたときにジャンプする
            if (keyboard.spaceKey.wasReleasedThisFrame || (keyboard.spaceKey.isPressed && _chargePower >= _maxCharge))
            {
                SetJumpChargeX(keyboard);
                //ジャンプ力上昇アイテムの処理 所持しているなら使用して補正をかける
                float jumpPowerModifier = 1f;
                var jumpPowerUp = _playerItemSystem.CheckItem<Item_JumpPowerup>();
                jumpPowerUp?.Use();
                jumpPowerModifier = jumpPowerUp == null ? 1 : jumpPowerUp.m_jumpPower;

                Jump(keyboard, _chargePower,jumpPowerModifier);

                _chargePower = 0.0f;
                //ジャンプ後に二段ジャンプのフラグを戻す
                _isPressdDoubleJump = false;
            }
        }
    }

    void SetJumpChargeX(Keyboard keyboard)
    {
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            _jumpChargeX = -1f;
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            _jumpChargeX = 1f;
        }
    }
    //地面を滑る処理
    void SlipIceGround(Keyboard keyboard)
    {
        //キー入力をしていないとき
        if (!(keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ||
                    keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed))
        {
            //かつ　氷の地面に立っているとき
            if (_isIceGround)
            {
                //Debug.Log($"滑っているよ{_rb.linearVelocity}");
                var playerVelocityOnSlip = _velocityBeforeFlame;
                // 現在の速度にslipperinessを掛け算して、じわじわとしか減速させない
                playerVelocityOnSlip.x *= _slipperiness;

                // 速度を更新（ジャンプ力などのY軸の速度は変えない）
                _rb.linearVelocity = playerVelocityOnSlip;
            }
        }
    }
    //二段ジャンプの処理
    void DoubleJump(Keyboard keyboard)
    {
        //地面に触れていないとき
        if (!_isGround)
        {
            //既に二段ジャンプをしているときは二段ジャンプできないようにする
            if (keyboard.spaceKey.isPressed && !_isPressdDoubleJump)
            {
                //前のフレームでスペースキーが押されてないとき
                if (!_isPressdSpaceKeyBeforeFlame)
                {
                    //二段ジャンプアイテムを持っているとき
                    if (_playerItemSystem.CheckItem<Item_DoubleJump>() is var doubleJump && doubleJump != null)
                    {
                        float jumpPowerModifier = 1f;
                        //ジャンプ飛距離上昇のアイテムが二段ジャンプに影響を与えるとき
                        if (_isJumpPowerUpEffectOnDoubleJump)
                        {
                            var jumpPowerUp = _playerItemSystem.CheckItem<Item_JumpPowerup>();
                            jumpPowerUp?.Use();
                            jumpPowerModifier = jumpPowerUp == null ? 1 : jumpPowerUp.m_jumpPower ;
                        }
                        doubleJump.Use();
                        Jump(keyboard,_doubleJumpPower, jumpPowerModifier);

                        _isPressdDoubleJump = true;
                    }
                    
                }
            }
        }
    }

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
}
