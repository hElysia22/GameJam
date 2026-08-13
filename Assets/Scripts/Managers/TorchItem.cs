using UnityEngine;

public class TorchItem : ItemBase
{
    [Header("火焰特效物体")]
    public GameObject fireVfx;
    [Header("点燃状态")]
    public bool isLit = true;

    private void Start()
    {
        UpdateFireDisplay();
    }

    public void UpdateFireDisplay()
    {
        if (fireVfx != null)
            fireVfx.SetActive(isLit);
    }

    public void ExtinguishTorch()
    {
        if (!isLit) return;
        isLit = false;
        UpdateFireDisplay();
        Debug.Log("火把被风吹灭");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, pickRange);
    }
}
