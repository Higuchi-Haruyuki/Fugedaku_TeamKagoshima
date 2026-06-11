using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Stage1Selection : MonoBehaviour
{
    private readonly Vector2 kSelectPos = new Vector2(0, 0);
    private readonly Vector2 kDontSelectPos = new Vector2(-1000, 0);

    bool m_isSelect = true;

    private void Start()
    {
        transform.localPosition = kSelectPos;
    }

    void Update()
    {
        // Enterキーが押された瞬間
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = true;

        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = false;
        }

        if(m_isSelect)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, kSelectPos, 0.01f);
        }
        else
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, kDontSelectPos, 0.01f);
        }
        if (Keyboard.current.enterKey.wasPressedThisFrame && m_isSelect)
        {
            SceneManager.LoadScene("StageScene1");
        }


    }
}