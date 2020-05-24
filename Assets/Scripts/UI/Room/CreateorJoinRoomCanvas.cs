using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateorJoinRoomCanvas : MonoBehaviour
{
    [SerializeField]
    private CreateRoom _createRoomMenu;
    [SerializeField]
    private RoomListingMenu _roomListingsMenu;

    private RoomCanvases _roomCanvases;
    
    public void FirstInitialize(RoomCanvases canvases){
        _roomCanvases = canvases;
        _createRoomMenu.FirstInitialize(canvases);
        _roomListingsMenu.FirstInitialize(canvases);
    }
}
