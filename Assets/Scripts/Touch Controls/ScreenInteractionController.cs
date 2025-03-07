using UnityEngine;

public class ScreenInteractionController : MonoBehaviour
{
    private ScreenInteraction currentInteraction;

    private static Camera _camera;

    new public static Camera camera => _camera;

    private Vector3 touchPositionLast;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        CheckTouch();
    }

    private void CheckTouch()
    {
        //If we find a touch
        if (TouchDistributor.TryGetTouch(currentInteraction != null ? currentInteraction.fingerID : -1, out Touch touch))
        {
            //If we're starting a new interaction
            if (currentInteraction == null)
            {
                //Construct one of our classes
                currentInteraction = new ScreenInteraction(touch.fingerId, touch.position);
            }

            else
            {
                currentInteraction.Poll(touch.position);
                touchPositionLast = touch.position;
            }

            //Try to find a touchable if we need to
            if (currentInteraction.touchable == null)
            {
                CastTouch(touch.position);
            }

            else
            {
                //Else we'll update our current touchable
                ManageTouch(touch.position);
            }

            //We found a touch so stop here
            return;
        }

        //If we don't find a touch
        NoTouch();
    }

    private void CastTouch(Vector3 touchPosition)
    {
        Ray touchRay = _camera.ScreenPointToRay(touchPosition);

        //If we hit something
        if (Physics.Raycast(touchRay, out RaycastHit hit))
        {
            //And that something has a touchable script
            if (hit.transform.TryGetComponent<ITouchable>(out ITouchable currentTouchable))
            {
                //Begin it's touch behaviour
                currentInteraction.TryAddTouchable(currentTouchable);
                currentTouchable.OnTouchBegin(touchPosition);
            }
        }
    }

    private void ManageTouch(Vector3 touchPosition)
    {
        currentInteraction.touchable.OnTouchStay(touchPosition);
    }

    private void NoTouch()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (currentInteraction == null)
            {
                currentInteraction = new ScreenInteraction(-2, Input.mousePosition);
            }

            else
            {
                currentInteraction.Poll(Input.mousePosition);
                touchPositionLast = Input.mousePosition;
            }

            if (currentInteraction.touchable == null)
            {
                CastTouch(Input.mousePosition);
            }

            else
            {
                ManageTouch(Input.mousePosition);
            }

            return;
        }
#endif

        if (currentInteraction != null)
        {
            //End the current interaction
            currentInteraction.End(touchPositionLast);
            //If we're managing a touchable
            if (currentInteraction.touchable != null)
            {
                currentInteraction.touchable.OnTouchEnd(touchPositionLast);
            }

            else
            {
                //Try to swipe first
                if (ScreenInteraction.Swipe.Try(currentInteraction, out ScreenInteraction.Swipe swipe))
                {
                    Debug.Log($"Did a swipe, from {swipe.start} to {swipe.end} covering distance of {swipe.distance}");
                }

                else if (ScreenInteraction.Tap.Try(currentInteraction, out ScreenInteraction.Tap tap))
                {
                    Debug.Log($"Did a tap at {tap.screenPosition}. In world, this is {tap.WorldPosition}");
                }
            }

            currentInteraction = null;
        }
    }
}

public interface ITouchable
{
    public void OnTouchBegin(Vector3 touchPosition);

    public void OnTouchStay(Vector3 touchPosition);

    public void OnTouchEnd(Vector3 touchPosition);
}

