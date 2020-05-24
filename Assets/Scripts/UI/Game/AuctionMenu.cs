using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MonopolyGame;
using System;

public class AuctionMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PropertyBuyMenu propertyBuyMenu;
    [SerializeField]
    private Transform _interactPanel;
    [SerializeField]
    private Text _propertyName;
    [SerializeField]
    private Text _propertyInfo;
    [SerializeField]
    private Text _bidText;
    [SerializeField]
    private Text _bidAmount;
    [SerializeField]
    private InputField _bidInput;
    [SerializeField]
    private Button _bidButton;
    private Tile tile;
    private GamePlayer player;
    private int currentBidAmount;
    private int playerBidAmount;
    private string currentBidPlayerName;
    private List<string> biddingPlayers;
    private string text;


    void Awake()
    {
        
    }


    void Start()
    {
        biddingPlayers = new List<string>();
    }


    void Update()
    {

    }


    public override void OnEnable()
    {
        base.OnEnable();
        
        _interactPanel.gameObject.SetActive(true);
    }


    public void Activate(Tile auctionTile)
    {
        player = Game.GetLocalPlayer();
        tile = auctionTile;
        _propertyName.text = tile.name;
        _propertyInfo.text = tile.ToString();
        _bidText.text = "Minimum bid:";
        _bidAmount.text = "$10";
        currentBidAmount = 0;
        currentBidPlayerName = "";
        playerBidAmount = 0;
        biddingPlayers.Clear();
        foreach (GamePlayer playerListing in Game.GetPlayers())
        {
            biddingPlayers.Add(playerListing.Name);
            Debug.Log(playerListing.Name + " added to Auction");
        }
    }


    public void UpdateBidText()
    {
        text = _bidInput.text;
        if (text != null && text != "")
        {
            playerBidAmount = int.Parse(text);
            if (player.Name != currentBidPlayerName && playerBidAmount >= 10 && playerBidAmount > currentBidAmount && playerBidAmount <= player.GetMoney())
                _bidButton.interactable = true;
            else
                _bidButton.interactable = false;
        }
        else
            _bidButton.interactable = false;
    }


    public void PlaceBid()
    {
        currentBidAmount = playerBidAmount;
        currentBidPlayerName = player.Name;
        _bidText.text = "Current Bid:";
        _bidAmount.text = string.Format("{0}: ${1}", currentBidPlayerName, currentBidAmount);
        _bidInput.text = "";
        base.photonView.RPC("RPC_PlaceBid", RpcTarget.Others, player.Name, playerBidAmount);
        if (biddingPlayers.Count == 1)
            LeaveAuction();
    }


    [PunRPC]
    public void RPC_PlaceBid(string playerName, int amount)
    {
        currentBidAmount = amount;
        currentBidPlayerName = playerName;
        _bidText.text = "Current Bid:";
        _bidAmount.text = string.Format("{0}: ${1}", currentBidPlayerName, currentBidAmount);
    }


    public void LeaveAuction()
    {
        _interactPanel.gameObject.SetActive(false);
        base.photonView.RPC("RPC_LeaveAuction", RpcTarget.All, player.Name);
    }


    [PunRPC]
    public void RPC_LeaveAuction(string playerName)
    {
        biddingPlayers.Remove(playerName);
        Debug.Log(playerName + " removed from Auction");
        if (biddingPlayers.Count == 1)
        {
            Debug.Log("Last player left in auction");
            if (currentBidPlayerName != "" && currentBidAmount >= 10 && (currentBidPlayerName == biddingPlayers[0] || currentBidPlayerName == playerName))
            {
                Game.PlayerWonAuction(Game.GetPlayer(currentBidPlayerName), tile, currentBidAmount);
                Debug.Log(currentBidPlayerName + " bought " + tile.name + " in Auction for $" + currentBidAmount);
            }
        }
        if (biddingPlayers.Count == 0)
        {
            Debug.Log("Nobody bought " + tile.name + " in Auction.");
            Game.EndAuction();
        }
    }
}
