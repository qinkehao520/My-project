using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manmove : MonoBehaviour
{
    public GameObject bullet;
    public Transform fire;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float doubleJumpForce = 6f;

    // 只允许 2 段跳
    private bool canDoubleJump;
    private bool isFirstJump;

    // ====================== 冲刺新增变量 ======================
    [SerializeField] private float dashSpeed = 20f;         // 冲刺速度
    [SerializeField] private float dashTime = 0.15f;        // 冲刺时长
    [SerializeField] private float dashCooldown = 1f;       // 冲刺冷却
    private bool isDashing = false;                         // 是否正在冲刺
    private float dashTimer;                                // 冲刺计时器
    private float dashCooldownTimer;                        // 冷却计时器

    private enum MovementState { idle, running, jumping, falling, doubleJump }

    [SerializeField] private AudioSource jumpSoundEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        canDoubleJump = true;
        isFirstJump = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 从对象池拿子弹
            GameObject bullet = BulletPool.Instance.GetBullet();
            // 仅赋值枪口位置，不再控制旋转
            bullet.transform.position = fire.position;

            // 获取子弹脚本，根据角色朝向决定飞行方向
            BulletMove bulletMove = bullet.GetComponent<BulletMove>();
            if (sprite.flipX)
            {
                bulletMove.flyDir = Vector2.left;
            }
            else
            {
                bulletMove.flyDir = Vector2.right;
            }
        }

        dirX = Input.GetAxisRaw("Horizontal");

        // ====================== 冲刺冷却计时 ======================
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // ====================== 冲刺优先级最高 ======================
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing)
        {
            StartDash();
        }

        // 如果正在冲刺，直接跳过其他所有逻辑
        if (isDashing)
        {
            DashUpdate();
            return;
        }

        // 正常移动
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        // 落地重置
        if (IsGrounded())
        {
            canDoubleJump = true;
            isFirstJump = true;
        }

        // 跳跃
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                jumpSoundEffect.Play();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isFirstJump = true;
            }
            else if (!IsGrounded() && canDoubleJump)
            {
                jumpSoundEffect.Play();
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                canDoubleJump = false;
                isFirstJump = false;
            }
        }

        UpdateAnimationState();
    }

    // ====================== 开始冲刺 ======================
    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        dashCooldownTimer = dashCooldown;
    }

    // ====================== 冲刺每帧更新 ======================
    private void DashUpdate()
    {
        dashTimer -= Time.deltaTime;

        // 冲刺方向：朝当前面朝方向 / 按住的方向
        float faceDir = sprite.flipX ? -1 : 1;
        float moveDir = Mathf.Abs(dirX) > 0.1f ? dirX : faceDir;

        rb.velocity = new Vector2(moveDir * dashSpeed, 0); // 冲刺时浮空不重力

        if (dashTimer <= 0)
        {
            isDashing = false;
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        // 冲刺中不播放跳跃动画
        if (isDashing)
        {
            return;
        }

        if (rb.velocity.y > .1f)
        {
            if (isFirstJump)
                state = MovementState.jumping;
            else
                state = MovementState.doubleJump;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
