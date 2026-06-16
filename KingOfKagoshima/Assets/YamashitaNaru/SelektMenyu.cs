using UnityEngine;
using TMPro; 

public class TextMenu : MonoBehaviour
{
    
    public TextMeshProUGUI[] menuTexts;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;
    private PlayerInputSystem inputSystem;
    void Start()
    {
        UpdateMenuColors();
        inputSystem = GameObject.FindGameObjectWithTag("PlayerInputSystem")?.GetComponent<PlayerInputSystem>();
    }

    void Update()
    {
        if (inputSystem.IsPressedUpKey())
        {
            selectedIndex = (selectedIndex + 1) % menuTexts.Length;
            UpdateMenuColors();
        }
        else if (inputSystem.IsPressedDownKey())
        {
            selectedIndex = (selectedIndex - 1 + menuTexts.Length) % menuTexts.Length;
            UpdateMenuColors();
        }
    }

    void UpdateMenuColors()
    {
        for (int i = 0; i < menuTexts.Length; i++)
        {
            if (i == selectedIndex)
            {
                menuTexts[i].color = selectedColor;
            }
            else
            {
                menuTexts[i].color = normalColor;
            }
        }
    }
}