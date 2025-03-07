using UnityEngine;

public static class TouchDistributor
{
    public static bool TryGetTouch(int touchID, out Touch touchFound)
    {
        //Loop through all our touches currently on the screen and try to find a new touch or maintain a current touch
        foreach (Touch touch in Input.touches)
        {
            if (touchID == -1 || touch.fingerId == touchID)   //If we have no current touch or we find our maintained touch
            {
                //Output the current touch we're iterating on
                touchFound = touch;
                return true;
            }
        }

        //Give our 'out' value a blank value
        touchFound = new Touch();
        //If we checked all the touches and none were a match, we have no touch
        return false;
    }
}
