using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Attribute;
[RequireComponent(typeof(PlayerItemSystem))]
[RequireComponent(typeof(PlayerStateManager))]

public class PlayerController : MonoBehaviour
{
    //<SerializeField>
    [Header("プレイヤーのステータス")]
    //移動速度
    [SerializeField] private float _moveSpeed = 5f;
    //ジャンプの最少溜め量
    [SerializeField] private float _minCharge = 10f;
    //ジャンプの最大溜め量
    [SerializeField] private float _maxCharge = 20f;
    //ジャンプ力
    [SerializeField] private float _jumpPower = 1.2f;
    //ジャンプ時の最小角度
    [SerializeField] private float _minJumpAngle = 30f;
    //ジャンプ時の最大角度
    [SerializeField] private float _maxJumpAngle = 70f;
    //受け取ったプレイヤーの入力
    [SerializeField, ReadOnly] private float _moveInput = 0.0f;
    //ジャンプ溜め時の力
    [SerializeField,ReadOnly] private float _chargePower = 0.0f;
    //ジャンプ時のX方向の入力
    [SerializeField,ReadOnly]private float _jumpChargeX = 0.0f;

    [Header("壁にぶつかったときの反射")]
    //壁反射時の反発係数（1.0で勢いを維持、0.8などで少し減速）
    [SerializeField] private float _bounciness = 1f;

    [Header("氷の地面を歩いたときの滑り度")]
    [SerializeField][Range(0f, 1.0f)] private float _slipperiness = 0.97f;

    [Header("アイテム")]
    //ジャンプ飛距離上昇のアイテムが二段ジャンプに影響を与えるか 
    [SerializeField] private float _doubleJumpPower = 10f;

    [Header("落下判定を出す高さ")]
    //落下判定を出す高さ
    [SerializeField] private float _fallDistance = 6.0f;

    //<フラグ>
    [Header("フラグ")]
    //ジャンプしていいか
    [SerializeField, ReadOnly] private bool _isEnableJump = true;
    //地面に触れているか
    [SerializeField, ReadOnly] private bool _isGround = true;
    //氷の地面にたっているか
    [SerializeField, ReadOnly] private bool _isIceGround = false;
    //今ジャンプをためているか
    [SerializeField, ReadOnly] private bool _isJumpPressed = false;
    //このフレームでジャンプするか
    [SerializeField, ReadOnly] private bool _isJump = false;
    //今回の落下をすでに記録したか
    [SerializeField, ReadOnly] private bool _isFallenSaved = false;
    //二段ジャンプが許可されているか(アイテムの所持状況は考慮しない)
    [SerializeField, ReadOnly] private bool _isEnableDoubleJump = false;

    [ReadOnly] public bool IsEnableInput { get; set; } = true;

    //<コンポーネントの変数>
    private PlayerItemSystem _itemSystem;
    /// <summary>
    /// プレイヤーのステートを管理するクラス。
    /// ステートに応じてアニメーションも変更してくれる。
    /// </summary>
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

    //1フレーム前の速度
    private Vector2 _velocityBeforeFrame = Vector2.zero;

    //インスペクタでの変更の値をわかりやすくするために補正する定数
    private const float JumpPowerModifier = 0.20f;

    //その方向を向いていると判定する速度のしきい値
    private const float VelocityThreshold = 0.1f;

