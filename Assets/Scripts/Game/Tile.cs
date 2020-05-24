using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System;

/*
 * for most actions involving a property, the game must check if player is allowed to do the action and then call these methods
 */


namespace MonopolyGame
{
    [System.Serializable]
    public class Tile
    {
        // All Tiles
        public int id;
        public string name;
        public string tileType;
        public string description;
        public bool purchasable;
        // Properties + Railroads + Utilities
        public bool buildable; // can build houses/hotels
        public bool owned;
        public bool mortgaged;
        public bool monopoly;
        public string ownerName;
        public int purchasePrice;
        public int mortgagePrice;
        public int mortgageRate;
        public int[] rent;
        public int rentPrice;
        public int currentValue;
        // Properties
        public int houseCost;
        public int hotelCost;
        public int houses;
        public int hotels;
        public string colorGroup;
        // Railroads

        // Utilities

        // Corners
        public int salary;
        // Tax


        public static Tile CreateFromJSON(string jsonString)
        {
            // Given JSON input:
            // {"id":1,"name":"Mediterranean Avenue","tileType":"property","description":"","purchasePrice":60,"mortgagePrice":30,"houseCost":50,"hotelCost":250,"colorGroup":"brown"}
            // this example will return a Tile object with
            // id == 1, name == "Mediterranean Avenue", tileType == "property", "cardType": "chance", "description == "", purchasePrice == 60, mortgagePrice == 30, houseCost == 50, hotelCost == 250, colorGroup == "brown"
            Tile tile = JsonUtility.FromJson<Tile>(jsonString);
            tile.TileInit();
            return tile;
        }


        private void TileInit()
        {
            switch (tileType)
            {
                case "property":
                    purchasable = true;
                    buildable = true;
                    owned = false;
                    mortgaged = false;
                    monopoly = false;
                    houses = 0;
                    hotels = 0;
                    rentPrice = RentMult(rent[0]);
                    currentValue = purchasePrice;
                    break;
                case "railroad":
                    purchasable = true;
                    buildable = false;
                    rentPrice = RentMult(rent[0]);
                    currentValue = purchasePrice;
                    break;
                case "utility":
                    purchasable = true;
                    buildable = false;
                    currentValue = purchasePrice;
                    break;
                case "corner":
                    purchasable = false;
                    break;
                case "cardDraw":
                    purchasable = false;
                    break;
                case "tax":
                    purchasable = false;
                    break;
                default:
                    break;
            }
        }


        /*
         * Assuming game allows player to purchase the property and has already taken money from player
         */
        public void SetOwner(string playerName)
        {
            ownerName = playerName;
            owned = true;
        }


        public void RemoveOwner()
        {
            ownerName = null;
            owned = false;
        }


        public bool IsOwner(string playerName)
        {
            return (playerName == ownerName);
        }


        public int RentMult(int rent)
        {
            return (int)Math.Round(rent*GameConfig.RENT_MULT);
        }


        /*
         * UpdateRent is called after actions happen involving the property
         */
        public void UpdateRent()
        {
            switch (tileType)
            {
                case "property":
                    if (hotels == 1)
                        rentPrice = rent[6];
                    else if (houses > 0)
                        rentPrice = rent[houses + 1];
                    else if (monopoly)
                        rentPrice = rent[1];
                    else
                        rentPrice = rent[0];
                    break;
                case "railroad":
                    // check how many railroads are owned by player
                    // rentPrice is 25 times number of railroads owned
                    if (monopoly)
                        rentPrice = rent[0] * 4;
                    break;
                    // utility rent price is dependent on player's dice roll
                    // 4 times dice roll, or if monopoly 10 times dice roll
            }
            rentPrice = RentMult(rentPrice);
        }


        public int GetRent()
        {
            if (tileType == "utility")
            {
                //get last dice roll value and multiply
                return rentPrice;
            }
            return rentPrice;
        }


        public void Mortgage()
        {
            // game needs to check that no buildings are on property

            if (!mortgaged)
                mortgaged = true;
            UpdateCurrentValue();
        }


        public void Unmortgage()
        {
            // game needs to check that player has enough money (purchasePrice + 10%)
            if (mortgaged)
            {
                mortgaged = false;
                UpdateCurrentValue();
            }
        }


        public void AddHouse()
        {
            // game should check if player has enough money, not this method
            // game must check for even distribution of buildings among color group
            // game must check that bank has a house available
            // game must check that property is not mortgaged
            
            houses++;
            UpdateRent();
            UpdateCurrentValue();
            // subtract house from bank
        }


        public void RemoveHouse()
        {
            houses--;
            UpdateRent();
            UpdateCurrentValue();
            // add house back to bank
        }


