using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class StageSelect : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _playerToCamera;
    [SerializeField] private List <TextMeshPro> _textList;
    //カーソル選択肢
    private int _index = 0;
    private Transform _selectedPos;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    private int _textIndex = 0;

    private void SetIndex(int value)
    {
        if (value > _textList.Count - 1) _index = _textList.Count - 1;
        else if (value < 0) _index = 0;
        else _index = value;
    }
    // Update is called once per frame
    // FixedUpdate から Update に変更
    void Update()
    {
        int temp = _index;

        // --- 左右の移動（0↔1, 2↔3） ---
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            if (temp != 1 && temp != 3) temp += 1; // 右端でなければ右へ
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
        {
            if (temp != 0 && temp != 2) temp -= 1; // 左端でなければ左へ
        }

        // --- 上下の移動（0↔2, 1↔3） ---
        if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
        {
            Debug.Log(Keyboard.current.downArrowKey.wasPressedThisFrame);
              

            if (temp == 0 || temp == 1) temp += 2; // 上段なら下へ (+2)
        }
        if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
        {
            if (temp == 2 || temp == 3) temp -= 2; // 下段なら上へ (-2)
        }

        // 選択が変わったときだけインデックスと色を更新
        // 選択が変わったときだけインデックスと色を更新
        if (temp != _index)
        {
            _index = temp; // インデックスを更新
             UpdateMenuColors(); // 下で作る色変え関数を呼び出す
        }
    }
    private void UpdateMenuColors()
    {
        for (int i = 0; i < _textList.Count; i++)
        {
            if (_textList[i] == null) continue;

            TextMeshPro text = _textList[i];
            if (text == null) continue;

            if (i == _index)
            {
                text.color = selectedColor;
            }
            else
            {
                text.color = normalColor;
            }
        }
    }


  
    void UpdateMenuColors(List<TextMeshPro> texts)
    {
        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i] == null) continue;

            if (i == _textIndex)
            {
                texts[i].color = selectedColor;
            }
            else
            {
                texts[i].color = normalColor;
            }
        }
    }
}
