using UnityEngine;

public class PipePair : MonoBehaviour, IStop, IRestart
{
    private Rigidbody2D rb;

    [SerializeField] private float speed;

    public void Restart()
    {
        Destroy(gameObject);
    }

    public void Stop()
    {
        rb.simulated = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.left * speed;   //Same as writing (-1, 0) * speed
    }

    void Update()
    {
        
    }
}
