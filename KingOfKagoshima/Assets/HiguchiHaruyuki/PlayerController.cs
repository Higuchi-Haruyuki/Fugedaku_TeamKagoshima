using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Attribute;
[RequireComponent(typeof(PlayerItemSystem))]
[RequireComponent(typeof(PlayerStateManager))]

public class PlayerController : MonoBehaviour
{
    //<SerializeField>
    [SerializeField]private PlayerInputSystem _inputSystem;
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
    [Header("フラグ")]
    //二段ジャンプが許可されているか(アイテムの所持状況は考慮しない)
    [SerializeField, ReadOnly] private bool _isEnableDoubleJump = false;
    //地面に触れているか
    [SerializeField, ReadOnly] private bool _isGround = true;
    //今ジャンプをためているか
    [SerializeField, ReadOnly] private bool _isJumpCharge = false;
    //今落下中か
    [SerializeField, ReadOnly]private bool _isFallen = false;
    //氷の地面にたっているか
    [SerializeField, ReadOnly] private bool _isIceGround = false;
    //このフレームでジャンプするか
    [SerializeField, ReadOnly] private bool _isJump = false;

    //<コンポーネントの変数>
    private PlayerItemSystem _itemSystem;
    private PlayerStateManager _stateManager;
    private Rigidbody2D _rb;

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
    Vector2 _velocityBeforeFrame = Vector2.zero;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _itemSystem = GetComponent<PlayerItemSystem>();
        _stateManager = GetComponent<PlayerStateManager>();
        _groundLayer = LayerMask.NameToLayer("Ground");
        //プレイヤーの状態を設定する
        _stateManager.CurrentState = PlayerState.Idle;
    }

    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Move();
        CheckChargeing();
        ChargeJump();
        SlipIceGround();
        //地面の上に立っているとき
        if(_isGround)
        {
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
        _velocityBeforeFrame = _rb.linearVelocity;

        //プレイヤーの状態の処理
        //ジャンプチャージ中
        //ジャンプチャージ中のフラグがtrueのときで地面に足がついているとき
        if(_isJumpCharge && _isGround)
        {
            _stateManager.CurrentState = PlayerState.JumpCharge;
        }
        //ジャンプ中
        //速度ベクトルが上方向で地面についていないとき
        else if(_rb.linearVelocityY > 0 && !_isGround)
        {
            _stateManager.CurrentState = PlayerState.Jump;
        }
        //落下中
        //速度ベクトルが下方向で地面についていないとき
        else if ( _rb.linearVelocityY <= 0 && !_isGround)
        {
            _stateManager.CurrentState = PlayerState.Fall;
        }
        //移動中
        //移動キーをおしているとき で 地面についているとき で 氷の地面についていないとき
        else if(_inputSystem.IsPressedMoveKey() && _isGround)
        {
            _stateManager.CurrentState = PlayerState.Move;
        }
        //待機中
        //速度ベクトルが0 か 移動キーを押下してないとき で地面についているとき
        else if(( _rb.linearVelocityX == 0 || !_inputSystem.IsPressedMoveKey()) && _isGround)
        {
            _stateManager.CurrentState = PlayerState.Idle;
        }

        //移動方向に回転させてみる
        if (_rb.linearVelocityX > 0)
        {
            transform.rotation = Quaternion.identity;
        }
        if( _rb.linearVelocityX < 0)
        {
            transform.rotation = Quaternion.Euler(new(0, 180, 0));
        }

    }

    void CheckChargeing()
    {
        if (_inputSystem.IsPressedJumpKey())
        {
            if (!_isGround) return;

            //ジャンプ力をためる処理
            _chargePower++;
            _chargePower = Mathf.Clamp(_chargePower, 0, _maxCharge);
            _isJumpCharge = true;
            return;
        }
        _isJumpCharge = false;
    }

    void Move()
    {

        if (!_isGround) return;
        if (_isJump) return;

        float x = 0f;

        if (_inputSystem.IsPressedLeftKey()) x = -1;
        if (_inputSystem.IsPressedRightKey()) x = 1f;

        //氷の地面に入力なしでたっているときは処理をしない
        if (_isIceGround && x == 0) return;

        //ジャンプため中は入力無効
        if(_isJumpCharge) x = 0;

        _rb.linearVelocity = new Vector2(x * _moveSpeed, _rb.linearVelocity.y);
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

        //Debug.Log($"angle: {angle}, targetVec: {targetVec}, jumpChargeX: {_jumpChargeX}");
        //ジャンプ力と乗算してベクトルをだす
        var jumpVec = targetVec * jumpPower;

        _rb.linearVelocity = jumpVec;

        _isJump = true;
        _isGround = false;

        _jumpChargeX = 0f;
        OnJump?.Invoke();
    }

    void ChargeJump()
    {
        //ジャンプキーを離したときまたはチャージ値が最大チャージ値を超えたときにジャンプする
        if (_inputSystem.IsReleasedJumpKey() || _chargePower >= _maxCharge)
        {
            if (!_isGround) return;

            SetJumpChargeX();
            //ジャンプ力上昇アイテムの処理 所持しているなら使用して補正をかける
            float jumpPowerModifier = 1f;
            jumpPowerModifier = Item_JumpPowerup.UseItem(_itemSystem);

            Jump(_chargePower, jumpPowerModifier);

            _chargePower = 0.0f;
            //ジャンプ後に二段ジャンプのフラグを戻す
            _isEnableDoubleJump = true;
        }
        else
        {
            _isJump = false;
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
        if (!_isGround || !_isIceGround) return;
        //キー入力をしていないとき
        if (!_inputSystem.IsPressedMoveKey())
        {
            var playerVelocityOnSlip = _velocityBeforeFrame;
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
    /// 指定したベクトルを法線ベクトルとして持っているならtrueを返す
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="vec"></param>
    /// <returns></returns>
    bool HasNormalVector(Collision2D collision,Vector2 vec)
    {
#if false
        var log = "\n";
        foreach(var contact in collision.contacts)
        {
            log += $"法線: {contact.normal}\n";
        }
        Debug.Log(log);
#endif
        foreach (var contact in collision.contacts)
        {
            //ほぼ一緒なら一緒とみなす
            var diff = contact.normal - vec;
            if (diff.x < 0.01f && diff.y < 0.01f) return true;
        }
        return false;
    }    
    void CheckIsGround(Collision2D collision)
    {
        if (collision.gameObject.layer == _groundLayer)
        {
            if (HasNormalVector(collision, new(0, 1.00f)))
            {
                _isGround = true;
                if (collision.gameObject.CompareTag("IceGround"))
                {
                    _isIceGround = true;
                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckIsGround(collision);
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
    private void OnCollisionEnter2D(Collision2D collision)
    {   
        // もし衝突した点（contacts）が1つも無ければ、処理をスキップする
        if (collision.contacts == null || collision.contacts.Length == 0) return;

        CheckIsGround(collision);

        ContactPoint2D contact = collision.contacts[0];
        Vector2 wallNormal = contact.normal;

        // 天井や床（上向きの面）は除外する
        if (wallNormal.y > 0.7f) return;

        Vector2 reflectDir = Vector2.Reflect(_velocityBeforeFrame, wallNormal);
        _rb.linearVelocity = reflectDir * _bounciness;
    }
}
