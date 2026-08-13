using UnityEngine;
using System.Collections.Generic;

public class GeneralItemPool : MonoBehaviour
{
    public static GeneralItemPool Instance;
    public Transform poolParent;
    private Dictionary<GameObject, Queue<GameObject>> poolDict = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void PreloadPool(GameObject prefab, int count)
    {
        if (prefab == null || count <= 0) return;
        if (!poolDict.ContainsKey(prefab))
            poolDict[prefab] = new Queue<GameObject>();

        Queue<GameObject> queue = poolDict[prefab];
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, poolParent);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
    }

    public GameObject GetItem(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab))
            poolDict[prefab] = new Queue<GameObject>();

        Queue<GameObject> queue = poolDict[prefab];
        GameObject obj;
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
            Debug.Log($"池子复用物体：{obj.name}");
        }
        else
        {
            obj = Instantiate(prefab, poolParent);
            Debug.Log($"池子无闲置，新建物体：{prefab.name}");
        }
        obj.SetActive(true);
        return obj;
    }

    // 普通道具回收（带ItemBase）
    public void RecycleItem(GameObject obj)
    {
        ItemBase item = obj.GetComponent<ItemBase>();
        if (item != null)
        {
            RecycleItem(obj, item.itemPrefab);
        }
        else
        {
            Debug.LogError($"物体 {obj.name} 无ItemBase，请调用 RecycleItem(obj, prefab)");
            obj.SetActive(false);
        }
    }

    // 重载：桥/无拾取道具专用回收方法
    public void RecycleItem(GameObject obj, GameObject prefab)
    {
        if (obj == null || prefab == null) return;

        obj.SetActive(false);
        obj.transform.SetParent(poolParent);

        if (!poolDict.ContainsKey(prefab))
            poolDict[prefab] = new Queue<GameObject>();

        poolDict[prefab].Enqueue(obj);
        Debug.Log($"回收物体入池：{obj.name}，预制体Key：{prefab.name}");
    }
}
