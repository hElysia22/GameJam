using System.Collections.Generic;
using UnityEngine;

public class WindTimeZone : MonoBehaviour
{
    [Header("区域时间倍率 1正常| 0.01凝滞")]
    public float localTimeScale = 1f;
    public float defaultTimeScale = 1f;

    private static List<WindTimeZone> _allZones = new List<WindTimeZone>();
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _allZones.Add(this);
    }

    private void OnDestroy()
    {
        _allZones.Remove(this);
    }

    public void SetTimeScale(float val)
    {
        localTimeScale = val;
    }

    public void ResetToDefault()
    {
        localTimeScale = defaultTimeScale;
    }

    /// <summary>
    /// 根据世界坐标获取该位置的局部时间系数
    /// </summary>
    public static float GetTimeScaleAtPoint(Vector2 worldPos)
    {
        foreach (var zone in _allZones)
        {
            if (zone._col != null && zone._col.OverlapPoint(worldPos))
            {
                return zone.localTimeScale;
            }
        }
        return 1f;
    }
}
