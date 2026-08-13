using UnityEngine;

public abstract class WindTimeMechanism : MonoBehaviour
{
    [SerializeField] protected float progress;
    // 缓存玩家原本父物体，离开时恢复
    private Transform _playerOriginalParent;

    protected virtual void Update()
    {
        // 获取当前机关所在位置的时间倍率
        Vector2 pos2D = transform.position;
        float timeCoeff = WindTimeZone.GetTimeScaleAtPoint(pos2D);
        // 进度随区域时间缩放
        progress += Time.deltaTime * timeCoeff;

        OnMechanismUpdate(progress);
    }

    /// <summary>
    /// 子类重写：根据进度更新位置/旋转
    /// </summary>
    protected abstract void OnMechanismUpdate(float currentProgress);

    /// <summary>
    /// 外部重置机关进度
    /// </summary>
    public virtual void ResetProgress(float p = 0f)
    {
        progress = p;
    }

    // 玩家踩到可移动平台
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform player = collision.transform;
            // 记录原始父物体，玩家无父时会存null
            _playerOriginalParent = player.parent;
            // 绑定为子物体，true 保持世界坐标不变，不会瞬移
            player.SetParent(transform, true);
        }
    }

    // 玩家跳跃/掉落离开平台
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform player = collision.transform;
            // 恢复原来父物体，存的null就直接取消父级，无报错
            player.SetParent(_playerOriginalParent);
            _playerOriginalParent = null;
        }
    }
}
