using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int hp;

    private MonsterController monster;

    void Awake()
    {
        monster = GetComponent<MonsterController>();
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;


        Debug.Log($"{name} HP : {hp}");


        if (hp <= 0)
        {
            hp = 0;

            monster.Die();
        }
        else
        {
            monster.ChangeState(
                MonsterController.MonsterState.Hit
            );
        }
    }
    // 플레이어와 접촉 데미지
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player =
                collision.GetComponent<PlayerController>();


            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}