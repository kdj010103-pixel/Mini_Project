using UnityEngine;

public class BackGround : MonoBehaviour
{
    
    private float moveSpeed = 5f;
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        if (transform.position.x < -19)
        {
            transform.position += new Vector3(38f,0,0);
        }
    }
}
