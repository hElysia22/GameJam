using UnityEngine;
using System.Collections.Generic;

public class BridgeBuilder : MonoBehaviour
{
    public static BridgeBuilder Instance;

    [Header("桥配置")]
    public GameObject bridgeSegmentPrefab;
    // 预加载池内桥段数量，根据地图最大桥长度设置
    public int poolPreloadCount = 20;

    private List<GameObject> activeBridgeSegments = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;


    }
    private void Start()
    {
        // 启动预加载桥段进对象池
        GeneralItemPool pool = GeneralItemPool.Instance;
        pool.PreloadPool(bridgeSegmentPrefab, poolPreloadCount);
    }

    #region 1. 两点直线桥（最常用）
    /// <summary>
    /// 生成两点之间直线桥
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    public void BuildStraightBridge(Vector2 start, Vector2 end)
    {
        // 先清空上一座桥
        ClearAllBridge();

        BridgeSegment seg = bridgeSegmentPrefab.GetComponent<BridgeSegment>();
        float segLen = seg.segmentLength;

        // 总距离、方向
        Vector2 totalDir = (end - start);
        float totalDist = totalDir.magnitude;
        Vector2 dirNormal = totalDir.normalized;

        // 需要多少节桥段
        int segmentCount = Mathf.CeilToInt(totalDist / segLen);

        for (int i = 0; i < segmentCount; i++)
        {
            // 每一节的生成位置
            Vector2 spawnPos = start + dirNormal * segLen * (i + 0.5f);
            // 旋转对齐桥面朝向
            float angle = Mathf.Atan2(dirNormal.y, dirNormal.x) * Mathf.Rad2Deg;

            GameObject piece = GeneralItemPool.Instance.GetItem(bridgeSegmentPrefab);
            piece.transform.position = spawnPos;
            piece.transform.rotation = Quaternion.Euler(0, 0, angle);

            activeBridgeSegments.Add(piece);
        }
    }
    #endregion

    #region 2. 多点路径桥（折线/弯道桥）
    /// <summary>
    /// 多点路径桥，依次连接每个坐标点
    /// </summary>
    public void BuildPathBridge(List<Vector2> pathPoints)
    {
        ClearAllBridge();
        if (pathPoints.Count < 2) return;

        BridgeSegment seg = bridgeSegmentPrefab.GetComponent<BridgeSegment>();
        float segLen = seg.segmentLength;

        for (int p = 0; p < pathPoints.Count - 1; p++)
        {
            Vector2 pointA = pathPoints[p];
            Vector2 pointB = pathPoints[p + 1];

            Vector2 dir = (pointB - pointA).normalized;
            float distance = Vector2.Distance(pointA, pointB);
            int count = Mathf.CeilToInt(distance / segLen);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            for (int i = 0; i < count; i++)
            {
                Vector2 pos = pointA + dir * segLen * (i + 0.5f);
                GameObject piece = GeneralItemPool.Instance.GetItem(bridgeSegmentPrefab);
                piece.transform.position = pos;
                piece.transform.rotation = Quaternion.Euler(0, 0, angle);
                activeBridgeSegments.Add(piece);
            }
        }
    }
    #endregion

    #region 销毁回收桥（关键，不销毁物体，回收入池）
    /// <summary>
    /// 清空当前所有桥段，回收进对象池
    /// </summary>
    public void ClearAllBridge()
    {
        GeneralItemPool pool = GeneralItemPool.Instance;
        foreach (GameObject seg in activeBridgeSegments)
        {
            pool.RecycleItem(seg);
        }
        activeBridgeSegments.Clear();
    }
    #endregion
}
