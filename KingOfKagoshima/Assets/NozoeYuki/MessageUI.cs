using System.Collections;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    public static MessageUI Instance;

    [SerializeField] private TextMeshProUGUI messageText;

    private Coroutine currentCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;

        Color color = messageText.color;
        color.a = 0;
        messageText.color = color;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
