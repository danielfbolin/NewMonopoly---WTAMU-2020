using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PlayerListing _playerListing;
    [SerializeField]
    private Text _readyUpText;

    private bool _ready = false;
    private Color _color;
    

    private List<PlayerListing> _listing = new List<PlayerListing>();

    
    private RoomCanvases _roomCanvases;

    public GameObject toggle; // GameSetting 

   private void Awake()
   {
       GetCurrentRoomPlayers();
        toggle.GetComponent<SettingBoard>().Active();
        toggle.SetActive(false);
   }

    public override void OnEnable(){
        base.OnEnable();
        //SetReadyUp(false);
        SetReadyUp(false);
        GetCurrentRoomPlayers();
        
    }
    public override void OnDisable() {
        base.OnDisable();
        for(int i = 0; i< _listing.Count; i++)
        {
            Destroy(_listing[i].gameObject);
            
        }
        _listing.Clear();
    }

    public void FirstInitialize(RoomCanvases canvases)
    {
        _roomCanvases = canvases;
    }

    private void GetCurrentRoomPlayers()
    {
        if( !PhotonNetwork.IsConnected)
            return;
        if ( PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayersListing(playerInfo.Value);
        }
        
    }

    
    private void AddPlayersListing(Player player)
    {
        int index = _listing.FindIndex(x => x.Player == player);
        if (index != -1) // if player exists
        {
            _listing[index].SetPlayerInfo(player); // update info
        }
        else
        {
            PlayerListing listing =  Instantiate(_playerListing, _content); //Instantiate: make a
            if ( listing != null)
            {
              listing.SetPlayerInfo(player);
              _listing.Add(listing);
            }
        }
    }
    //OnMasterClientSwitched is called when Master Client leave or is changed
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _roomCanvases.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayersListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listing.FindIndex( x => x.Player == otherPlayer);
        if(index != -1)
        {
            Destroy(_listing[index].gameObject);    //destroy gameobject
            _listing.RemoveAt(index);               //remove from listing
        }

    }

    private void SetReadyUp(bool state)
    {
        _ready = state;
        if (_ready)
        {
            _readyUpText.text = "Ready";
            _color = new Color(255, 255, 255);
}
        else
        {
            _readyUpText.text = "Not Ready";
            _color = new Color(90, 220, 90);
        }
    }

    public void OnClick_Readyup()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            SetReadyUp(!_ready);
            base.photonView.RPC("RPC_ChangeReadyState", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, _ready);
            //base.photonView.RPC("RPC_ChangeReadyColor", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, _color);
        }
    }


    [PunRPC]
    private void RPC_ChangeReadyState(Player player, bool ready)
    {
        int index = _listing.FindIndex( x => x.Player == player);
        if(index != -1)
        {
            _listing[index].Ready = ready;
        }

    }


    [PunRPC]
    private void RPC_ChangeReadyColor(Player player, Color color)
    {
        int index = _listing.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listing[index].image.GetComponent<Graphic>().color = color;
        }

    }

    public void OnClick_StartGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            //check if all players are ready
            for( int i = 0; i< _listing.Count; i++)
            {
                if(_listing[i].Player != PhotonNetwork.LocalPlayer)
                {
                    if(!_listing[i].Ready)
                        return;
                }
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
        }
    }


    public void OnClick_Setting(){
        toggle.SetActive(true);
    }

    

    
}
