using UnityEngine;

public class Paddle : MonoBehaviour, IRestart
{
    [SerializeField] private float speed;

    public void Restart()
    {
        //Recentre the paddle
        transform.position = new(0, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (!RoundManager.Singleton.isRoundActive)
        {
            return;
        }

        Vector2 point = new Vector2();

        if (!Input.GetMouseButton(0))
        {
            return;
        }

        //Take the mouse position on the screen and convert it to a world position
        point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Target the left/right position of the input without moving any other axis
        Vector3 target = new Vector3(point.x, transform.position.y, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
