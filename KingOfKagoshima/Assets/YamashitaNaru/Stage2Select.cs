using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Stage2Selection : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.01f;
    [SerializeField] private PlayerInputSystem inputSystem;
    private readonly Vector2 kSelectPos = new Vector2(1000, 0);
    private readonly Vector2 kDontSelectPos = new Vector2(0, 0);

    bool m_isSelect = false;

    private void Start()
    {
        transform.localPosition = kSelectPos;
    }

    void FixedUpdate()
    {
        // Enterキーが押された瞬間
        if (inputSystem.IsPressedRightKey())
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = false;

        }
        if (inputSystem.IsPressedLeftKey())
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = true;
        }

        if (m_isSelect)
        {
            Debug.Log("選択中");
            transform.localPosition = Vector2.Lerp(transform.localPosition, kSelectPos, speed);
        }
        else
        {
            Debug.Log("選択中じゃない");
            transform.localPosition = Vector2.Lerp(transform.localPosition, kDontSelectPos, speed);
        }
        if (inputSystem.IsPressedThisFlameJumpKey() && m_isSelect)
        {

            SceneManager.LoadScene("SteageScene2tamari 1");
        }
    }
}