using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Layer设置")]
    public LayerMask groundLayer;
    public LayerMask windTargetLayer;
    public LayerMask obstacleLayer;

    [Header("移动跳跃")]
    public float moveAcceleration = 14f;    // 移动推力加速度
    public float maxMoveSpeed = 3.0f;       // 水平最大速度
    public float groundFriction = 14f;       // 地面摩擦力（松开按键减速）
    public float jumpForce = 10f;
    public float detectRadius = 0.15f;
    public float airDrag = 2.0f;

    [Header("吹风技能设置")]
    public float windRange = 3.5f;
    [Range(20f, 100f)] public float windSectorAngle = 60f;
    public float pushForce = 7f;
    public float recoilForce = 14f;        // 吹风后坐冲量
    public float windCooldown = 2f;

    [Header("垂直限制")]
    public float maxUpVelocity = 6f;
}