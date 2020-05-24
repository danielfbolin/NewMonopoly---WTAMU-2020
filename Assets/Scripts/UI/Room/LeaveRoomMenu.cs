using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LeaveRoomMenu : MonoBehaviour
{   
    private ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();
    RoomCanvases _roomCanvases;

    public void FirstInitialize( RoomCanvases canvases)
    {
        _roomCanvases = canvases;
    }
    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        _roomCanvases.CurrentRoomCanvas.Hide();
        _myCustomProperties.Remove("Token");
    }

    
}
