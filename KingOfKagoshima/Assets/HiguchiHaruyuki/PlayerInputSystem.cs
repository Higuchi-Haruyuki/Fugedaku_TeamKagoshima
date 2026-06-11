using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    //以前のフレームでスペースキーが押されているか
    private bool isPressedSpaceKeyLastFlame = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 左に動く入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedLeftKey()
    {
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// 右に動く入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedRightKey()
    {
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// ジャンプ入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedJumpKey()
    {
        if (Keyboard.current.spaceKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// ジャンプ入力をやめたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsReleasedJumpKey()
    {
        if (Keyboard.current.spaceKey.wasReleasedThisFrame) return true;
        return false;
    }
    /// <summary>
    /// ジャンプ入力がされ始めたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedThisFlameJumpKey()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            if (!isPressedSpaceKeyLastFlame)
            { 
                isPressedSpaceKeyLastFlame= true;
                return true; 
            }
        }
        else
        {
            isPressedSpaceKeyLastFlame = false;
        }
        return false;
    }
    /// <summary>
    /// 移動キーが押されているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedMoveKey()
    {
        return IsPressedLeftKey() || IsPressedRightKey();
    }
}
