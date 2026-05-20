using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainUIDirector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_timerText;
    private ScoreTime m_scoreTime = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public ScoreTime GetScoreTime()
    {
        return m_scoreTime;
    }
    // Update is called once per frame
    void Update()
    {
        m_timerText.SetText(m_scoreTime.ToString());
    }

}
