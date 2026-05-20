using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Stage1Selection : MonoBehaviour
{
    void Update()
    {
        // Enterキーが押された瞬間
        if (Keyboard.current.digit1Key.isPressed)
        {
            // StageSelectシーンへ移動
            SceneManager.LoadScene("StageSelect");
        }
    }
}