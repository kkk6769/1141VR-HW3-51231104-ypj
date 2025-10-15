using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("跳跃检测")]
    public LayerMask groundLayerMask = -1;
    public float groundCheckDistance = 0.1f;
    
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isGrounded;
    private float horizontalInput;
    
    // 当项目使用新的 Input System 时，这个宏会启用相关分支的编译
    private bool useNewInputSystem = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        
#if ENABLE_INPUT_SYSTEM
        // 如果启用了新的 Input System，则尝试检测键盘是否存在
        useNewInputSystem = Keyboard.current != null;
#else
        useNewInputSystem = false;
#endif
    }
    
    void Update()
    {
        // 获取输入（兼容新旧 Input 系统）
        if (useNewInputSystem)
        {
#if ENABLE_INPUT_SYSTEM
            float left = (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) ? -1f : 0f;
            float right = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) ? 1f : 0f;
            horizontalInput = right + left;

            // 检测是否在地面上
            CheckGrounded();

            // 跳跃输入（检测按下的一帧）
            if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) && isGrounded)
            {
                Jump();
            }

            // 下跳（按住）
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                StartCoroutine(DisableCollisionTemporarily());
            }
#endif
        }
        else
        {
            // 旧输入系统
            horizontalInput = Input.GetAxis("Horizontal");

            // 检测是否在地面上
            CheckGrounded();

            // 跳跃输入
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }

            // 下跳（穿过平台）
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                // 临时禁用碰撞器以穿过平台
                StartCoroutine(DisableCollisionTemporarily());
            }
        }
    }
    
    void FixedUpdate()
    {
        // 水平移动
    rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        
        // 翻转角色朝向
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    void CheckGrounded()
    {
        // 使用OverlapBox检测指定Layer上的碰撞体，避免依赖Tag
        Vector2 boxCenter = col.bounds.center;
        Vector2 boxSize = col.bounds.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter + Vector2.down * groundCheckDistance, boxSize, 0f, groundLayerMask);

        isGrounded = hits != null && hits.Length > 0;

        // 调试可视化（在Scene视图显示OverlapBox）
        Color boxColor = isGrounded ? Color.green : Color.red;
        Vector3 dbgCenter = boxCenter + Vector2.down * groundCheckDistance;
        Debug.DrawLine(dbgCenter - (Vector3)(boxSize * 0.5f), dbgCenter + new Vector3(boxSize.x * 0.5f, 0, 0), boxColor);
        Debug.DrawLine(dbgCenter - (Vector3)(boxSize * 0.5f), dbgCenter + new Vector3(0, boxSize.y * 0.5f, 0), boxColor);
    }
    
    void Jump()
    {
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    
    System.Collections.IEnumerator DisableCollisionTemporarily()
    {
        // 临时禁用与地面的碰撞
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Default"), true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Default"), false);
    }
    
    void OnDrawGizmosSelected()
    {
        // 在Scene视图中显示地面检测区域
        if (col != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(
                col.bounds.center + Vector3.down * groundCheckDistance,
                col.bounds.size
            );
        }
    }
}