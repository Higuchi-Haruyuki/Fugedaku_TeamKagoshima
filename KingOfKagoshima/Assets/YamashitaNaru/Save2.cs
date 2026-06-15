using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Save2 : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.01f;
    private readonly Vector2 kSelectPos = new Vector2(0, 0);
    private readonly Vector2 kDontSelectPos = new Vector2(1000, 0);

    bool m_isSelect = false;

    private void Start()
    {
        transform.localPosition = kDontSelectPos;
    }

    void FixedUpdate()
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
            transform.localPosition = Vector2.Lerp(transform.localPosition, kSelectPos, speed);
        }
        else
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, kDontSelectPos, speed);
        }
        if (Keyboard.current.enterKey.wasPressedThisFrame && m_isSelect)
        {
            // ※もしステージ2用のシーン名があるなら、ここを "StageScene2" などに変更してください
            SceneManager.LoadScene("SteageScene2tamari 1");
        }
    }
}