using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MonopolyGame;

public class PlayerTurnMenu : MonoBehaviour
{
    [SerializeField]
    private Transform UIPanel;
    [SerializeField]
    private Transform ManageUIPanel;
    [SerializeField]
    private Text _text;
    [SerializeField]
    private ManagePropertyMenu managePropertyMenu;
    [SerializeField]
    private Button managePropertiesButton;
    [SerializeField]
    private Button tradeButton;
    [SerializeField]
    private Button endTurnButton;


    public void Awake()
    {
        
    }


    void Start()
    {
        EventManager.onEndTurn += EndTurn;
        ManageUIPanel.gameObject.SetActive(false);
    }


    void Update()
    {
        if (Game.GetLocalPlayer() != null)
        {
            if (Game.IsLocalPlayerCurrent())
            {
                endTurnButton.gameObject.SetActive(true);
                tradeButton.gameObject.SetActive(true);
            }
            else
            {
                endTurnButton.gameObject.SetActive(false);
                tradeButton.gameObject.SetActive(false);
            }
            if (Game.GetLocalPlayer().inGame == false)
                UIPanel.gameObject.SetActive(false);
        }
    }


    public void ManageProperties()
    {
        // Open Manage Properties UI
        if (ManageUIPanel.gameObject.activeSelf)
            ManageUIPanel.gameObject.SetActive(false);
        else
        {
            ManageUIPanel.gameObject.SetActive(true);
            managePropertyMenu.GetPlayerProperties();
        }
    }


    public void Trade()
    {
        // Open Trade UI
        Game.Trade();
    }


    // handled in EventManager
    public void EndTurn()
    {
        Debug.Log("PlayerTurnMenu.EndTurn() called");
        if (Game.IsLocalPlayerCurrent())
        {
            endTurnButton.gameObject.SetActive(true);
            tradeButton.gameObject.SetActive(true);
        }
        else
        {
            endTurnButton.gameObject.SetActive(false);
            tradeButton.gameObject.SetActive(false);
        }
        ManageUIPanel.gameObject.SetActive(false);
    }


    void OnDisable()
    {
        EventManager.onEndTurn -= EndTurn;
    }
}