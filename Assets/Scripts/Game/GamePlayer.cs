using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using System.Text;


namespace MonopolyGame
{
    public class GamePlayer
    {
        public int Player_Id;
        public string Name;
        public List<Tile> Properties = new List<Tile>();
        public int Money;
        public int FullWorth;

        public bool inGame = true;
        public bool inJail = false;
        public bool isLocalPlayer = false;
        public bool isCurrentPlayer = false;
        public int PropertyCount;
        public int JailPasses;

        //deleted the token from the instance variables and added this constructor - M.C.
        public GamePlayer(string name, int id)
        {
            Player_Id = id;
            Name = name;
            //Properties = null;
            // we shouldn't set roperties to null because it already has an empty array of 30 to hold which tile they own
            Money = GameConfig.START_MONEY;
            FullWorth = Money;
            PropertyCount = 0;
            JailPasses = 0;
            Debug.Log("initialized player: " + Name + ", Money: " + Money);
        }


        public int GetMoney()
        {
            return Money;
        }


        public void SetMoney(int value)
        {
            Money = value;
            UpdateFullWorth();
        }


        public void AddMoney(int value)
        {
            Money += value;
            UpdateFullWorth();
        }


        public void SubMoney(int value)
        {
            Money -= value;
            UpdateFullWorth();
        }


        public void AddJailPass()
        {
            JailPasses++;
        }


        public void RemoveJailPass()
        {
            JailPasses--;
        }


        public List<Tile> GetProperties()
        {
            return Properties;
        }


        public void SetProperties(List<Tile> newProperties)
        {
            Properties = new List<Tile>(newProperties);
            UpdateFullWorth();
        }


        public void AddProperty(Tile newTile)
        {
            Properties.Add(newTile);
            UpdateFullWorth();
        }


        public List<Tile> AddProperties(List<Tile> newProperties)
        {
            foreach (Tile tile in newProperties)
            {
                Properties.Add(tile);
                PropertyCount++;
            }
            UpdateFullWorth();
            return Properties;
        }


        //adding the option to remove properties for trading
        //last updated 4/29/2020
        //- M.C.
        public void RemoveProperty(Tile oldTile)
        {
            Properties.Remove(oldTile);
            PropertyCount--;
            UpdateFullWorth();
        }


        //this will remove a list of properties
        public void RemoveProperties(List<Tile> oldProperties)
        {
            foreach (Tile tile in oldProperties)
            {
                Properties.Remove(tile);
                PropertyCount--;
            }
            UpdateFullWorth();
        }


        public bool CheckProperties()
        {
            int index;
            foreach (Tile tile in Board.TileList())
            {
                if (tile.owned && tile.ownerName == Name)
                {
                    index = Properties.FindIndex(x => x.id == tile.id);
                    if (index < 0)
                        return false;
                }
            }
            foreach (Tile tile in Properties)
            {
                if (!tile.owned || tile.ownerName != Name)
                {
                    return false;
                }
            }
            return true;
        }


        public void UpdateProperties()
        {
            PropertyCount = 0;
            Properties.Clear();
            foreach (Tile tile in Board.TileList())
            {
                if (tile.owned && tile.ownerName == Name)
                {
                    Properties.Add(tile);
                    PropertyCount++;
                }
            }
            Board.CheckMonopoly();
            UpdateFullWorth();
        }


        public void UpdateFullWorth()
        {
            FullWorth = Money;
            foreach (Tile tile in Properties)
            {
                FullWorth += tile.GetTileValue();
            }
        }


        public int GetFullWorth()
        {
            return FullWorth;
        }


        public void GoBankrupt()
        {
            foreach (Tile tile in Properties)
            {
                tile.ClearProperty();
                Properties.Remove(tile);
            }
            Money = 0;
            PropertyCount = 0;
            inGame = false;
            JailPasses = 0;
            FullWorth = 0;
            isCurrentPlayer = false;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Money: $" + Money + "\n");
            if (JailPasses > 0)
                sb.Append("Jail Passes: " + JailPasses + "\n");
            return sb.ToString();
        }
    }
}