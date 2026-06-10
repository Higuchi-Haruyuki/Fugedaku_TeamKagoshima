using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Stage2Selection : MonoBehaviour
{
    private readonly Vector2 kSelectPos = new Vector2(0, 0);
    private readonly Vector2 kDontSelectPos = new Vector2(800, 0);

    bool m_isSelect = false;

    private void Start()
    {
        transform.localPosition = kDontSelectPos;
    }

    void Update()
    {
        // Enterキーが押された瞬間
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = false;

        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = true;
        }

        if (m_isSelect)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, kSelectPos, 0.01f);
        }
        else
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, kDontSelectPos, 0.01f);
        }
    }
}