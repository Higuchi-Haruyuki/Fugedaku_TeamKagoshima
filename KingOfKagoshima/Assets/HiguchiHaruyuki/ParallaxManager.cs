using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private List<ParallaxLayer> _layers;


    [SerializeField] private bool _isEasing = false;

    private float _cameraInitialY = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraInitialY = Camera.main.transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var cameraCurrentY = Camera.main.transform.position.y;
        foreach(var layer in _layers)
        {
            if(_isEasing) layer.ApplyParallaxEasing(cameraCurrentY, _cameraInitialY);
            else layer.ApplyParallax(cameraCurrentY, _cameraInitialY);
        }
    }
}
