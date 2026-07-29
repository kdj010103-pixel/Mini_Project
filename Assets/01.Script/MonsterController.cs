using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class MonsterController : MonoBehaviour
{
    public enum MonsterState
    {
        Idle,
        Patrol,
        Trace,
        Attack,
        Hit,
        Dead
    }
    public int contactDamage = 1; // 접촉 대미지

    public float traceRange = 5f;   // 추적 거리
    public float attackRange = 1.5f; // 공격 거리

    public float moveSpeed = 2f;

    private bool isDead = false;

    private bool isHitCoroutine = false;
    private SpriteRenderer spriteRenderer;

    private Transform player;
    private Animator animator;
   
    public MonsterState currentState;

   

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        ChangeState(MonsterState.Idle);
    }

    void Update()
    {
        switch (currentState)
        {
            case MonsterState.Idle:
                Idle();
                break;

            case MonsterState.Trace:
                Trace();
                break;

            case MonsterState.Attack:
                Attack();
                break;

            case MonsterState.Dead:
                Dead();
                break;
            case MonsterState.Hit:
                Hit();
                break;
        }
    }

    public void ChangeState(MonsterState newState)
    {
        currentState = newState;


        switch (newState)
        {
            case MonsterState.Idle:
                animator.Play("SlimeIdle");
                break;


            case MonsterState.Trace:
                animator.Play("SlimeRun");
                break;


            case MonsterState.Attack:
                animator.Play("SlimeAttack");
                break;


            case MonsterState.Dead:
                animator.Play("SlimeDead");
                break;

           case MonsterState.Hit:
                animator.Play("SlimeHit");
                break;
        }
    }
    void Idle()
    {
        // 대기 행동

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= traceRange)
        {
            ChangeState(MonsterState.Trace);
        }
    }
    void Trace()
    {
        if (currentState == MonsterState.Hit)
            return;
        float distance =
            Vector2.Distance(transform.position, player.position);
        // 공격 거리 진입
        if (distance <= attackRange)
        {
            ChangeState(MonsterState.Attack);
            return;
        }
        // 추적
        transform.position =
            Vector2.MoveTowards
            (
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
            );
    }

    void Attack()
    {
        float distance =
            Vector2.Distance(transform.position, player.position);
        // 공격 거리 벗어나면 추적
        if (distance > attackRange)
        {
            ChangeState(MonsterState.Trace);
            return;
        }
        // 공격 애니메이션 실행
        // Animation Event에서 데미지 처리 추천
    }

    public void Dead()
    {
        isDead = true;

        ChangeState(MonsterState.Hit);
    }


    public void ReturnPool()
    {
        gameObject.SetActive(false);
    }


    void Hit()
    {
        // 한번만 실행
        if (!isHitCoroutine)
        {
            StartCoroutine(HitRoutine());
        }
    }

    IEnumerator DeadFade()
    {
        animator.Play("SlimeDead");

        yield return new WaitForSeconds(1f);

        float fadeTime = 3f;

        float timer = 0f;

        Color color = spriteRenderer.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(
                1f,
                0f,
                timer / fadeTime
            );
            spriteRenderer.color =
                new Color(
                    color.r,
                    color.g,
                    color.b,
                    alpha
                );
            yield return null;
        }
        gameObject.SetActive(false);
    }
    IEnumerator HitRoutine()
    {
        isHitCoroutine = true;


        animator.Play("SlimeHit");


        // Hit 애니메이션 시간
        yield return new WaitForSeconds(0.3f);



        // 사망 여부 체크
        if (isDead)
        {
            ChangeState(MonsterState.Dead);

            StartCoroutine(DeadFade());
        }
        else
        {
            ChangeState(MonsterState.Idle);
        }


        isHitCoroutine = false;
    }
    public void Die()
    {
        isDead = true;

        ChangeState(MonsterState.Hit);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player =
                collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(contactDamage);
            }
        }
    }
}