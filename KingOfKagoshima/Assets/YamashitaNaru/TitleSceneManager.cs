using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    void Update()
    {
        // Enterキーが押された瞬間
        if (Keyboard.current.enterKey.isPressed)
        {
            // StageSelectシーンへ移動
            SceneManager.LoadScene("StageSelect");
        }
    }
}