using UnityEngine;

public class GamePiece : MonoBehaviour, ITouchable
{
    public void OnTouchBegin(Vector3 touchPosition)
    {

    }

    public void OnTouchEnd(Vector3 touchPosition)
    {

    }

    public void OnTouchStay(Vector3 touchPosition)
    {
        //Get a world space position based on the touch input
        Vector3 newPosition = ScreenInteractionController.camera.ScreenToWorldPoint(touchPosition);

        //Maintain our z position
        newPosition.z = transform.position.z;

        //Apply the new position
        transform.position = newPosition;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
