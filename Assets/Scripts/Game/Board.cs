using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


namespace MonopolyGame
{
    public class Board : SingletonBehaviour<Board>
    {
        public static List<Tile> tileList = new List<Tile>();
        public static TileColorGroup brownTileList;
        public static TileColorGroup lightBlueTileList;
        public static TileColorGroup pinkTileList;
        public static TileColorGroup orangeTileList;
        public static TileColorGroup redTileList;
        public static TileColorGroup yellowTileList;
        public static TileColorGroup greenTileList;
        public static TileColorGroup darkBlueTileList;
        public static List<int> chanceCardDeck = new List<int>();
        public static List<int> communityChestCardDeck = new List<int>();
        public static List<Card> chanceCardList = new List<Card>();
        public static List<Card> communityChestCardList = new List<Card>();
        public static int chanceCardCount;
        public static int communityChestCardCount;
        public static int chanceCardsPosition;
        public static int communityChestCardsPosition;


        private void Awake()
        {
            var tilesStrings = Resources.Load<TextAsset>("JSON/TileDefinitions");
            foreach (var tileString in tilesStrings.text.Split('\n'))
            {
                var tile = Tile.CreateFromJSON(tileString);
                tileList.Add(tile);
                //Debug.Log("added tile id: " + tile.id + " name: " + tile.name);
                brownTileList = new TileColorGroup("brown");
                lightBlueTileList = new TileColorGroup("lightBlue");
                pinkTileList = new TileColorGroup("pink");
                orangeTileList = new TileColorGroup("orange");
                redTileList = new TileColorGroup("red");
                yellowTileList = new TileColorGroup("yellow");
                greenTileList = new TileColorGroup("green");
                darkBlueTileList = new TileColorGroup("darkBlue");
                switch (tile.colorGroup)
                {
                    case "brown":
                        brownTileList.Add(tile);
                        break;
                    case "lightBlue":
                        lightBlueTileList.Add(tile);
                        break;
                    case "pink":
                        pinkTileList.Add(tile);
                        break;
                    case "orange":
                        orangeTileList.Add(tile);
                        break;
                    case "red":
                        redTileList.Add(tile);
                        break;
                    case "yellow":
                        yellowTileList.Add(tile);
                        break;
                    case "green":
                        greenTileList.Add(tile);
                        break;
                    case "darkBlue":
                        darkBlueTileList.Add(tile);
                        break;
                    default:
                        break;
                }
            }
            var chanceStrings = Resources.Load<TextAsset>("JSON/ChanceCardDefinitions");
            foreach (var cardString in chanceStrings.text.Split('\n'))
            {
                var card = Card.CreateFromJSON(cardString);
                chanceCardList.Add(card);
            }
            var communityChestStrings = Resources.Load<TextAsset>("JSON/CommunityChestCardDefinitions");
            foreach (var cardString in communityChestStrings.text.Split('\n'))
            {
                var card = Card.CreateFromJSON(cardString);
                communityChestCardList.Add(card);
            }
            chanceCardCount = 16;
            communityChestCardCount = 16;
            chanceCardsPosition = 0;
            communityChestCardsPosition = 0;
            for (int i = 0; i < 16; i++)
            {
                chanceCardDeck.Add(i);
                communityChestCardDeck.Add(i);
            }
            ShuffleDeck(chanceCardDeck);
            ShuffleDeck(communityChestCardDeck);
        }


        public static List<Tile> TileList()
        {
            return tileList;
        }


        public static Tile GetTileById(int id)
        {
            Debug.Log("tile " + id + " is " + tileList[id].name);
            return tileList[id];
        }


        public static List<Tile> GetColorGroupTileList(string colorGroup)
        {
            switch (colorGroup)
            {
                case "brown":
                    return brownTileList.tileListing;
                case "lightBlue":
                    return lightBlueTileList.tileListing;
                case "pink":
                    return pinkTileList.tileListing;
                case "orange":
                    return orangeTileList.tileListing;
                case "red":
                    return redTileList.tileListing;
                case "yellow":
                    return yellowTileList.tileListing;
                case "green":
                    return greenTileList.tileListing;
                case "darkBlue":
                    return darkBlueTileList.tileListing;
                default:
                    return null;
            }
        }


        public static void ShuffleDeck(List<int> deck)
        {
            for (int i = 0; i < 16; i++)
            {
                int temp = deck[i];
                int rand = Random.Range(i, 16);
                deck[i] = deck[rand];
                deck[rand] = temp;
            }
        }


        public static int DrawChanceCard()
        {
            int card = chanceCardDeck[chanceCardsPosition];
            chanceCardsPosition++;
            if (chanceCardsPosition > 15)
            {
                ShuffleDeck(chanceCardDeck);
                chanceCardsPosition = 0;
            }
            return card;
        }


        public static int DrawCommunityChestCard()
        {
            int card = communityChestCardDeck[communityChestCardsPosition];
            communityChestCardsPosition++;
            if (communityChestCardsPosition > 15)
            {
                ShuffleDeck(communityChestCardDeck);
                communityChestCardsPosition = 0;
            }
            return card;
        }


        public static Card GetChanceCard(int cardNumber)
        {
            return chanceCardList[cardNumber];
        }


        public static Card GetCommunityChestCard(int cardNumber)
        {
            return communityChestCardList[cardNumber];
        }


        public static void CheckMonopoly()
        {
            int count, num;
            bool monopoly;
            string name = brownTileList.tileListing[0].ownerName;
            if (name != null)
            {
                count = brownTileList.tileListing.Count;
                num = 0;
                foreach (Tile tile in brownTileList.tileListing)
                {
                    if (tile.owned && tile.ownerName == name)
                        num++;
                }
                if (num == count)
                {
                    brownTileList.monopoly = true;
                }
                else
                    brownTileList.monopoly = false;
            }
        }


        public class TileColorGroup
        {
            public string color;
            public bool monopoly;
            public string monopolyPlayerName;
            public List<Tile> tileListing;


            public TileColorGroup(string colorName)
            {
                color = colorName;
                tileListing = new List<Tile>();
                monopoly = false;
            }


            public void Add(Tile tile)
            {
                tileListing.Add(tile);
            }


            public void Remove(Tile tile)
            {
                tileListing.Add(tile);
            }
        }
    }
}