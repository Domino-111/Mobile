using UnityEngine;

public class Paddle : MonoBehaviour, IRestart
{
    [SerializeField] private float speed;

    public void Restart()
    {
        //Recentre the paddle
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (!RoundManager.Singleton.isRoundActive)
        {
            return;
        }

        if (!TryGetInputPosition(out Vector3 point))
        {
            return;
        }

        //Convert our screenspace point to a world point
        point = Camera.main.ScreenToWorldPoint(point);

        //Target the left/right position of the input without moving any other axis
        Vector3 target = new Vector3(point.x, transform.position.y, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private bool TryGetInputPosition(out Vector3 position)
    {
        //This must be included to initialise the "out" variable
        position = new Vector3();

        //If we're getting touches, do this stuff
        if (Input.touchCount > 0)
        {
            position = Input.GetTouch(0).position;
            return true;
        }

        //If we make it here no touch is happening
#if UNITY_EDITOR
        //If we're in editor check for mouse controls
        if (!Input.GetMouseButton(0))
        {
            return false;
        }

        position = Input.mousePosition;
        return true;
#endif
        //If we get here we're not in editor AND we have no touch, so return false
        return false;
    }
}
