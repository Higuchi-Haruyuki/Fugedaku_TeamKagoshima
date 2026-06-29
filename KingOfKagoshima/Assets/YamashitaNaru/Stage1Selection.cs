using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Stage1Selection : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.01f;
    [SerializeField] private PlayerInputSystem inputSystem;
    private readonly Vector2 kSelectPos = new Vector2(0, 0);
    private readonly Vector2 kDontSelectPos = new Vector2(-100000000, 0);

    bool m_isSelect = true;

    private void Start()
    {
        transform.localPosition = kSelectPos;
    }

    void FixedUpdate()
    {
        // Enterキーが押された瞬間
        if (inputSystem.IsPressedLeftKey()) 
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = true;

        }
        if (inputSystem.IsPressedRightKey())
        {
            // StageSelectシーンへ移動
            //SceneManager.LoadScene("StageScene1");

            m_isSelect = false;
        }

        if(m_isSelect)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, kSelectPos, speed);
        }
        else
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, kDontSelectPos, speed);
        }
        if (inputSystem.IsPressedThisFlameJumpKey() && m_isSelect)
        {
            Debug.Log("PressEnter");
            SceneManager.LoadScene("StageScene1");
        }
       


    }
}