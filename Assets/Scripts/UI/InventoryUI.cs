using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("显示道具的Image组件")]
    public Image itemIconImage;

    /// <summary>刷新道具图标</summary>
    public void RefreshItemIcon(GameObject itemPrefab)
    {
        if (itemIconImage == null || itemPrefab == null) return;
        Sprite itemSprite = itemPrefab.GetComponent<SpriteRenderer>()?.sprite;
        if (itemSprite != null)
        {
            itemIconImage.sprite = itemSprite;
            itemIconImage.enabled = true;
        }
    }

    /// <summary>清空道具图标</summary>
    public void ClearIcon()
    {
        if (itemIconImage == null) return;
        itemIconImage.enabled = false;
        itemIconImage.sprite = null;
    }
}
