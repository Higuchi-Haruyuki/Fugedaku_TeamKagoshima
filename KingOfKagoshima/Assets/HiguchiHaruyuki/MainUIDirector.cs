using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainUIDirector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_timerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ScoreTime.AddTime(Time.deltaTime);
        m_timerText.SetText(ScoreTime.ToString());
    }
}
