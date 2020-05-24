using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class SettingBoard : MonoBehaviourPunCallbacks
{
    static public string levelSetting;
    public Toggle easy;
    public Toggle medium;
    public Toggle hard;
    public GameObject toggleBoard;


    public void Awake()
    {
        
    }


    public void Start()
    {
        
    }


    public void Active()
    {
        levelSetting = "easy";
        base.photonView.RPC("RPC_SetLevel", RpcTarget.Others, levelSetting);
    }


    public void ActiveToggle(){
        if( easy.isOn){
            Debug.Log("level : Easy ");
            levelSetting ="easy";
        }
        if( medium.isOn){
            Debug.Log("level : Medium ");
            levelSetting ="medium";
        }
        if( hard.isOn){
            Debug.Log("level : Hard ");
            levelSetting ="hard";
        }
        base.photonView.RPC("RPC_SetLevel", RpcTarget.Others, levelSetting);
    }


    [PunRPC]
    private void RPC_SetLevel(string difficulty)
    {
        levelSetting = difficulty;
    }


    // Update is called once per frame
    public void OnClick_OK(){
        ActiveToggle();
        toggleBoard.SetActive(false);      
    }


}
