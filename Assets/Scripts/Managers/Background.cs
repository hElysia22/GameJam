using UnityEngine;

public class BgFitCamera : MonoBehaviour
{
    private Camera cam;
    private SpriteRenderer spr;

    void Awake()
    {
        cam = Camera.main;
        spr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        FitScreenSize();
    }

    // 窗口大小变化时重新适配
    void Update()
    {
        FitScreenSize();
    }

    void FitScreenSize()
    {
        // 相机正交视野尺寸
        float viewHeight = cam.orthographicSize * 2f;
        float viewWidth = viewHeight * cam.aspect;

        // 精灵原图宽高
        float texW = spr.sprite.bounds.size.x;
        float texH = spr.sprite.bounds.size.y;

        // 计算缩放，保证完全覆盖屏幕
        float scaleX = viewWidth / texW;
        float scaleY = viewHeight / texH;
        float finalScale = Mathf.Max(scaleX, scaleY);

        transform.localScale = Vector3.one * finalScale;
    }
}
