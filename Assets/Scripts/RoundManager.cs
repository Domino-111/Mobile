using UnityEngine;

//This gives us Unity Events
using UnityEngine.Events;

//This namespace helps us sort through and compare different types
using System.Linq;

public class RoundManager : MonoBehaviour
{
    private bool _isRoundActive = true;

    public bool isRoundActive => _isRoundActive;

    public UnityEvent onRoundEnd;

    private static RoundManager _singleton;

    public static RoundManager Singleton
    {
        //Access the private variable via the Get property
        get
        {
            return _singleton;
        }

        //private set means only this script can set via the property
        private set
        {
            _singleton = value; //value is whatever we tried to set the property to (i.e. player.Health = 6; value is 6)
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    public void NewGame()
    {
        _isRoundActive = true;

        foreach (IRestart restart in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IRestart>())
        {
            restart.Restart();
        }
    }

    public void EndGame()
    {
        _isRoundActive = false;

        //This triggers the event and any attached behaviours
        onRoundEnd.Invoke();

        foreach (IStop stop in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IStop>())
        {
            stop.Stop();
        }
    }
}
