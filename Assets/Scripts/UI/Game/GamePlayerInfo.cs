using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MonopolyGame;


public class GamePlayerInfo : MonoBehaviour
{
    private GamePlayer player;

    [SerializeField]
    public Text _text;
    private string PlayerName;

    

    void Awake()
    {

    }


    void Update()
    {
        //Debug.Log("updating player: " + PlayerName);
        if (Game.GetLocalPlayer() != null)
            player = Game.GetLocalPlayer();
        if (player != null)
        {
            if (player.inGame)
                _text.text = player.ToString();
            else
                _text.text = "Bankrupt";
        }
            
        
    }
}