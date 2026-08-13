using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("跟随目标（玩家）")]
    public Transform target;

    [Header("跟随平滑系数")]
    public float smoothSpeed = 8f;

    [Header("开启上下跟随")]
    public bool followY = true;

    [Header("地图边界 X Y")]
    public float minX = -2f;
    public float maxX = 120f;
    public float minY = -8f;
    public float maxY = 12f;

    private Vector3 _offset;

    void Awake()
    {
        if (target != null)
        {
            _offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + _offset;

        // 控制相机所在范围
        desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
        if (followY)
        {
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
        }

        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;
    }

    /// <summary>复活瞬移时，相机立刻对齐，不做插值</summary>
    public void SnapToTarget()
    {
        if (target == null) return;
        Vector3 desiredPos = target.position + _offset;
        desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
        if (followY)
        {
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
        }
        transform.position = desiredPos;
    }
}
