using System.Collections.Generic;
using UnityEngine;

public class ScreenInteractionController : MonoBehaviour
{
    private ScreenInteraction[] currentInteraction = new ScreenInteraction[5];

    private static Camera _camera;

    new public static Camera camera => _camera;

    private Vector3[] touchPositionLast = new Vector3[5];

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        int[] fingerIDs = GetCurrentFingerIDs();

        for (int i = 0; i < 5; i++)
        {
            CheckTouch(i, GetCurrentFingerIDs());
        }

    }

    private void CheckTouch(int index, int[] excludeFingerIDs)
    {
        //If we find a touch
        if (TouchDistributor.TryGetTouch(currentInteraction[index] != null ? currentInteraction[index].fingerID : -1, out Touch touch, excludeFingerIDs))
        {
            //If we're starting a new interaction
            if (currentInteraction[index] == null)
            {
                //Construct one of our classes
                currentInteraction[index] = new ScreenInteraction(touch.fingerId, touch.position);
            }

            else
            {
                currentInteraction[index].Poll(touch.position);
                touchPositionLast[index] = touch.position;
            }

            //Try to find a touchable if we need to
            if (currentInteraction[index].touchable == null)
            {
                CastTouch(touch.position, index);
            }

            else
            {
                //Else we'll update our current touchable
                ManageTouch(touch.position, index);
            }

            //We found a touch so stop here
            return;
        }

        //If we don't find a touch
        NoTouch(index);
    }

    private void CastTouch(Vector3 touchPosition, int index)
    {
        Ray touchRay = _camera.ScreenPointToRay(touchPosition);

        //If we hit something
        if (Physics.Raycast(touchRay, out RaycastHit hit))
        {
            //And that something has a touchable script
            if (hit.transform.TryGetComponent<ITouchable>(out ITouchable currentTouchable))
            {
                //Begin it's touch behaviour
                currentInteraction[index].TryAddTouchable(currentTouchable);
                currentTouchable.OnTouchBegin(touchPosition);
            }
        }
    }

    private void ManageTouch(Vector3 touchPosition, int index)
    {
        currentInteraction[index].touchable.OnTouchStay(touchPosition);
    }

    private void NoTouch(int index)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && index == 0)
        {
            if (currentInteraction[index] == null)
            {
                currentInteraction[index] = new ScreenInteraction(-2, Input.mousePosition);
            }

            else
            {
                currentInteraction[index].Poll(Input.mousePosition);
                touchPositionLast[index] = Input.mousePosition;
            }

            if (currentInteraction[index].touchable == null)
            {
                CastTouch(Input.mousePosition, index);
            }

            else
            {
                ManageTouch(Input.mousePosition, index);
            }

            return;
        }
#endif

        if (currentInteraction[index] != null)
        {
            //End the current interaction
            currentInteraction[index].End(touchPositionLast[index]);
            //If we're managing a touchable
            if (currentInteraction[index].touchable != null)
            {
                currentInteraction[index].touchable.OnTouchEnd(touchPositionLast[index]);
            }

            else
            {
                //Try to swipe first
                if (ScreenInteraction.Swipe.Try(currentInteraction[index], out ScreenInteraction.Swipe swipe))
                {
                    Debug.Log($"Did a swipe, from {swipe.start} to {swipe.end} covering distance of {swipe.distance}");
                }

                else if (ScreenInteraction.Tap.Try(currentInteraction[index], out ScreenInteraction.Tap tap))
                {
                    Debug.Log($"Did a tap at {tap.screenPosition}. In world, this is {tap.WorldPosition}");
                }
            }

            currentInteraction[index] = null;
        }
    }

    private int[] GetCurrentFingerIDs()
    {
        List<int> fingerIDs = new();

        foreach (ScreenInteraction interaction in currentInteraction)
        {
            if (interaction != null)
            {
                fingerIDs.Add(interaction.fingerID);
            }
        }

        return fingerIDs.ToArray();
    }
}

public interface ITouchable
{
    public void OnTouchBegin(Vector3 touchPosition);

    public void OnTouchStay(Vector3 touchPosition);

    public void OnTouchEnd(Vector3 touchPosition);
}

