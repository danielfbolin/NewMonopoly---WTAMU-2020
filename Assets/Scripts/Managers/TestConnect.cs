using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class TestConnect : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text playerName;
    public GameObject login;
    public GameObject canvases;
    

    

    private void Awake() {
        canvases.SetActive(false);
    }
    
    public void OnClick_Enter()
    {   
        Debug.Log("Player name: " + playerName.text);
        Debug.Log("Connecting to server.");
        PhotonNetwork.AutomaticallySyncScene= true;
        //PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.NickName = playerName.text;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
        login.SetActive(false);
        canvases.SetActive(true);
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {

        Debug.Log("Connected to server. ",this);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected to server for reason: " + cause.ToString());
    }
}
