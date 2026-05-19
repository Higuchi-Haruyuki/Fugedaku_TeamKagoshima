using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Stage2Selection : MonoBehaviour
{
    void Update()
    {
        // Enterキーが押された瞬間
        if (Keyboard.current.digit2Key.isPressed)
        {
            // StageSelectシーンへ移動
            SceneManager.LoadScene("StageSelect");
        }
    }
}