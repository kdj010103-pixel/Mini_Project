using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField] int maxHP;
    [SerializeField] int nowHP;

    float moveSpeed;

    Transform target;

    SpriteRenderer sr;

    // 공격 사정거리
    [SerializeField] float range;
    private void OnEnable()
    {
        nowHP = 10;
        maxHP = 10;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed = 3f;
        sr = GetComponent<SpriteRenderer>();
       
        target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }
    void CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, target.position); // 나하고 타겟 사이의 거리

        if (distance < range)
        {
            // 공격 -> 적 방향으로 공격

        }
        else
        {

            Trace();
        }
    }
        void Trace()
        {
     
            sr.flipX = CheckFlip();

  
            Move();
        }

        void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        }

        Vector2 GetDirection()
        {
            return target.position - transform.position;
        }

        bool CheckFlip()
        {
            return transform.position.x > target.position.x ? true : false;
        }

    public void TakeDamage(int damage)
    {
        nowHP -= damage;
        if (nowHP < 0)
        {
            nowHP = 0;
            Die();
        }
    }

    void Die()
    {
        StageManager.instance.RemoveMonster(this.gameObject);
        ReturnPool();
    }

    public void ReturnPool()
    {
        ObjectPoolManager.instance.ReturnObject("Monster", this.gameObject);
    }

}
