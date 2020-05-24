using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviourPun
{
    [SerializeField]
    private PlayerListingsMenu _playerListingsMenu;
    [SerializeField]
    private LeaveRoomMenu _leaveRoomMenu;
    private RoomCanvases _roomCanvases;
    public LeaveRoomMenu LeaveRoomMenu { get {return _leaveRoomMenu;}}

    public GameObject settingButton;
    
    void Start(){
        if(PhotonNetwork.IsMasterClient )
            settingButton.SetActive(true);
        else
        {
            settingButton.SetActive(false);
        }
    }
    public void FirstInitialize(RoomCanvases canvases){
        _roomCanvases = canvases;
        _playerListingsMenu.FirstInitialize(canvases);
        _leaveRoomMenu.FirstInitialize(canvases);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
