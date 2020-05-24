using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    public GameObject obj;  //obj holds CurrentRoomCanvas
    public Text sayHello;
    public Text playerName;
    
    private List<string> nameList = new List<string>();
    private CreateRoom _createRoomMenu;

    [SerializeField]
    private Text _roomName;

    //passing value between CurrentRoomCanvas , RoomCanvases
    
    private RoomCanvases _roomCanvases;     

    public void FirstInitialize(RoomCanvases canvases){
        _roomCanvases = canvases;
    }



    private void Start() {
        sayHello.text = "Hello, " + playerName.text;    //display player's name

    }
    public void OnClick_CreateRoom()
    {
        if ( !PhotonNetwork.IsConnected)
            return;
        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;
        options.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create room successfully.", this);
        Debug.Log("change scene");
        //_roomCanvases.CurrentRoomCanvas.Show();
        obj.SetActive(true);    

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed: " + message, this);
    }
}
