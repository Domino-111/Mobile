using UnityEngine;

public class PipePair : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.left * speed;   //Same as writing (-1, 0) * speed
    }

    void Update()
    {
        
    }
}
