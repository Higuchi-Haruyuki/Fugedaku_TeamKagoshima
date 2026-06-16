using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageDebug : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Vector3 pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pos = _player.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.leftShiftKey.isPressed)
        {
            _player.position = pos;
        }
    }
}
