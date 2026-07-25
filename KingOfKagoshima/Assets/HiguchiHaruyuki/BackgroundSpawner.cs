using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BackgroundSpawner : MonoBehaviour
{
    [SerializeField] private float _backgroundImageOffset = 10.8f;

    [SerializeField] private int _backgroundImageCount = 20;

    [SerializeField] private List<GameObject> _backgroundPrefab;

    private int _beforeRandomValue = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < _backgroundImageCount; i++)
        {
            Debug.Log($"index: {i}, spawnPos: {i * _backgroundImageOffset}");
            Instantiate(_backgroundPrefab[GetRandomValue()], new Vector3(0,i * _backgroundImageOffset,0),new Quaternion(),transform);
        }
    }

    int GetRandomValue()
    {
        int value = Random.Range(0, _backgroundPrefab.Count);
        if (value == _beforeRandomValue) return GetRandomValue();
        _beforeRandomValue = value;
        return value;
    }
}
