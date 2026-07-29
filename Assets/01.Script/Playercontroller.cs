using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false; // 공격 체크
    private bool isHit;// 피격 체크

    private bool isGround;
    private Vector2 dir;
    //이동속도 점프 크기 
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpCutMultiplier = 0.5f; // 가변 점프
    [SerializeField] private float jumpGravity = 1f;   // 올라갈 때
    [SerializeField] private float fallGravity = 2f;   // 내려갈 때
    // 그라운드 체크
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 checkSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    //체력
    [SerializeField] private float maxHP = 10;
    [SerializeField] private float nowHP = 10;
    //공격
    [SerializeField] private Transform attackPoint;
    public Vector2 attackSize = new Vector2(2f, 1f);
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private float attackHitDelay = 0.15f; // 딜레이
    [SerializeField] private float attack1Duration = 0.35f;
    //공격 대상
   
    //[SerializeField] private float attack2Duration = 0.25f;
    //콤보
    //[SerializeField] private float comboWindow = 0.35f;
    //private bool comboInput = false;
    //private bool canCombo = false;
    //private Coroutine attackCoroutine;

    private float attackTimer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nowHP = maxHP;
    }

    void Update()
    {
        Move();
        Jump();
        Attack();
        UpdateAnimation();
    }
    void Move()
    {
        if (isAttacking && isGround)
        {
            dir = Vector2.zero;
            return;
        }
        dir = Vector2.zero;

        if (Keyboard.current.aKey.isPressed)
            dir += Vector2.left;

        if (Keyboard.current.dKey.isPressed)
            dir += Vector2.right;

        dir = dir.normalized;

        // 캐릭터 방향 전환
        if (dir.x < 0)
        {
            spriteRenderer.flipX = true;
            attackPoint.localPosition = new Vector3(-0.5f, 0, 0);
        }
        else if (dir.x > 0)
        {
            spriteRenderer.flipX = false;
            attackPoint.localPosition = new Vector3(0.5f, 0, 0);
        }

    }

    void Attack()
    {
        if (!Keyboard.current.zKey.wasPressedThisFrame)
            return;


        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
        //else if (!comboInput)
        //{
        //    comboInput = true;
        //}
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        //comboInput = false;

        //==================
        // Attack 1
        //==================

        animator.SetTrigger("Attack1");


        yield return new WaitForSeconds(attackHitDelay);

        DealDamage();

        // Attack1 진행 중 콤보 입력 받기
        float timer = 0f;

        while (timer < attack1Duration)
        {
            timer += Time.deltaTime;

            //if (comboInput)
            //    break;
            yield return null;
        }
        //==================
        // Attack 2
        //==================

        //if (comboInput)
        //{
        //    comboInput = false;


        //    animator.SetTrigger("Attack2");


        //    yield return new WaitForSeconds(attackHitDelay);

        //    DealDamage();


        //    yield return new WaitForSeconds(
        //        attack2Duration - attackHitDelay
        //    );
        //}


     
        //canCombo = false;

        isAttacking = false;
    }
    void DealDamage()
{
    Collider2D[] enemies = Physics2D.OverlapBoxAll(
        attackPoint.position,
        attackSize,
        0f,
        enemyLayer);

    foreach (Collider2D enemy in enemies)
    {
        enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage);
    }
}

    void Jump()
    {
        if (isAttacking)
            return;
        // New Input System 사용
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGround = false;
        }
        if (Keyboard.current.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    void UpdateAnimation()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGround", isGround);
        animator.SetFloat("YVelocity", rb.linearVelocity.y);
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                groundCheck.position,
                checkSize
            );
        }
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(
                attackPoint.position,
                attackSize
            );
        }
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            dir.x * moveSpeed,
            rb.linearVelocity.y);

        isGround = Physics2D.OverlapBox(
        groundCheck.position,
        checkSize,
        0f,
        groundLayer);
        if (rb.linearVelocity.y > 0)
            rb.gravityScale = jumpGravity;
        else
            rb.gravityScale = fallGravity;
    }



    public void TakeDamage(int damage)
    {
        if (isHit)
            return;


        nowHP -= damage;


        animator.SetTrigger("TakeHit");


        StartCoroutine(HitRoutine());


        if (nowHP <= 0)
        {
            nowHP = 0;
            Die();
        }
    }
    IEnumerator HitRoutine()
    {
        isHit = true;


        // 잠시 이동 제한
        dir = Vector2.zero;


        yield return new WaitForSeconds(0.3f);


        isHit = false;
    }

    void Die()
    {
        Debug.Log("Player Die");
    }
}