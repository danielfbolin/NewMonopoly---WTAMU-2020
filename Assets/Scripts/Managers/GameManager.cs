using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace MonopolyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        Transform pausePanel; //Will assign our panel to this variable so we can enable/disable it
        [SerializeField]
        private PropertyBuyMenu buyMenu;
        [SerializeField]
        private AuctionMenu auctionMenu;
        [SerializeField]
        private Transform auctionMenuPanel;
        [SerializeField]
        private PayDebtMenu payDebtMenu;
        [SerializeField]
        private Transform gameOverPanel;
        [SerializeField]
        private Text winnerText;

        bool isPaused; //Used to determine paused state


        void Start()
        {
            gameOverPanel.gameObject.SetActive(false);
            pausePanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts
            auctionMenuPanel.gameObject.SetActive(false);
            payDebtMenu.gameObject.SetActive(false);
            isPaused = false; //make sure isPaused is always false when our scene opens
        }


        void Update()
        {

            //If player presses escape and game is not paused. Pause game. If game is paused and player presses escape, unpause.
            if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
                Pause();
            else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
                UnPause();
            if (payDebtMenu.gameObject.activeSelf)
                if (Game.GetLocalPlayer().inGame == false)
                    payDebtMenu.gameObject.SetActive(false);
        }


        public void PlayerLandedOnTile(int tileId)
        {
            Tile tile = Board.GetTileById(tileId);
            Debug.Log(tile.name + ": " + tile.id);
            if (tile.purchasable)
            {
                // Properties, Utilities, Railroads
                if (tile.owned)
                {
                    Game.PlayerPaysRent(tile);
                }
                else if (tile.tileType == "property" || tile.tileType == "utility" || tile.tileType == "railroad")
                    buyMenu.Activate(tileId);
            }
            else
            {
                //Tax
                if (tile.tileType == "tax")
                    Game.PlayerPaysTax(tile);
            }

        }


        public void PlayerDrawChanceCard(int cardNumber)
        {
            Card card = Board.GetChanceCard(cardNumber);
            switch (card.actionType)
            {
                case "collect":
                    Game.PlayerCollect(card.amount);
                    break;
                case "payment":
                    Game.PlayerPayment(card.amount);
                    break;
                case "payRepairs":
                    Game.PlayerPaysRepairs(25, 100);
                    break;
                case "payPlayers":
                    Game.PlayerPaysPlayers(card.amount);
                    break;
                case "jailPass":
                    Game.PlayerGetsJailPass();
                    break;
                case "advance":

                    break;
                case "goToJail":

                    break;
                case "backThree":

                    break;
            }
        }


        public void PlayerDrawCommunityChestCard(int cardNumber)
        {
            Card card = Board.GetCommunityChestCard(cardNumber);
            switch (card.actionType)
            {
                case "collect":
                    Game.PlayerCollect(card.amount);
                    break;
                case "payment":
                    Game.PlayerPayment(card.amount);
                    break;
                case "payRepairs":
                    Game.PlayerPaysRepairs(40, 115);
                    break;
                case "collectPlayers":
                    Game.PlayerCollectsFromPlayers(card.amount);
                    break;
                case "jailPass":
                    Game.PlayerGetsJailPass();
                    break;
                case "advance":

                    break;
                case "goToJail":

                    break;
            }
        }


        public void PlayerPassedGo()
        {
            Game.PlayerCollect(200);
            Debug.Log(Game.GetLocalPlayer().Name + " passed go and collected $" + 200);
        }


        public void PlayerLandedOnCorner(int tileId)
        {
            // Jail, Free Parking, Go To Jail, Go
        }


        public void AuctionProperty(Tile tile)
        {
            auctionMenuPanel.gameObject.SetActive(true);
            auctionMenu.Activate(tile);
            base.photonView.RPC("RPC_AuctionProperty", RpcTarget.Others, tile.id);
        }


        [PunRPC]
        public void RPC_AuctionProperty(int tileId)
        {
            if (Game.GetLocalPlayer().inGame)
            {
                auctionMenuPanel.gameObject.SetActive(true);
                auctionMenu.Activate(Board.GetTileById(tileId));
            }
        }


        public void EndAuction()
        {
            auctionMenuPanel.gameObject.SetActive(false);
        }


        public void PlayerDebt(int amount)
        {
            payDebtMenu.gameObject.SetActive(true);
            payDebtMenu.SetDebt(amount);
        }


        public void GameOver(string playerName)
        {
            base.photonView.RPC("RPC_GameOver", RpcTarget.All, playerName);
        }


        [PunRPC]
        private void RPC_GameOver(string playerName)
        {
            if (Game.GetLocalPlayer().Name == playerName)
            {
                winnerText.text = "Congratulations " + playerName + ", you won!";
            }
            else
            {
                winnerText.text = "You have gone bankrupt!\n" + playerName + " has won the game!";
            }
            gameOverPanel.gameObject.SetActive(true);
        }


        public void Pause()
        {
            isPaused = true;
            pausePanel.gameObject.SetActive(true); //turn on the pause menu
        }


        public void UnPause()
        {
            isPaused = false;
            pausePanel.gameObject.SetActive(false); //turn off pause menu
        }


        public void QuitGame()
        {
            Application.Quit();
        }


        public void Restart()
        {
            Application.LoadLevel(1);
        }
    }
}