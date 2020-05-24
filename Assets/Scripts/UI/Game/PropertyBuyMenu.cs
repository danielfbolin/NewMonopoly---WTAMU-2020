using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MonopolyGame;

public class PropertyBuyMenu : MonoBehaviourPun
{
    [SerializeField]
    private Transform UIPanel;
    [SerializeField]
    private Text _propertyName;
    [SerializeField]
    private Text _propertyInfo;
    private Tile tile;
    [SerializeField]
    private GameManager gameManager;

    // Change Color of properties when player purchase 
    string token;
    public GameObject ColorParent;
    //==============
    public void Activate(int tile_id)
    {
        tile = Board.GetTileById(tile_id);
        if (tile.purchasable)
        {
            _propertyName.text = tile.name;
            _propertyInfo.text = tile.ToString();

            UIPanel.gameObject.SetActive(true);
        }
    }


    void Awake()
    {
        UIPanel.gameObject.SetActive(false);
        token = (string)PhotonNetwork.LocalPlayer.CustomProperties["Token"];
    }


    void Start()
    {
        EventManager.onEndTurn += EndTurn;
    }


    public void PurchaseProperty()
    {
        bool purchased = Game.PlayerPurchaseProperty(tile);
        if (purchased){
            UIPanel.gameObject.SetActive(false);
            //base.photonView.RPC("RPC_ChangeColor", RpcTarget.All, tile.id, token);
            RPC_ChangeColor(tile.id, token);
        }
    }


    public void AuctionProperty()
    {
        gameManager.AuctionProperty(tile);
        UIPanel.gameObject.SetActive(false);
    }


    public void EndTurn()
    {
        UIPanel.gameObject.SetActive(false);
    }


    void OnDisable()
    {
        EventManager.onEndTurn -= EndTurn;
    }

    //[PunRPC]
    void RPC_ChangeColor(int number, string token){
        Transform t = ColorParent.transform.GetChild(number);
        t.gameObject.SetActive(true);
        GameObject child = ColorParent.transform.GetChild(number).gameObject;

        switch(token)
        {
            case "Cube":
                child.GetComponent<Renderer>().material.color = Color.red;
                break;
            case "Sphere":
                child.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case "Capsule":
                child.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case "Cylinder":
                
                break;
        }


    }
}