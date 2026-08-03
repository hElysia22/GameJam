using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spr;

    private Vector2 moveDir;
    private Vector2 skillDir;
    public LayerMask groundLayer;
    public float speed = 2.0f;
    public float jumpForce = 6f;
    public float detectRadius = 0.15f;

    //判断角色是否落地，防止无限跳
    private bool isOnGround = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            moveDir = ctx.ReadValue<Vector2>();

        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            moveDir = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Performed && isOnGround)
        {
            Debug.Log("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }


    void CheckOnGround()
    { 
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * 0.6f;
        isOnGround = Physics2D.OverlapCircle(checkPos, detectRadius, groundLayer);
        
    }

    private void Update()
    {
        CheckOnGround();

        if (moveDir.x > 0.1f)
            spr.flipX = true;
        else if (moveDir.x < -0.1f)
            spr.flipX = false;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDir.x * speed, rb.linearVelocity.y);
    }
}
