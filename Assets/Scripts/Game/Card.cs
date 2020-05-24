using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using System.Text;

namespace MonopolyGame
{
    [System.Serializable]
    public class Card
    {
        // All Cards
        public int id;
        public string cardType;
        public string description;
        public string actionType;
        public string toTile;
        public int amount;
        // Chance Cards
        public string toTileType;


        public static Card CreateFromJSON(string jsonString)
        {
            Card card = JsonUtility.FromJson<Card>(jsonString);
            return card;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string formatString;
            switch (cardType)
            {
                default:
                    break;
            }
            return sb.ToString();
        }
    }
}