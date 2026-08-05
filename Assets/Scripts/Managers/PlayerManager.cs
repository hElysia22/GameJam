using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spr;

    public PlayerStats stats;

    private Vector2 moveDir;
    private Vector2 faceDir = Vector2.right;

    private bool isOnGround = true;
    private bool canBlowWind = true;
    private float windCoolTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        windCoolTimer = stats.windCooldown;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            moveDir = ctx.ReadValue<Vector2>();
            if (moveDir.magnitude > 0.1f)
                faceDir = moveDir.normalized;
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            moveDir = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed && isOnGround)
        {
            Debug.Log("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocityX, stats.jumpForce);
        }
    }

    public void OnWindBlow(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed) return;
        if (!canBlowWind || windCoolTimer > 0)
            return;

        Vector2 blowDir;
        if (moveDir.magnitude > 0.1f)
        {
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                blowDir = new Vector2(Mathf.Sign(moveDir.x), 0);
            else
                blowDir = new Vector2(0, Mathf.Sign(moveDir.y));
        }
        else
        {
            blowDir = faceDir;
        }

        ExecuteWindSkill(blowDir);
        windCoolTimer = stats.windCooldown;
        canBlowWind = false;
    }

    void CheckOnGround()
    {
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * 0.6f;
        isOnGround = Physics2D.OverlapCircle(checkPos, stats.detectRadius, stats.groundLayer);
    }

    //====风区相关变量====
    private Vector2 currentWindForce;

    public void SetInWindZone(Vector2 wind)
    {
        currentWindForce = wind;
    }

    private void Update()
    {
        CheckOnGround();

        if (windCoolTimer > 0)
        {
            windCoolTimer -= Time.deltaTime;
        }
        if (windCoolTimer <= 0 && isOnGround)
        {
            canBlowWind = true;
        }

        if (moveDir.x > 0.1f)
            spr.flipX = true;
        else if (moveDir.x < -0.1f)
            spr.flipX = false;
    }

    private void FixedUpdate()
    {
        HandlePhysicsMove();
        ClampVerticalSpeed();
    }

    void HandlePhysicsMove()
    {
        float inputX = moveDir.x;

        // 玩家输入移动：只有该方向速度没有超限，才施加推力
        if (Mathf.Abs(inputX) > 0.01f)
        {
            float currentXVel = rb.linearVelocity.x;
            // 如果当前速度已经朝输入方向超过最大速度，则不再叠加玩家推力
            bool overPlayerSpeedLimit = (inputX > 0 && currentXVel >= stats.maxMoveSpeed)
                                     || (inputX < 0 && currentXVel <= -stats.maxMoveSpeed);

            if (!overPlayerSpeedLimit)
            {
                rb.AddForce(Vector2.right * inputX * stats.moveAcceleration, ForceMode2D.Force);
            }
        }
        else
        {
            if (isOnGround)
            {
                Vector2 vel = rb.linearVelocity;
                float slowStep = stats.groundFriction * Time.fixedDeltaTime;
                vel.x = Mathf.MoveTowards(vel.x, 0, slowStep);
                rb.linearVelocity = vel;
            }
            else
            {
                Vector2 vel = rb.linearVelocity;
                float airDrag = 2.0f * Time.fixedDeltaTime;
                vel.x = Mathf.MoveTowards(vel.x, 0, airDrag);
                rb.linearVelocity = vel;
            }
        }
    }

    void ClampVerticalSpeed()
    {
        Vector2 vel = rb.linearVelocity;
        vel.y = Mathf.Min(vel.y, stats.maxUpVelocity);
        rb.linearVelocity = vel;
    }

    /// <summary>
    /// 处理吹风逻辑
    /// </summary>
    /// <param name="blowDir"></param>
    void ExecuteWindSkill(Vector2 blowDir)
    {
        Vector2 playerPos = transform.position;

        //反作用力
        rb.AddForce(-blowDir * stats.recoilForce, ForceMode2D.Impulse);

        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, stats.windRange, stats.windTargetLayer);
        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;

            Vector2 targetPos = hit.transform.position;
            Vector2 targetOffsetDir = (targetPos - playerPos).normalized;

            float angle = Vector2.Angle(blowDir, targetOffsetDir);
            if (angle > stats.windSectorAngle / 2f) continue;

            if (CheckObstacleBlock(playerPos, targetPos))
                continue;

            Rigidbody2D targetRb = hit.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.AddForce(blowDir * stats.pushForce, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// 射线检测与被吹的物体间是否前方有障碍物
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    bool CheckObstacleBlock(Vector2 origin, Vector2 targetPos)
    {
        Vector2 dir = (targetPos - origin).normalized;
        float distance = Vector2.Distance(origin, targetPos);
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, stats.obstacleLayer);
        Debug.DrawLine(origin, targetPos, hit ? Color.red : Color.green, 0.4f);
        return hit;
    }

    Vector2 RotateVector(Vector2 vec, float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(vec.x * cos - vec.y * sin, vec.x * sin + vec.y * cos);
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Vector2 origin = transform.position;
        Vector2 previewDir = moveDir.magnitude > 0.1f ? moveDir.normalized : faceDir;
        float halfAngle = stats.windSectorAngle / 2;

        Vector2 rightEdge = RotateVector(previewDir, halfAngle);
        Vector2 leftEdge = RotateVector(previewDir, -halfAngle);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + rightEdge * stats.windRange);
        Gizmos.DrawLine(origin, origin + leftEdge * stats.windRange);

        int segmentCount = 16;
        Vector2 prevPoint = origin + RotateVector(previewDir, halfAngle) * stats.windRange;
        for (int i = 1; i <= segmentCount; i++)
        {
            float ang = halfAngle - stats.windSectorAngle / segmentCount * i;
            Vector2 curPoint = origin + RotateVector(previewDir, ang) * stats.windRange;
            Gizmos.DrawLine(prevPoint, curPoint);
            prevPoint = curPoint;
        }
    }
}