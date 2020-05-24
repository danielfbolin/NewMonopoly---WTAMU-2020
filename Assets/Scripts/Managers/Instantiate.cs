using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MonopolyGame;

public class Instantiate : MonoBehaviour
{
    
    private Vector3 position;
    private void Awake()
    {
        //string difficulty = (string)PhotonNetwork.CurrentRoom.CustomProperties["Difficulty"];
        string difficulty = SettingBoard.levelSetting;
        Debug.Log("from INstantiate class, level is : " + difficulty);
        string token = (string)PhotonNetwork.LocalPlayer.CustomProperties["Token"];
        switch (token)
        {
            case "Cube":
                position = new Vector3(-9.723f, 0.35f, 1f);
                PhotonNetwork.Instantiate("Cube", position, Quaternion.identity);
                Debug.Log("instantiated " + token);
                break;
            case "Sphere":
                position = new Vector3(-9.723f, 0.35f, 0.5f);
                PhotonNetwork.Instantiate("Sphere", position, Quaternion.identity);
                Debug.Log("instantiated " + token);
                break;
            case "Capsule":
                position = new Vector3(-10.223f, 0.35f, 0.5f);
                PhotonNetwork.Instantiate("Capsule", position, Quaternion.identity);
                Debug.Log("instantiated " + token);
                break;
            case "Cylinder":
                position = new Vector3(-10.223f, 0.35f, 1f);
                PhotonNetwork.Instantiate("Cylinder", position, Quaternion.identity);
                Debug.Log("instantiated " + token);
                break;
        }
        GameConfig.SetDifficulty(difficulty);
    }


    private void Start()
    {
        
    }
}
