/* 
 * Started on 4/15/2020
 * Last Updated 4/26/2020
 * 
 * 
 * ************************How this class works so far*****************************************
 * This class at the moment makes a list of GamePlayer objects using the Player objects
 * in PhotonNetwork.CurrentRoom.  We can now use and edit these GamePlayers in the update
 * function, and we can write subroutines to handle things for us.  The GamePlayers each
 * have money, properties, and a name.  The turn order is decided based on how they are
 * listed in the room.
 * ********************************************************************************************
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;


namespace MonopolyGame
{
    public class Game : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Transform _content;
        private static GameManager gameManager;
        private static GamePlayer currentPlayer;
        private static GamePlayer localPlayer;
        private static int currentIndex;
        private static int playerCount;
        private static bool playerCanEndTurn;
        private static string levelDifficulty;
        private static List<GamePlayer> TurnListing = new List<GamePlayer>();
        private static string payDebtToPlayer;


        void Awake()
        {

        }


        public override void OnEnable()
        {
            base.OnEnable();
            playerCount = 0;
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
                {
                    base.photonView.RPC("RPC_AddPlayer", RpcTarget.All, player.Value.NickName);
                }
                Debug.Log("Turn order has been made");
            }
            gameManager = GameObject.FindObjectOfType<GameManager>();
            playerCanEndTurn = true;
        }


        // Start is called before the first frame update
        void Start()
        {

        }


        // Update is called once per frame
        void Update()
        {
        
        }
        

        public static List<GamePlayer> GetPlayers()
        {
            return TurnListing;
        }


        public static GamePlayer GetPlayer(string name)
        {
            GamePlayer player = TurnListing.Find(x => x.Name == name);
            if (player != null)
                return player;
            Debug.LogError("Player not found " + name);
            return null;
        }


        public static GamePlayer GetCurrentPlayer()
        {
            return currentPlayer;
        }


        public static GamePlayer GetLocalPlayer()
        {
            return localPlayer;
        }


        public static bool IsCurrentPlayer(GamePlayer player)
        {
            return (player.isCurrentPlayer);
        }
        

        public static bool IsLocalPlayerCurrent()
        {
            return localPlayer.isCurrentPlayer;
        }


        [PunRPC]
        public void RPC_SetTurnListing(List<GamePlayer> list)
        {
            TurnListing = list;
        }


        [PunRPC]
        public void RPC_AddPlayer(string name)
        {
            TurnListing.Add(new GamePlayer(name, playerCount));
            Debug.Log("player added: " + TurnListing.Last().Name);
            if (playerCount == 0)
            {
                TurnListing.Last().isCurrentPlayer = true;
                currentPlayer = TurnListing.Last();
                currentIndex = 0;
                Debug.Log(PhotonNetwork.LocalPlayer.NickName + ": current player is " + currentPlayer.Name);
            }
            if (PhotonNetwork.LocalPlayer.NickName == name)
            {
                TurnListing.Last().isLocalPlayer = true;
                localPlayer = TurnListing.Last();
                Debug.Log(PhotonNetwork.LocalPlayer.NickName + ": local player is " + localPlayer.Name);
            }
            playerCount++;
        }


        [PunRPC]
        public void RPC_SetCurrentPlayer()
        {
            currentIndex = ((currentIndex + 1) % playerCount);
            currentPlayer.isCurrentPlayer = false;
            currentPlayer = TurnListing[currentIndex];
            TurnListing[currentIndex].isCurrentPlayer = true;
        }


        public static void PlayerGoToJail()
        {
            Debug.Log(currentPlayer.Name + " is going to Jail!");
        }


        public void EndTurn()
        {
            // change currentPlayer
            if (playerCanEndTurn)
            {
                Debug.Log("Event: " + currentPlayer.Name + " ended their turn");
                PhotonView.Find(2).RPC("RPC_SetCurrentPlayer", RpcTarget.All);
                Debug.Log("It is " + currentPlayer.Name + "'s turn");
                EventManager.RaiseEndTurn();
            }
        }


        public static bool PlayerPurchaseProperty(Tile tile)
        {
            int price = tile.GetPrice();
            if (localPlayer.GetMoney() < price)
                return false;
            else
            {
                localPlayer.SubMoney(price);
                localPlayer.AddProperty(tile);
                tile.SetOwner(localPlayer.Name);
                PhotonView.Find(2).RPC("RPC_PlayerPurchaseProperty", RpcTarget.Others, localPlayer.Name, tile.id);
                Debug.Log(localPlayer.Name + " purchased " + tile.name + " for $" + price);
                return true;
            }
        }


        [PunRPC]
        private void RPC_PlayerPurchaseProperty(string playerName, int tileId)
        {
            GamePlayer player = GetPlayer(playerName);
            Tile tile = Board.GetTileById(tileId);
            player.SubMoney(tile.GetPrice());
            player.AddProperty(tile);
            tile.SetOwner(playerName);
        }


        public static bool PlayerSellProperty(Tile tile)
        {
            int price = tile.GetPrice();
            localPlayer.AddMoney(price);
            localPlayer.RemoveProperty(tile);
            tile.RemoveOwner();
            PhotonView.Find(2).RPC("RPC_PlayerSellProperty", RpcTarget.Others, localPlayer.Name, tile.id);
            Debug.Log(localPlayer.Name + " sold " + tile.name + " for $" + price);
            return true;
        }


        [PunRPC]
        private void RPC_PlayerSellProperty(string playerName, int tileId)
        {
            GamePlayer player = GetPlayer(playerName);
            Tile tile = Board.GetTileById(tileId);
            player.AddMoney(tile.GetPrice());
            player.RemoveProperty(tile);
            tile.RemoveOwner();
        }


        public static bool PlayerMortgageProperty(Tile tile)
        {
            if (tile.houses > 0 || tile.hotels > 0)
                return false;
            if (tile.mortgaged)
                return false;
            int price = tile.GetPrice();
            localPlayer.AddMoney(tile.mortgagePrice);
            tile.Mortgage();
            PhotonView.Find(2).RPC("RPC_PlayerMortgageProperty", RpcTarget.Others, localPlayer.Name, tile.id);
            return true;
        }


        [PunRPC]
        private void RPC_PlayerMortgageProperty(string playerName, int tileId)
        {
            GamePlayer player = GetPlayer(playerName);
            Tile tile = Board.GetTileById(tileId);
            player.AddMoney(tile.mortgagePrice);
            tile.Mortgage();
        }


        //This function will create a list of players to choose how to trade with as buttons,
        //once a button is selected a different function will be called to choose properties for trading
        //last updated 4/30/2020
        //- M.C.
        public static void Trade()
        {
            Debug.Log("Player is trying to pick a player to trade with");
            //
        }


        public static void SwitchProperties(GamePlayer player1, GamePlayer player2, List<Tile> p1, List<Tile> p2)
        {
            //first remove player 1's properties and add them to player 2's properties
            player1.RemoveProperties(p1);
            player2.AddProperties(p1);

            //now remove player 2's properties and add them to player 1's properties
            player2.RemoveProperties(p2);
            player1.AddProperties(p2);
        }


        public static bool PlayerBuyHouse(Tile tile)
        {
            if (Bank.GetHouses() < 1)
                return false;
            if (tile.houses > 3)
                return false;
            if (localPlayer.GetMoney() < tile.houseCost)
                return false;
            foreach (Tile otherTile in Board.GetColorGroupTileList(tile.colorGroup))
            {
                if (otherTile.ownerName != localPlayer.Name || otherTile.mortgaged)
                    return false;
            }
            localPlayer.SubMoney(tile.houseCost);
            tile.AddHouse();
            Bank.RemoveHouse();
            PhotonView.Find(2).RPC("RPC_PlayerBuyHouse", RpcTarget.Others, localPlayer.Name, tile.id);
            Debug.Log(localPlayer.Name + " purchased a House on " + tile.name);
            return true;
        }


        [PunRPC]
        private void RPC_PlayerBuyHouse(string playerName, int tileId)
        {
            GamePlayer player = GetPlayer(playerName);
            Tile tile = Board.GetTileById(tileId);
            player.SubMoney(tile.houseCost);
            Bank.RemoveHouse();
            tile.AddHouse();
        }


        public static bool PlayerSellHouse(Tile tile)
        {
            if (tile.houses < 1)
                return false;
            localPlayer.AddMoney(tile.houseCost);
            tile.RemoveHouse();
            Bank.AddHouse();
            PhotonView.Find(2).RPC("RPC_PlayerSellHouse", RpcTarget.Others, localPlayer.Name, tile.id);
            Debug.Log(localPlayer.Name + " sold a House on " + tile.name);
            return true;
        }


        [PunRPC]
        private void RPC_PlayerSellHouse(string playerName, int tileId)
        {
            GamePlayer player = GetPlayer(playerName);
            Tile tile = Board.GetTileById(tileId);
            player.AddMoney(tile.houseCost);
            tile.RemoveHouse();
            Bank.AddHouse();
        }


        public static bool PlayerBuyHotel(Tile tile)
        {
            if (Bank.GetHotels() < 1)
                return false;
            if (tile.hotels > 0 && tile.houses == 4)
                return false;
            if (localPlayer.GetMoney() < tile.hotelCost)
                return false;
            foreach (Tile otherTile in Board.GetColorGroupTileList(tile.colorGroup))
            {
                if (otherTile.ownerName != localPlayer.Name || otherTile.mortgaged)
                    return false;
            }
            localPlayer.SubMoney(tile.hotelCost);
            Bank.RemoveHotel();
            tile.AddHotel();
            PhotonView.Find(2).RPC("RPC_PlayerBuyHotel", RpcTarget.Others, localPlayer.Name, tile.id);
            Debug.Log(localPlayer.Name + " purchased a Hotel on " + tile.name);
            return true;
        }


        [PunRPC]
        private void RPC_PlayerBuyHotel(string playerName, int tileId)
        {
            GamePlayer player = GetPlayer(playerName);
            Tile tile = Board.GetTileById(tileId);
            player.SubMoney(tile.hotelCost);
            Bank.RemoveHotel();
            tile.AddHotel();
        }


        public static bool PlayerSellHotel(Tile tile)
        {
            if (tile.hotels == 0)
                return false;
            localPlayer.AddMoney(tile.hotelCost);
            tile.RemoveHotel();
            Bank.AddHotel();
            PhotonView.Find(2).RPC("RPC_PlayerSellHotel", RpcTarget.Others, localPlayer.Name, tile.id);
            Debug.Log(localPlayer.Name + " sold a Hotel on " + tile.name);
            return true;
        }


        [PunRPC]
        private void RPC_PlayerSellHotel(string playerName, int tileId)
        {
            GamePlayer player = GetPlayer(playerName);
            Tile tile = Board.GetTileById(tileId);
            player.AddMoney(tile.hotelCost);
            tile.RemoveHotel();
            Bank.AddHotel();
        }


        public static void PlayerPaysRent(Tile tile)
        {
            if (localPlayer.Name == tile.ownerName)
                return;
            GamePlayer otherPlayer = GetPlayer(tile.ownerName);
            if (tile.mortgaged)
                return;
            int rent = tile.GetRent();
            if (localPlayer.GetMoney() < rent)
                PlayerAwaitFunds(rent, otherPlayer.Name);
            else
            {
                localPlayer.SubMoney(rent);
                otherPlayer.AddMoney(rent);
                playerCanEndTurn = true;
                PhotonView.Find(2).RPC("RPC_PlayerPaysRent", RpcTarget.Others, localPlayer.Name, tile.ownerName, rent);
                Debug.Log(localPlayer.Name + " paid rent of $" + rent + " on " + tile.name);
            }
        }


        [PunRPC]
        private void RPC_PlayerPaysRent(string playerName, string otherPlayerName, int rent)
        {
            GamePlayer player = GetPlayer(playerName);
            GamePlayer otherPlayer = GetPlayer(otherPlayerName);
            player.SubMoney(rent);
            otherPlayer.AddMoney(rent);
        }


        public static void PlayerPaysTax(Tile tile)
        {
            int tax = tile.GetPrice();
            if (localPlayer.GetMoney() < tax)
                PlayerAwaitFunds(tax, "bank");
            else
            {
                localPlayer.SubMoney(tax);
                playerCanEndTurn = true;
                PhotonView.Find(2).RPC("RPC_PlayerPayment", RpcTarget.Others, localPlayer.Name, tax);
                Debug.Log(localPlayer.Name + " paid " + tile.name + " of $" + tax);
            }
        }


        [PunRPC]
        private void RPC_PlayerPayment(string playerName, int amount)
        {
            GamePlayer player = GetPlayer(playerName);
            player.SubMoney(amount);
        }


        // This is called from within a PunRPC method in AuctionMenu
        public static void PlayerWonAuction(GamePlayer player, Tile tile, int price)
        {
            player.SubMoney(price);
            player.AddProperty(tile);
            tile.SetOwner(player.Name);
            playerCanEndTurn = true;
            gameManager.EndAuction();
        }


        public static void EndAuction()
        {
            gameManager.EndAuction();
        }


        public static void PlayerCollect(int amount)
        {
            localPlayer.AddMoney(amount);
            PhotonView.Find(2).RPC("RPC_PlayerCollect", RpcTarget.Others, localPlayer.Name, amount);
        }


        [PunRPC]
        private void RPC_PlayerCollect(string playerName, int amount)
        {
            GamePlayer player = GetPlayer(playerName);
            player.AddMoney(amount);
        }


        public static void PlayerPayment(int amount)
        {
            localPlayer.SubMoney(amount);
            if (localPlayer.GetMoney() < amount)
                PlayerAwaitFunds(amount, "bank");
            else
            {
                PhotonView.Find(2).RPC("RPC_PlayerPayment", RpcTarget.Others, localPlayer.Name, amount);
            }
        }


        public static void PlayerPaysRepairs(int houseCost, int hotelCost)
        {
            int houses = 0;
            int hotels = 0;
            int amount = 0;
            foreach (Tile tile in localPlayer.GetProperties())
            {
                houses += tile.houses;
                hotels += tile.hotels;
            }
            amount += houses * houseCost;
            amount += hotels * hotelCost;
            if (localPlayer.GetMoney() < amount)
                PlayerAwaitFunds(amount, "bank");
            else
            {
                PhotonView.Find(2).RPC("RPC_PlayerPayment", RpcTarget.Others, localPlayer.Name, amount);
            }
        }


        public static void PlayerPaysPlayers(int amount)
        {
            int totalAmount = amount * playerCount;
            if (localPlayer.GetMoney() < totalAmount)
                PlayerAwaitFunds(totalAmount, "allPlayers");
            else
            {
                localPlayer.SubMoney(totalAmount);
                foreach (GamePlayer player in GetPlayers())
                    if (player != localPlayer)
                        player.AddMoney(amount);
                PhotonView.Find(2).RPC("RPC_PlayerPaysPlayers", RpcTarget.Others, localPlayer.Name, amount);
            }
        }


        [PunRPC]
        private void RPC_PlayerPaysPlayers(string playerName, int amount)
        {
            GamePlayer player = GetPlayer(playerName);
            player.SubMoney(amount * playerCount);
            foreach (GamePlayer otherPlayer in GetPlayers())
                if (otherPlayer != player)
                    otherPlayer.AddMoney(amount);
        }


        public static void PlayerCollectsFromPlayers(int amount)
        {
            int totalAmount = amount * playerCount;
            foreach (GamePlayer player in GetPlayers())
                if (player != localPlayer)
                    player.SubMoney(amount);
            localPlayer.AddMoney(totalAmount);
            PhotonView.Find(2).RPC("RPC_PlayerCollectsFromPlayers", RpcTarget.Others, localPlayer.Name, amount);
        }


        [PunRPC]
        private void RPC_PlayerCollectsFromPlayers(string playerName, int amount)
        {
            // Will need to check each localPlayer to see if they have the amount.
            // They will need to be able to sell property while it's not their turn
            //   to make up the required amount.
            GamePlayer player = GetPlayer(playerName);
            foreach (GamePlayer otherPlayer in GetPlayers())
                if (otherPlayer != player)
                    otherPlayer.SubMoney(amount);
            player.AddMoney(amount * playerCount);
        }


        public static void PlayerGetsJailPass()
        {
            localPlayer.AddJailPass();
            PhotonView.Find(2).RPC("RPC_PlayerGetsJailPass", RpcTarget.Others, localPlayer.Name);
        }


        [PunRPC]
        private void RPC_PlayerGetsJailPass(string playerName)
        {
            GamePlayer player = GetPlayer(playerName);
            player.AddJailPass();
        }


        public static void SetEndTurnStatus(bool status)
        {
            playerCanEndTurn = status;
        }


        private static void PlayerAwaitFunds(int amount, string debtToPlayer)
        {
            payDebtToPlayer = debtToPlayer;
            Debug.Log("Player must sell houses/hotels, mortgage or sell properties to make a payment.");
            playerCanEndTurn = false;
            gameManager.PlayerDebt(amount);
        }


        public static void PlayerPayDebt(int amount)
        {
            string debtToPlayer = payDebtToPlayer;
            switch (debtToPlayer)
            {
                case "bank":
                    localPlayer.SubMoney(amount);
                    PhotonView.Find(2).RPC("RPC_PlayerPaysRent", RpcTarget.Others, localPlayer.Name, amount);
                    break;
                case "allPlayers":
                    localPlayer.SubMoney(amount * playerCount);
                    foreach (GamePlayer otherPlayer in GetPlayers())
                        if (otherPlayer != localPlayer)
                            otherPlayer.AddMoney(amount);
                    PhotonView.Find(2).RPC("RPC_PlayerPaysPlayers", RpcTarget.Others, localPlayer.Name, amount);
                    break;
                default:
                    localPlayer.SubMoney(amount);
                    GetPlayer(debtToPlayer).AddMoney(amount);
                    PhotonView.Find(2).RPC("RPC_DebtToPlayer", RpcTarget.Others, localPlayer.Name, debtToPlayer, amount);
                    break;
            }
            // debt needs to be distributed to the correct location, bank or player
            PhotonView.Find(2).RPC("RPC_PlayerPayment", RpcTarget.Others, localPlayer.Name, amount);
            playerCanEndTurn = true;
        }


        public static void PlayerBankrupt()
        {
            Debug.Log(localPlayer.Name + " went bankrupt");
            string playerName = localPlayer.Name;
            int index = (currentIndex + 1) % playerCount;
            string currentPlayerName = TurnListing[index].Name;
            localPlayer.GoBankrupt();
            PhotonView.Find(2).RPC("RPC_PlayerBankrupt", RpcTarget.Others, playerName);
            PhotonView.Find(2).RPC("RPC_RebuildTurnListing", RpcTarget.All, playerName, currentPlayerName);
            EventManager.RaiseEndTurn();
            if (playerCount == 1)
            {
                // game over
                gameManager.GameOver(TurnListing[0].Name);
            }
            else
                Debug.Log(playerCount + " players are left!");
            
            
        }


        [PunRPC]
        private void RPC_RebuildTurnListing(string playerName, string currentPlayerName)
        {
            Debug.Log("removing " + playerName + "from turn order");
            List<GamePlayer> tempListing = new List<GamePlayer>();
            int tempCount = 0;
            foreach (GamePlayer listing in TurnListing)
            {
                if (listing.Name != playerName)
                {
                    tempListing.Add(listing);
                    if (listing.Name == currentPlayerName)
                    {
                        currentIndex = tempCount;
                        tempListing[currentIndex].isCurrentPlayer = true;
                    }
                    else
                        listing.isCurrentPlayer = false;
                    tempCount++;
                }
            }
            TurnListing.Clear();
            TurnListing = tempListing;
            foreach (GamePlayer player in TurnListing)
            {
                Debug.Log(player.Name + " is still in the game");
            }
            playerCount = tempCount;
        }


        [PunRPC]
        private void RPC_PlayerBankrupt(string playerName)
        {
            GamePlayer player = GetPlayer(playerName);
            player.GoBankrupt();
        }


        public override void OnDisable()
        {
            base.OnDisable();
        }
    }
}