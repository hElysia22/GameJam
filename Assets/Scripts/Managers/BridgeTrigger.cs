using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    [Header("桥起止坐标")]
    public Vector2 start;
    public Vector2 end;

    // 标记桥是否已经生成，避免重复触发
    private bool bridgeSpawned = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 只有种子进入 且 桥还没生成时执行
        if (collision.CompareTag("Seed") && !bridgeSpawned)
        {
            BridgeBuilder.Instance.BuildStraightBridge(start, end);
            bridgeSpawned = true;
        }
    }

    // 可选：离开触发区回收桥（按需删除）
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Seed") && bridgeSpawned)
        {
            BridgeBuilder.Instance.ClearAllBridge();
            bridgeSpawned = false;
        }
    }
}
