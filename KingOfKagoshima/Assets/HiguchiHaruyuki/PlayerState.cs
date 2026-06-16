using UnityEngine;
public enum PlayerState
{
    Idle,Move,JumpCharge,Jump,Fall
}

public class PlayerStateManager : MonoBehaviour
{
    private static readonly int PlayerIdleHash = Animator.StringToHash("Player_Idle");
    private static readonly int PlayerMoveHash = Animator.StringToHash("Player_Move");
    private static readonly int PlayerJumpChargeHash = Animator.StringToHash("Player_JumpCharge");
    private static readonly int PlayerJumpHash = Animator.StringToHash("Player_Jump");
    private static readonly int PlayerFallHash = Animator.StringToHash("Player_Fall");
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public PlayerState CurrentState 
    {
        get 
        { 
            return _currentState;
        }
        set
        {
            //プレイヤーの状態が変化したとき
            if (value != _currentState)
            {
                Debug.Log($"Player State Changed: {_currentState} -> {value}");
                _currentState = value;
                PlayAnimation(_currentState);
            }
        }
    }
    private PlayerState _currentState;
    private void PlayAnimation(PlayerState state)
    {
        if (!_animator) return;
        switch (state)
        {
            case PlayerState.Idle:
                _animator.Play(PlayerIdleHash);
                break;
            case PlayerState.Move:
                _animator.Play(PlayerMoveHash);
                break;
            case PlayerState.JumpCharge:
                _animator.Play(PlayerJumpChargeHash);
                break;
            case PlayerState.Jump:
                _animator.Play(PlayerJumpHash);
                break;
            case PlayerState.Fall:
                _animator.Play(PlayerFallHash);
                break;
            default:
                _animator.Play(PlayerIdleHash);
                break;
        }
    }

}
