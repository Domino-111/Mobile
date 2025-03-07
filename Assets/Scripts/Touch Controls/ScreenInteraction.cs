using System.Collections.Generic;
using UnityEngine;

public class ScreenInteraction
{
    public int fingerID;
    public Vector3 screenPositionStart, screenPositionEnd;
    public float timeStart;
    public List<Vector3> screenPositionsPolled = new List<Vector3>();

    public ITouchable touchable;

    /// <summary>
    /// How long the interaction has laster for in seconds.
    /// </summary>
    public float duration => Time.time - timeStart;

    /// <summary>
    /// Returns the current position of the screen interaction.
    /// </summary>
                                    //if screenPositionsPolled.Count is larger than 1 use screenPositionsPolled[^1]. Else use screenPositionStart
    public Vector3 screenPositionCurrent => screenPositionsPolled.Count > 1 ? screenPositionsPolled[^1] : screenPositionStart;  //[^1] gives you the last index of a collection

    public ScreenInteraction(int fingerID, Vector3 screenPositionStart)
    {
        this.fingerID = fingerID;
        this.screenPositionStart = screenPositionStart;
        screenPositionsPolled.Add(screenPositionStart);
        timeStart = Time.time;
    }

    /// <summary>
    /// Add a touch position to the current screen interaction.
    /// </summary>
    /// <param name="screenPosition"></param>
    public void Poll(Vector3 screenPosition)
    {
        screenPositionsPolled.Add(screenPosition);
    }

    /// <summary>
    /// Finalise a screen interaction.
    /// </summary>
    /// <param name="screenPosition"></param>
    public void End(Vector3 screenPosition)
    {
        screenPositionsPolled.Add(screenPosition);
        screenPositionEnd = screenPosition;
    }

    public bool TryAddTouchable(ITouchable touchable)
    {
        if (this.touchable != null)
        {
            return false;
        }

        this.touchable = touchable;
        return true;
    }

    //readonly means this variable connot be altered at runtime
    public readonly static float swipeDistanceThreshold = 100f;

    public class Tap
    {
        public Vector3 screenPosition;
        public float timeStart;

        /// <summary>
        /// Get the relative world position of the tap.
        /// </summary>
        public Vector3 WorldPosition => ScreenInteractionController.camera.ScreenToWorldPoint(screenPosition);

        private Tap(Vector3 screenPosition, float timeStart)
        {
            this.screenPosition = screenPosition;
            this.timeStart = timeStart;
        }

        public static bool Try(ScreenInteraction screenInteraction, out Tap tap)
        {
            //Make sure our out has a value even if that value is null
            tap = null;
            //Check if the distance covered by our interaction is too far for a tap
            float distance = Vector2.Distance(screenInteraction.screenPositionStart, screenInteraction.screenPositionEnd);

            if (distance > swipeDistanceThreshold)
            {
                return false;
            }

            //Otherwise we have a valid tap, so construct one
            tap = new Tap(screenInteraction.screenPositionStart, screenInteraction.timeStart);
            return true;
        }
    }

    public class Swipe
    {
        public float distance;
        public Vector2 direction;
        public Vector3 start, end;

        public Vector3 WorldStart => ScreenInteractionController.camera.ScreenToWorldPoint(start);
        public Vector3 WorldEnd => ScreenInteractionController.camera.ScreenToWorldPoint(end);
        public float WorldDistance => Vector3.Distance(WorldStart, WorldEnd);

        private Swipe(Vector2 start, Vector2 end)
        {
            this .start = start;
            this .end = end;
            distance = Vector2.Distance(start, end);
            direction = (end - start).normalized;
        }

        public static bool Try(ScreenInteraction screenInteraction, out Swipe swipe)
        {
            swipe = null;
            float distance = Vector2.Distance(screenInteraction.screenPositionStart, screenInteraction.screenPositionEnd);

            //If we're too short to swipe
            if (distance < swipeDistanceThreshold)
            {
                return false;
            }

            swipe = new Swipe(screenInteraction.screenPositionStart, screenInteraction.screenPositionEnd);
            return true;
        }
    }
}
