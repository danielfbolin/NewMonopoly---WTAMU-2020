using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;


namespace MonopolyGame
{
    static class GameConfig
    {
        public static int START_MONEY;
        public static float RENT_MULT;
        public static float TAX_MULT;
        public static float INTEREST_MULT;


        public static void SetDifficulty(string difficulty)
        {
            Debug.Log("Difficulty will be set to " + difficulty);
            switch (difficulty)
            {
                case "easy":
                    START_MONEY = 1500;
                    RENT_MULT = 1.0f;
                    TAX_MULT = 1.0f;
                    INTEREST_MULT = 1.1f;
                    Debug.Log("setting is on easy");
                    break;
                case "medium":
                    START_MONEY = 1500;
                    RENT_MULT = 1.05f;
                    TAX_MULT = 1.05f;
                    INTEREST_MULT = 1.125f;
                    break;
                case "hard":
                    START_MONEY = 1500;
                    RENT_MULT = 1.1f;
                    TAX_MULT = 1.1f;
                    INTEREST_MULT = 1.15f;
                    break;
            }
        }
    }
}