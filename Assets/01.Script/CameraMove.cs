using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform target;

    Vector3 velocity;

    Vector3 targetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocity = Vector3.zero;    
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.2f);
    }
}
