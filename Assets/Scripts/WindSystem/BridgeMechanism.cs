using UnityEngine;

public class BridgeMechanism : WindTimeMechanism
{
    [Header("完整升降周期(现实秒数)")]
    public float cycle = 6f;
    public float lowerY;
    public float upperY;

    protected override void OnMechanismUpdate(float currentProgress)
    {
        float t = (currentProgress % cycle) / cycle;
        float s = (Mathf.Sin(t * Mathf.PI * 2f) + 1f) / 2f;
        float targetY = Mathf.Lerp(lowerY, upperY, s);

        Vector3 localPos = transform.localPosition;
        localPos.y = targetY;
        transform.localPosition = localPos;
    }
}
