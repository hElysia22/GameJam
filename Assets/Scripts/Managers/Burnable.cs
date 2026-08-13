using UnityEngine;

public class Burnable : MonoBehaviour
{
    [Header("燃烧特效")]
    public GameObject burnVfx;
    [Header("燃烧后销毁延迟")]
    public float burnDelay = 1.5f;

    public bool isBurning { get; private set; }

    public void StartBurn()
    {
        if (isBurning) return;
        isBurning = true;
        if (burnVfx != null)
            burnVfx.SetActive(true);
        Invoke(nameof(RecycleSelf), burnDelay);
    }

    /// <summary>
    /// 虚方法：燃烧结束生成物体，子类重写自定义生成逻辑
    /// </summary>
    protected virtual void SpawnAfterBurn()
    {
        
    }

    void RecycleSelf()
    {
        // 先执行子类重写的生成逻辑
        SpawnAfterBurn();

        // 原有回收/销毁逻辑不变
        ItemBase item = GetComponent<ItemBase>();
        if (item != null && GeneralItemPool.Instance != null)
        {
            GeneralItemPool.Instance.RecycleItem(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // virtual OnDisable，子类可重写做额外清理
    protected virtual void OnDisable()
    {
        CancelInvoke();
    }
}
