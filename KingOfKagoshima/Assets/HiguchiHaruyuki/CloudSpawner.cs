using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{

    [SerializeField] private List<GameObject> _clouds;

    [SerializeField] private Vector3 _leftUp;
    [SerializeField] private Vector3 _rightBottom;

    [SerializeField] private float _spawnRate = 0.01f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int x = 0; x < _rightBottom.x - _leftUp.x; x++)
        {
            for (int y = 0; y < _leftUp.y - _rightBottom.y; y++)
            {
                var a = Random.Range(0.0f, 1.0f);

                if(a < _spawnRate)
                    Instantiate(_clouds[Random.Range(0, _clouds.Count)],new Vector3(_leftUp.x + x, _rightBottom.y + y,0.0f),new Quaternion(),transform);
            }
        }
    }

}
