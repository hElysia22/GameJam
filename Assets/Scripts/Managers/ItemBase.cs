using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [Header("道具唯一标识ID")]
    public string itemId;
    [Header("拖拽自身预制体文件")]
    public GameObject itemPrefab;
    [Header("拾取判定范围")]
    public float pickRange = 0.8f;
    [Header("是否可拾取")]
    public bool canPickUp = true;

    // 场景视图绘制拾取范围圈
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickRange);
    }
}
