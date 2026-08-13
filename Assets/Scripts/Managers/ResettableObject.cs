using UnityEngine;

public class ResettableObject : MonoBehaviour
{
    [Header("是否重置刚体速度")]
    public bool resetVelocity = true;

    private Vector3 originPos;
    private Quaternion originRot;
    private Rigidbody2D rb2d;

    void Awake()
    {
        // 保存出生初始状态
        originPos = transform.position;
        originRot = transform.rotation;
        rb2d = GetComponent<Rigidbody2D>();
    }

    // 外部调用复位
    public void ResetState()
    {
        transform.position = originPos;
        transform.rotation = originRot;

        if (resetVelocity && rb2d != null)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
        }
    }
}
