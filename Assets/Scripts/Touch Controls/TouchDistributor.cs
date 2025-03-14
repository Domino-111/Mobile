using UnityEngine;
using System.Linq;

public static class TouchDistributor
{
    public static bool TryGetTouch(int touchID, out Touch touchFound, params int[] excludeIDs)
    {
        //Loop through all our touches currently on the screen and try to find a new touch or maintain a current touch
        foreach (Touch touch in Input.touches)
        {
            //If the currently iterated touch is already in use, but NOT the finger we're currently trying to update, go to the next touch
            if (excludeIDs.Contains(touch.fingerId) && touchID != touch.fingerId)
            {
                continue;
            }

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
