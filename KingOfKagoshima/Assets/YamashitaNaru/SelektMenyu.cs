using UnityEngine;
using TMPro;

public class TextMenu : MonoBehaviour
{
    public TextMeshProUGUI[] menuTexts;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;
    private PlayerInputSystem inputSystem;

    // 連打防止用のタイマー（inputSystemが"押した瞬間"を検知できない場合の保険）
    private float inputCooldown = 0.2f;
    private float cooldownTimer = 0f;

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
        if (inputSystem == null || menuTexts.Length == 0) return;

        // クールダウンタイマーの更新
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // タイマーが0のときだけ入力を受け付ける
        if (cooldownTimer <= 0)
        {
            // 一般的に「Upキー」はリストを上（インデックスを減らす方向）に進めることが多いです
            if (inputSystem.IsPressedUpKey())
            {
                selectedIndex = (selectedIndex - 1 + menuTexts.Length) % menuTexts.Length;
                UpdateMenuColors();
                cooldownTimer = inputCooldown; // タイマーリセット
            }
            else if (inputSystem.IsPressedDownKey())
            {
                selectedIndex = (selectedIndex + 1) % menuTexts.Length;
                UpdateMenuColors();
                cooldownTimer = inputCooldown; // タイマーリセット
            }
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