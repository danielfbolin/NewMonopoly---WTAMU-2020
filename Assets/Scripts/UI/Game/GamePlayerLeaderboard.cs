using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using MonopolyGame;

public class GamePlayerLeaderboard : NetworkBehaviour
{
    [SerializeField]
    public Text _text;
    private StringBuilder sb;


    void Awake()
    {
        
    }


    void Start()
    {
        sb = new StringBuilder();
    }


    void Update()
    {
        // should this be updated every frame or just call an update when a player's info changes?
        sb.Clear();
        foreach (GamePlayer player in Game.GetPlayers())
        {
            sb.Append(player.Name + ": $" + player.GetMoney() + "\n" +
                      "Worth: $" + player.GetFullWorth() + "\n\n");
        }
        _text.text = sb.ToString();
    }
}