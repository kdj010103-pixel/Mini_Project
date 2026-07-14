using UnityEngine;
using UnityEngine.InputSystem;

public class Playercontroller : MonoBehaviour
{
    Rigidbody rb;

    Vector2 dir;

    public float moveSpeed;

    [SerializeField] float maxHP;
    [SerializeField] float nowHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb= GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        dir = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        { dir += Vector2.up; }
        if (Keyboard.current.sKey.isPressed)
        { dir += Vector2.down; }
        if (Keyboard.current.aKey.isPressed)
        { dir += Vector2.left; }
        if (Keyboard.current.dKey.isPressed)
        { dir += Vector2.right ; }

        dir = dir.normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = dir * moveSpeed;
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
        StageManager.instance.ClearMonsterList();
    }
}
