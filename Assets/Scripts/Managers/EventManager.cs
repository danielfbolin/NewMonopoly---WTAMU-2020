using System.Collections;
using UnityEngine;

public class EventManager : SingletonBehaviour<EventManager>
{
    public delegate void OnEndTurn();
    public static event OnEndTurn onEndTurn;

    public static void RaiseEndTurn()
    {
        onEndTurn();
    }
}