        public void AddHotel()
        {
            // game should check if player has enough money, not this method
            // game must check for even distribution of buildings among color group
            // game must check that bank has a hotel available
            // game must check that property is not mortgaged

            // buying hotel removes 4 houses

            // no more than 1 hotel
            if (hotels < 1)
            {
                hotels++;
                houses = 0;
            }
            UpdateRent();
            UpdateCurrentValue();
            // subtract hotel from bank
        }

        
        public void RemoveHotel()
        {
            // selling hotel will give player amount of (houseCost / 2)
            // houses goes back up to 4

            if (hotels > 0)
            {
                hotels--;
                houses = 4;
            }
            UpdateRent();
            UpdateCurrentValue();
            // add hotel back to bank
        }


        public void ClearProperty()
        {
            switch (tileType)
            {
                case "property":
                    owned = false;
                    ownerName = null;
                    mortgaged = false;
                    monopoly = false;
                    houses = 0;
                    hotels = 0;
                    rentPrice = RentMult(rent[0]);
                    currentValue = purchasePrice;
                    break;
                case "railroad":
                    rentPrice = RentMult(rent[0]);
                    currentValue = purchasePrice;
                    break;
                case "utility":
                    currentValue = purchasePrice;
                    break;
                default:
                    break;
            }
        }


        public int GetPrice()
        {
            if (tileType == "tax")
                return (int)Math.Round(purchasePrice * GameConfig.TAX_MULT);
            if (!mortgaged)
                return purchasePrice;
            // to buy a mortgaged property costs 110% of mortgaged price
            return (int)(mortgagePrice * GameConfig.INTEREST_MULT);
        }


        public void UpdateCurrentValue()
        {
            if (!purchasable)
                return;    // shouldn't happen unless broken
            if (mortgaged)
                currentValue = mortgagePrice;   // mortgaged so only mortgage value
            currentValue = 0;
            switch (tileType)
            {
                case "property":
                    currentValue += (houses + hotels) * houseCost;   // properties have houses and hotels
                    currentValue += purchasePrice;
                    break;
                default:
                    currentValue += purchasePrice;   // all purchasable properties have a value
                    break;
            }
            Debug.Log(name + " has a value of $" + currentValue);
        }


        public int GetTileValue()
        {
            return currentValue;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string formatString;
            switch (tileType)
            {
                case "property":
                    formatString = "{0,-15} ${1,4}\n" +
                                          "{2,-15} ${3,4}\n" +
                                          "{4,-15} ${5,4}\n" +
                                          "{6,-15} ${7,4}\n" +
                                          "{8,-15} ${9,4}\n" +
                                          "{10,-15} ${11,4}\n" +
                                          "{12,-15} ${13,4}\n\n" +
                                          "{14} ${15}\n" +
                                          "{16} ${17} {18}\n" +
                                          "{19} ${20} {21}";
                    sb.AppendFormat(formatString, "Cost", purchasePrice,
                                                  "Rent", RentMult(rent[0]),
                                                  "With 1 House", RentMult(rent[2]),
                                                  "With 2 Houses", RentMult(rent[3]),
                                                  "With 3 Houses", RentMult(rent[4]),
                                                  "With 4 Houses", RentMult(rent[5]),
                                                  "With Hotel", RentMult(rent[6]),
                                                  "Mortgage Value", mortgagePrice,
                                                  "Houses Cost", houseCost, "Each",
                                                  "Hotels", houseCost, "Plus 4 Houses");
                    break;
                case "railroad":
                    formatString = "{0,-15} ${1,4}\n" +
                                          "{2,-15} ${3,4}\n" +
                                          "{4,-15} ${5,4}\n" +
                                          "{6,-15} ${7,4}\n" +
                                          "{8,-15} ${9,4}\n\n" +
                                          "{10} ${11}";
                    sb.AppendFormat(formatString, "Cost", purchasePrice,
                                                  "Rent", RentMult(rent[0]),
                                                  "If 2 R.R.'s Are Owned", RentMult(rent[0] * 2),
                                                  "If 3 R.R.'s Are Owned", RentMult(rent[0] * 3),
                                                  "If 4 R.R.'s Are Owned", RentMult(rent[0] * 4),
                                                  "Mortgage Value", mortgagePrice);
                    break;
                case "utility":
                    formatString = "{0,-15} ${1,4}\n" +
                                          "{2}\n" +
                                          "{3}\n\n" +
                                          "{4} ${5}";
                    sb.AppendFormat(formatString, "Cost", purchasePrice,
                                                  "If one \"Utility\" is owned rent is 4 times amount shown on dice.",
                                                  "If both \"Utilities\" are owned rent is 10 times amount shown on dice.",
                                                  "Mortgage Value", mortgagePrice);
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }
    }
}