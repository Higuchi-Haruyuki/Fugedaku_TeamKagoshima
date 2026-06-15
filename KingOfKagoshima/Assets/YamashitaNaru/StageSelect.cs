using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class StageSelect : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _playerToCamera;
    [SerializeField] private List<Transform> _textPos;
    [SerializeField] private PlayerInputSystem _inputSystem;
    private int _index = 0;   // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void SetIndex(int value)
    {
        if (value > _textPos.Count - 1) _index = _textPos.Count - 1;
        else if (value < 0) _index = 0;
        else _index = value;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int temp = _index;
        if (_inputSystem.IsPressedThisFrameRightKey())
        {
            temp++;
        }
        if (_inputSystem.IsPressedThisFrameLeftKey())
        {
            temp--;
        }
        SetIndex(temp);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _textPos[_index].position + _playerToCamera, _speed);
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (_index == 0)
            {
                SceneManager.LoadScene("StageScene1");
            }
            else if (_index > 0)
            {
                SceneManager.LoadScene("SteageScene2tamari 1");
            }
        }
    }
}
