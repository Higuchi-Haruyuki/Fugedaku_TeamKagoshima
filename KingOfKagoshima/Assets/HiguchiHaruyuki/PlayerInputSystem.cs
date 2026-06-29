using UnityEngine;
using UnityEngine.InputSystem;
[DefaultExecutionOrder(-100)]
public class PlayerInputSystem : MonoBehaviour
{
    //以前のフレームでスペースキーが押されているか
    private bool isPressedSpaceKeyLastFrame = false;
    //以前のフレームでエスケープキーが押されているか
    private bool isPressedEscapeKeyLastFrame = false;
    //以前のフレームで上入力が押されているか
    private bool isPressedUpKeyLastFrame = false;
    //以前のフレームで下入力が押されているか
    private bool isPressedDownKeyLastFrame = false;
    //以前のフレームで右入力が押されているか
    private bool isPressedRightKeyLastFrame = false;
    //以前のフレームで左入力がされているか
    private bool isPressedLeftKeyLastFrame = false;
    /// <summary>
    /// 左入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedLeftKey()
    {
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// 左入力をやめたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsReleasedLeftKey()
    {
        if (Keyboard.current.aKey.wasReleasedThisFrame || Keyboard.current.leftArrowKey.wasReleasedThisFrame) return true;
        return false;
    }
    /// <summary>
    /// 左入力がされ始めたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedThisFrameLeftKey() => IsPressedThisFrameKey(Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed, ref isPressedLeftKeyLastFrame);

    /// <summary>
    /// 右入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedRightKey()
    {
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// 右入力をやめたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsReleasedRightKey()
    {
        if (Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.rightArrowKey.wasReleasedThisFrame) return true;
        return false;
    }
    /// <summary>
    /// 右入力がされ始めたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedThisFrameRightKey() => IsPressedThisFrameKey(Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed, ref isPressedRightKeyLastFrame);

    /// <summary>
    /// 上入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedUpKey()
    {
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// 上入力をやめたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsReleasedUpKey()
    {
        if (Keyboard.current.wKey.wasReleasedThisFrame || Keyboard.current.upArrowKey.wasReleasedThisFrame) return true;
        return false;
    }
    /// <summary>
    /// 上入力がされ始めたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedThisFrameUpKey() => IsPressedThisFrameKey(Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed, ref isPressedUpKeyLastFrame);

    /// <summary>
    /// 下入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedDownKey()
    {
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// 下入力をやめたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsReleasedDownKey()
    {
        if (Keyboard.current.sKey.wasReleasedThisFrame || Keyboard.current.downArrowKey.wasReleasedThisFrame) return true;
        return false;
    }
    /// <summary>
    /// 下入力がされ始めたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedThisFrameDownKey() => IsPressedThisFrameKey(Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed, ref isPressedDownKeyLastFrame);

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
    public bool IsPressedThisFlameJumpKey() => IsPressedThisFrameKey(Keyboard.current.spaceKey.isPressed, ref isPressedSpaceKeyLastFrame);

    /// <summary>
    /// エスケープ入力がされているときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedEscapeKey()
    {
        if (Keyboard.current.escapeKey.isPressed) return true;
        return false;
    }
    /// <summary>
    /// エスケープ入力をやめたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsReleasedEscapeKey()
    {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame) return true;
        return false;
    }
    /// <summary>
    /// エスケープ入力がされ始めたときにtrueを返す関数
    /// </summary>
    /// <returns></returns>
    public bool IsPressedThisFlameEscapeKey() => IsPressedThisFrameKey(Keyboard.current.escapeKey.isPressed, ref isPressedEscapeKeyLastFrame);

    private bool IsPressedThisFrameKey(bool pressKey, ref bool lastFrame)
    {
        if (pressKey)
        {
            if (!lastFrame)
            {
                lastFrame = true;
                return true;
            }
        }
        else
        {
            lastFrame = false;
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
