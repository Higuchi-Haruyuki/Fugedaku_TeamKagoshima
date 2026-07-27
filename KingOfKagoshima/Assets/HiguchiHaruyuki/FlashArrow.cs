using UnityEngine;
using System.Collections.Generic;

public class FlashArrow : MonoBehaviour
{
    [SerializeField] private float _flashDuration = 2.0f;

    private GameObject[] _chlildren;
    private List<SpriteRenderer> _renderers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _chlildren = new GameObject[transform.childCount];
        _renderers = new List<SpriteRenderer>();
        for (int i = 0; i < transform.childCount; i++)
        {
            _chlildren[i] = transform.GetChild(i).gameObject;
            _renderers.Add(_chlildren[i].GetComponent<SpriteRenderer>());

        }
    }

    // Update is called once per frame
    void Update()
    {
        Flash();
    }

    void Flash()
    {
        float time = Time.time;
        foreach (var renderer in _renderers)
        {
            var t = Mathf.PingPong(time / _flashDuration, 1);

            var alpha = Mathf.Lerp(1.0f, 0.0f, t);

            var color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }



       
    }
}
