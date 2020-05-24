using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCanvases : MonoBehaviour
{
    [SerializeField]
    private CreateorJoinRoomCanvas _createOrJoinRoomCanvas;
    private CreateorJoinRoomCanvas CreateOrJoinRoomCanvas {get {return _createOrJoinRoomCanvas;}}

    [SerializeField]
    private CurrentRoomCanvas _currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas {get {return _currentRoomCanvas;}}


    private void Awake() {
        FirstInitialize();
        
    }
    private void FirstInitialize()
    {
        CreateOrJoinRoomCanvas.FirstInitialize(this);
        CurrentRoomCanvas.FirstInitialize(this);
    }
}
