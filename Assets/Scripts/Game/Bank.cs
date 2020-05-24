using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


namespace MonopolyGame
{
    public class Bank : MonoBehaviour
    {
        public static int Houses;
        public static int Hotels;

        void Start()
        {
            Houses = 32;
            Houses = 12;
        }


        public static int GetHouses()
        {
            return Houses;
        }


        public static int GetHotels()
        {
            return Hotels;
        }


        public static void RemoveHouse()
        {
            Houses--;
        }


        public static void AddHouse()
        {
            Houses++;
        }


        public static void RemoveHotel()
        {
            Hotels--;
            Houses += 4;
        }


        public static void AddHotel()
        {
            Houses -= 4;
            Hotels++;
        }
    }
}