    private List<Vector2> _normalVectors;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _itemSystem = GetComponent<PlayerItemSystem>();
        _stateManager = GetComponent<PlayerStateManager>();
        _groundLayer = LayerMask.NameToLayer("Ground");
        //プレイヤーの状態を設定する
        _stateManager.CurrentState = PlayerState.Idle;
        _normalVectors = new List<Vector2>();
    }

    /// <summary>
    /// フラグの管理と、ステートの更新をする。
    /// </summary>
    private void Update()
    {

        CheckChargeing();
        ChargeJump();
        Move();

        //<落下回数に関する処理>
        //地面の上に立っているときに現在の座標を記録しておく。
        if (_isGround)
        {
            _lastGroundY = transform.position.y;
            _isFallenSaved = false;
        }
        //まだ落下を記録していないときで、規定の距離以上落下したとき。
        if (!_isFallenSaved && transform.position.y < (_lastGroundY - _fallDistance))
        {
            _isFallenSaved = true;
            //落下イベントの実行(別クラスで数値の保存をする)
            OnFall?.Invoke();
        }

        //<二段ジャンプフラグの更新>
        //二段ジャンプフラグがfalseになっているときは、再び地面についてからフラグをtrueにする。
        if (!_isEnableDoubleJump)
        {
            if (_isGround) _isEnableDoubleJump = true;
        }


        //<プレイヤーの状態の処理>
        //いずれかの移動キーが押されているかを取得する。
        bool isPressedMoveKey = false;

        if ((IsEnableInput))
        {
            isPressedMoveKey = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed
            || Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
        }
        

        //ジャンプチャージ中
        //ジャンプチャージ中のフラグがtrueのときで地面に足がついているとき
        if (_isJumpPressed && _isGround && _isEnableJump)
        {
            _stateManager.CurrentState = PlayerState.JumpCharge;
        }

        //ジャンプ中
        //速度ベクトルが上方向で地面についていないとき
        else if (_rb.linearVelocityY > 0 && !_isGround)
        {
            _stateManager.CurrentState = PlayerState.Jump;
        }

        //落下中
        //速度ベクトルが下方向で地面についていないとき
        else if (_rb.linearVelocityY <= 0 && !_isGround)
        {
            _stateManager.CurrentState = PlayerState.Fall;
        }

        //移動中
        //移動キーをおしているとき で 地面についているとき で 氷の地面についていないとき
        else if (isPressedMoveKey && _isEnableJump && _isGround && !_isIceGround)
        {
            _stateManager.CurrentState = PlayerState.Move;
        }

        //待機中
        //速度ベクトルが0 か 移動キーを押下してないとき で地面についているとき
        else if ((_rb.linearVelocityX == 0 || !isPressedMoveKey) && _isGround)
        {
            _stateManager.CurrentState = PlayerState.Idle;
        }

        //<移動方向に回転>
        //壁にあたっているなら回転処理を実行しない(プレイヤーのガタツキ防止のため)
        if (HasNormalVector(new(1.0f, 0.0f)) || HasNormalVector(new(-1.0f, 0.0f))) return;

        //X方向の速度が0のときは向きを変更しない。
        if (_rb.linearVelocityX > VelocityThreshold || (_isJumpPressed && _jumpChargeX == 1))
        {
            transform.rotation = Quaternion.identity;
        }
        if (_rb.linearVelocityX < -VelocityThreshold || (_isJumpPressed && _jumpChargeX == -1))
        {
            transform.rotation = Quaternion.Euler(new(0, 180, 0));
        }
    }

    /// <summary>
    /// フラグの状態から変更を加える。
    /// </summary>
    void FixedUpdate()
    {
        if (_isGround)
        {
            //ジャンプため処理
            if (_isJumpPressed && _isEnableJump)
            {
                _chargePower++;
                _chargePower = Mathf.Clamp(_chargePower, _minCharge, _maxCharge);
                _isJumpPressed = false;
            }
            //ジャンプ処理
            else if (_isJump && _isEnableJump)
            {
                Jump();
            }
            else
            {

                //CheckChargeing();
                //移動処理
                _rb.linearVelocityX = _moveInput * _moveSpeed;
                _moveInput = 0.0f;
            }
        }
        //地面の上に立っていないとき
        else
        {
            _chargePower = 0.0f;

            //二段ジャンプが許可されているときに二段ジャンプ関数を実行する。
            if (_isEnableDoubleJump) DoubleJump();
        }

        SlipIceGround();

        //前のフレームでの速度を保存しておく
        _velocityBeforeFrame = _rb.linearVelocity;
    }

    /// <summary>
    /// ジャンプが溜められているかを判定する関数。
    /// </summary>
    void CheckChargeing()
    {
        if (!IsEnableInput) return;

        if (Keyboard.current.spaceKey.isPressed)
        {
            if (!_isGround) return;
            if (_chargePower >= _maxCharge) return;
            //ジャンプ方向入力の受付
            SetJumpChargeX();
            _isJumpPressed = true;
            _moveInput = 0.0f;
        }
    }

    /// <summary>
    /// プレイヤーの入力に応じて移動入力を取得する。
    /// </summary>
    void Move()
    {
        _moveInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) _moveInput = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) _moveInput = 1f;

        if (!IsEnableInput) _moveInput = 0.0f;

        //ジャンプため中は入力無効
        if (_isJumpPressed) _moveInput = 0;
    }
    /// <summary>
    /// 溜められた力やアイテム状況から実際にプレイヤーの速度を変更してジャンプをする。
    /// </summary>
    void Jump()
    {
        //ジャンプ方向入力の受付
        SetJumpChargeX();

        //ジャンプ力上昇アイテムの処理 所持しているなら使用して補正をかける
        float jumpPowerModifier = Item_JumpPowerup.UseItem(_itemSystem);

        //ジャンプ力の計算
        float power = _chargePower + _minCharge;
        float jumpPower = Mathf.Min(power, _maxCharge) * _jumpPower * jumpPowerModifier * JumpPowerModifier;

        //ジャンプ角度の計算(溜めた割合に応じてジャンプ角度が上昇する)
        //線形補間で角度を出す。
        var temp = _chargePower / _maxCharge;
        var angle = Mathf.Lerp(_minJumpAngle, _maxJumpAngle, temp);

        //正規化されたジャンプ方向ベクトルを出す。
        var cos = Mathf.Cos(angle * Mathf.Deg2Rad) * _jumpChargeX;
        var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        var targetVec = new Vector2(cos, sin).normalized;

        //Debug.Log($"angle: {angle}, cos: {cos}, sin: {sin}, targetVec: {targetVec}, jumpChargeX: {_jumpChargeX}");

        //ジャンプ力と乗算してベクトルをだす。
        var jumpVec = targetVec * jumpPower;

        Debug.DrawLine(transform.position, jumpVec, new(255, 0, 0), 3);

        _rb.linearVelocity += jumpVec;

        //ジャンプフラグをリセットする。
        _isJump = false;
        _isEnableDoubleJump = true;

        //ジャンプ関連のメンバ変数をリセットする。
        _chargePower = 0f;
        _jumpChargeX = 0f;

        //ジャンプイベントの発火(他クラスで回数を保存する)
        OnJump?.Invoke();
    }

    /// <summary>
    /// 溜めている状態からどの状態になったらジャンプするかを判定する。
    /// </summary>
    void ChargeJump()
    {
        if (!IsEnableInput) return;

        //ジャンプできないとき処理をしない
        if (!_isEnableJump) return;

        //ジャンプキーを離したときまたはチャージ値が最大チャージ値を超えたときにジャンプする
        if (Keyboard.current.spaceKey.wasReleasedThisFrame || _chargePower >= _maxCharge)
        {
            if (!_isGround) return;
            _isJump = true;
            _moveInput = 0.0f;
        }
    }
    /// <summary>
    /// ジャンプため中にジャンプの向きの入力を受け付ける関数。
    /// </summary>
    void SetJumpChargeX()
    {
        if (!IsEnableInput) return;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) _jumpChargeX = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) _jumpChargeX = 1f;

    }

    /// <summary>
    /// 氷の地面を歩いたときに滑るようにする。
    /// </summary>
    void SlipIceGround()
    {
        if (!_isGround || !_isIceGround) return;

        //キー入力をしていないとき
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed
            || Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) return;

        var playerVelocityOnSlip = _velocityBeforeFrame;
        // 現在の速度にslipperinessを掛け算して、じわじわとしか減速させない
        playerVelocityOnSlip.x *= _slipperiness;

        // 速度を更新（ジャンプ力などのY軸の速度は変えない）
        _rb.linearVelocity = playerVelocityOnSlip;

    }

    /// <summary>
    /// フラグとアイテム所持状況をみてJump関数を実行する関数
    /// </summary>
    void DoubleJump()
    {
        if (!_isEnableDoubleJump) return;
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {

            //二段ジャンプアイテムを持っているとき
            if (Item_DoubleJump.UseItem(_itemSystem))
            {
                _chargePower = _doubleJumpPower;
                Jump();
                _isEnableDoubleJump = false;
            }
        }
    }

    bool IsInLangeY(float min, float max)
    {
        foreach (var vector in _normalVectors)
        {
            //ほぼ一緒なら一緒とみなす
            if (min < vector.y && vector.y < max) return true;
        }
        return false;
    }

    /// <summary>
    /// 指定したベクトルを法線ベクトルとして持っているならtrueを返す
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="vec"></param>
    /// <returns></returns>
    bool HasNormalVector(Vector2 vec)
    {
#if false
        var log = "\n";
        foreach(var contact in collision.contacts)
        {
            log += $"法線: {contact.normal}\n";
        }
        Debug.Log(log);
#endif
        foreach (var vector in _normalVectors)
        {
            //ほぼ一緒なら一緒とみなす
            var diff = vector - vec;
            diff.x = MathF.Abs(diff.x);
            diff.y = MathF.Abs(diff.y);
            if (diff.x < 0.01f && diff.y < 0.01f) return true;
        }
        return false;
    }
    void CheckIsGround(Collision2D collision)
    {
        if (collision.gameObject.layer == _groundLayer)
        {
            //今回の法線ベクトルをメンバ変数に保存。
            _normalVectors.Clear();
            foreach(var contact in collision.contacts)
            {
                _normalVectors.Add(contact.normal);
            }

            if (IsInLangeY(0.5f, 1.5f))
            {
                _isGround = true;
                if (collision.gameObject.CompareTag("IceGround"))
                {
                    _isIceGround = true;
                }
            }
        }
    }

    public float GetChargeRatio()
    {
        return (_chargePower - _minCharge) / (_maxCharge - _minCharge);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckIsGround(collision);

        var normalY = collision.contacts[0].normal.y;
        var normalX = collision.contacts[0].normal.x;
        //30~80度の時、-80~-30度は滑るようにする
        var rad = MathF.Atan(normalY / normalX);
        var degree = rad * Mathf.Rad2Deg;
        Debug.Log(degree);

        bool leftTopToRightBottom = degree > 30 && degree < 80;
        bool rightTopToleftBottom = degree > -80 && degree < -30;

        if (leftTopToRightBottom || rightTopToleftBottom)
        {
            var pos = transform.position;
            pos.y -= 0.1f;
            transform.position = pos;
            _isEnableJump = false;
        }
        else _isEnableJump = true;
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
