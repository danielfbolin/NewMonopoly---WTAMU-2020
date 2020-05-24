using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private RoomListing _roomListing;
    private RoomCanvases _roomsCanvases;

    private List<RoomListing> _listing = new List<RoomListing>();

        

    public void FirstInitialize(RoomCanvases canvases){
        _roomsCanvases = canvases;
    }
    public override void OnJoinedRoom()
    {
        _roomsCanvases.CurrentRoomCanvas.Show();
        _content.DestroyChildren();
        _listing.Clear();
    }
    
    public override void OnRoomListUpdate (List<RoomInfo> roomlist)
    {
        foreach (RoomInfo info in roomlist)
        {
            //removed from room list
            if(info.RemovedFromList) // the room is no longer listed
            {
                int index = _listing.FindIndex( x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(_listing[index].gameObject);
                    _listing.RemoveAt(index);
                }
            }
            //added to room list
            else
            {   
                //check if a room listing exists on a collection by the same name
                int index = _listing.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index == -1) // if nothing is found
                {
                    RoomListing listing = (RoomListing) Instantiate(_roomListing, _content);
                    if ( listing != null)
                    {
                        listing.SetRoomInfo(info);
                        _listing.Add(listing);
                    }
                }
                
            }
        }
    }
}