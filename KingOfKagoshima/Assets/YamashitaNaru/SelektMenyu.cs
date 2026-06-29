using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TextMenu : MonoBehaviour
{
    public TextMeshPro[] menuTexts;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;
    private PlayerInputSystem inputSystem;

    void Start()
    {
        // エラー防止：配列が空なら警告を出す
        if (menuTexts == null || menuTexts.Length == 0)
        {
            Debug.LogError("Menu Texts がインスペクターで設定されていません！");
            return;
        }

        inputSystem = GameObject.FindGameObjectWithTag("PlayerInputSystem")?.GetComponent<PlayerInputSystem>();
        if (inputSystem == null)
        {
            Debug.LogError("PlayerInputSystem が見つかりません！タグを確認してください。");
        }

        UpdateMenuColors();
    }

    void Update()
    {
        Debug.Log("UPDATE!!");
        if (inputSystem == null) return;
        if (menuTexts.Length == 0) { Debug.Log("length == 0"); return; }


        // 一般的に「Upキー」はリストを上（インデックスを減らす方向）に進めることが多いです
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex - 1 + menuTexts.Length) % menuTexts.Length;
            UpdateMenuColors();
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex + 1) % menuTexts.Length;
            UpdateMenuColors();
        }
    }

    void UpdateMenuColors()
    {
        for (int i = 0; i < menuTexts.Length; i++)
        {
            if (menuTexts[i] == null) continue;

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