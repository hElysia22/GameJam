using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("放置道具向前偏移距离")]
    public float placeOffset = 1f;
    [Header("背包UI面板物体")]
    public InventoryUI inventoryUI;

    public PlayerManager playerManager;

    // 当前手持道具信息
    public string CurrentItemId { get; private set; }
    public GameObject StoredItemPrefab { get; private set; }
    public bool HasItem => StoredItemPrefab != null;

    private Transform _playerTrans;

    private void Awake()
    {
        _playerTrans = transform;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Debug.Log($"按下交互键，是否持有道具：{HasItem}");
        // 先查找附近是否存在可拾取道具
        ItemBase nearestItem = FindNearestPickItem();

        if (HasItem)
        {
            // 手里有东西
            if (nearestItem != null)
            {
                // 附近有新道具：先放下当前道具，再拾取新道具
                PlaceItem();
                PickUp(nearestItem);
            }
            else
            {
                // 附近无道具：单纯放置手中物品
                PlaceItem();
            }
        }
        else
        {
            // 空手状态，正常拾取
            if (nearestItem != null)
            {
                PickUp(nearestItem);
            }
            else
            {
                Debug.Log("附近没有可拾取道具");
            }
        }
    }

    private ItemBase FindNearestPickItem()
    {
        ItemBase[] allSceneItems = Object.FindObjectsByType<ItemBase>(FindObjectsSortMode.None);
        ItemBase nearestItem = null;
        float minDistance = float.MaxValue;
        Vector2 playerPos = (Vector2)_playerTrans.position;

        foreach (ItemBase item in allSceneItems)
        {
            if (!item.canPickUp) continue;
            if (!item.gameObject.activeSelf) continue;
            float distance = Vector2.Distance(playerPos, item.transform.position);
            if (distance < item.pickRange && distance < minDistance)
            {
                minDistance = distance;
                nearestItem = item;
            }
        }
        return nearestItem;
    }


    // 拾取道具
    private void PickUp(ItemBase item)
    {
        
        GameObject sourcePrefab = item.itemPrefab;
        CurrentItemId = item.itemId;
        StoredItemPrefab = sourcePrefab;

        
        if (StoredItemPrefab == null)
        {
            Debug.LogError($"道具 {item.name} 的ItemBase.itemPrefab未绑定Project原始预制体！");
            GeneralItemPool.Instance.RecycleItem(item.gameObject);
            return;
        }

        Debug.Log($"拾取完成，存储预制体：{StoredItemPrefab.name}");
        
        GeneralItemPool.Instance.RecycleItem(item.gameObject);

        // 拾取缩放动画
        StartCoroutine(PickAnim());
        // 刷新UI图标
        inventoryUI?.RefreshItemIcon(StoredItemPrefab);
    }

    // 放置道具逻辑（从对象池取出复用）
    private void PlaceItem()
    {
        Debug.Log($"【放置调试】当前存储预制体：{StoredItemPrefab.name}");
        if (StoredItemPrefab == null)
        {
            Debug.LogError("没有手持物品，无法放置");
            return;
        }
 
        Vector2 spawnPos = (Vector2)_playerTrans.position + playerManager.faceDir * placeOffset;
        GameObject newItem = GeneralItemPool.Instance.GetItem(StoredItemPrefab);
        newItem.transform.position = spawnPos;
        newItem.transform.rotation = Quaternion.identity;

        Debug.Log($"道具放置完成，坐标：{spawnPos}");
        ClearInventory();
    }


    // 清空背包手持道具
    public void ClearInventory()
    {
        CurrentItemId = string.Empty;
        StoredItemPrefab = null;
        Debug.Log("背包清空");
        inventoryUI?.ClearIcon();
    }

    // 拾取缩放动画
    private IEnumerator PickAnim()
    {
        transform.localScale = Vector3.one * 1.25f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = Vector3.one;
    }
}
