using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GoalAnim : MonoBehaviour
{
    [SerializeField] float _rotationRange = 45.0f;
    [SerializeField] float _rotationDuration = 2.0f;
    [SerializeField] float _popY = 10.0f;
    [SerializeField] float _popDuration = 2.0f;

    float _targetAngle = 0.0f;
    float _startAngle = 0.0f;

    Vector3 _startPos;
    Vector3 _endPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startAngle = -_rotationRange;
        _targetAngle = _rotationRange;
        transform.rotation = Quaternion.Euler(0,_startAngle,0);

        _startPos = transform.position;
        _endPos = _startPos + new Vector3(0, _popY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        _endPos = _startPos + new Vector3(0, _popY, 0);

        RotateYAnim();
        Pop();
        
    }

    void RotateYAnim()
    {
        var t = Mathf.PingPong(Time.time / _rotationDuration, 1);

        var angle = Mathf.Lerp(_startAngle, _targetAngle, t);

        transform.rotation = Quaternion.Euler(0, angle, 0);

    }

    void Pop()
    {
        var t = Mathf.PingPong(Time.time / _popDuration, 1);

        var pos = Vector3.Lerp(_startPos, _endPos, t);

        transform.position = pos;
    }
}
