using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListing : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Text _text;
    [SerializeField]
    public Graphic image;


    public Player Player {get; private set;}    //Photon player

    public bool Ready = false;
    public string token;

    
    public void SetPlayerInfo(Player player)
    {   
        Player = player;
        SetPlayerText(player);
    }


    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(target,changedProps);
        if( target != null && target == Player)
        {
            if (changedProps.ContainsKey("Token"))
                SetPlayerText(target);
        }
    }
    private void SetPlayerText(Player player)
    {
        token ="Pick Token";
        if (player.CustomProperties.ContainsKey("Token"))
            token = (string)player.CustomProperties["Token"];
        _text.text = player.NickName + ", " + token;
    }
    private void OnDestroy() {
        
    }
}
