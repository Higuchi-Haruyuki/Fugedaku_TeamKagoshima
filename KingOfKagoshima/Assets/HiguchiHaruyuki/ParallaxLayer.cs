using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float _parallaxFactor = 0.1f;
    private float _easingFactor = 1.0f;

    private float _initialY = 0.0f;

    public float ParallaxFactor => _parallaxFactor;

    private bool _isEasing = false;

    float _targetY = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _initialY = transform.position.y;
    }


    /// <summary>
    /// カメラの初期位置と現在位置から、目標座標を計算してそこに移動する関数
    /// </summary>
    /// <param name="cameraCurrentY"></param>
    /// <param name="cameraInitialY"></param>
    public void ApplyParallax(float cameraCurrentY, float cameraInitialY)
    {
        var cameraDeltaY = cameraCurrentY - cameraInitialY;

        var targetY = _initialY + cameraDeltaY * _parallaxFactor;

        var pos = transform.position;

        pos.y = targetY;
        transform.position = pos;
    }

    /// <summary>
    /// カメラの初期位置と現在位置から、目標座標を計算してそこに移動する関数
    /// </summary>
    /// <param name="cameraCurrentY"></param>
    /// <param name="cameraInitialY"></param>
    public void ApplyParallaxEasing(float cameraCurrentY, float cameraInitialY)
    {
        var cameraDeltaY = cameraCurrentY - cameraInitialY;

        _targetY = _initialY + cameraDeltaY * _parallaxFactor;
        _isEasing = true;
    }

    private void LateUpdate()
    {
        if (!_isEasing) return;

        var t = 1.0f - Mathf.Exp(_easingFactor);

        var pos = new Vector3(0, (_targetY - transform.position.y) * t, 0);

        transform.position -= pos;

        _isEasing = false;
    }
}
