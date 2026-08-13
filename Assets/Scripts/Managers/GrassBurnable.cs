using UnityEngine;

public class GrassBurnable : Burnable
{
    [Header("木头燃烧生成灰烬预制体")]
    public GameObject ashPrefab;

    // 重写生成逻辑
    protected override void SpawnAfterBurn()
    {
        if (ashPrefab == null) return;
        Instantiate(ashPrefab, transform.position, transform.rotation);
    }
}
