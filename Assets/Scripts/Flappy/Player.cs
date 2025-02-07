using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float jumpPower;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Mouse button 0 is left click
        if (Input.GetMouseButtonDown(0))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = Vector2.up * jumpPower;
    }
}
