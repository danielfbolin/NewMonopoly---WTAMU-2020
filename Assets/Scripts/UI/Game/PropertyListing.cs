using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using MonopolyGame;
using System.Text;

public class PropertyListing : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Text _text;
    [SerializeField]
    public Graphic image;

    public Tile tile {get; private set;}


    public void SetPropertyInfo(Tile newTile)
    {
        tile = newTile;
        SetPropertyText();
    }


    private void SetPropertyText()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(tile.name + "\n");
        if (tile.buildable)
        {
            if (tile.houses > 0)
                sb.Append("Houses: " + tile.houses + " ");
            if (tile.hotels > 0)
                sb.Append("Hotels: " + tile.hotels);
        }
        _text.text = sb.ToString();
        switch (tile.colorGroup)
        {
            case "brown":
                image.color = new Color32(137, 67, 57, 200);
                break;
            case "lightBlue":
                image.color = new Color32(158, 209, 255, 200);
                break;
            case "pink":
                image.color = new Color32(249, 38, 211, 200);
                break;
            case "orange":
                image.color = new Color32(255, 144, 0, 200);
                break;
            case "red":
                image.color = new Color32(255, 13, 1, 200);
                break;
            case "yellow":
                image.color = new Color32(222, 255, 0, 200);
                break;
            case "green":
                image.color = new Color32(40, 184, 1, 200);
                break;
            case "darkBlue":
                image.color = new Color32(6, 0, 255, 200);
                break;
            default:
                break;
        }
    }


    public void BuyHouse()
    {
        Game.PlayerBuyHouse(tile);
        SetPropertyText();
    }


    public void SellHouse()
    {
        Game.PlayerSellHouse(tile);
        SetPropertyText();
    }


    public void BuyHotel()
    {
        Game.PlayerBuyHotel(tile);
        SetPropertyText();
    }


    public void SellHotel()
    {
        Game.PlayerSellHotel(tile);
        SetPropertyText();
    }


    public void MortgageProperty()
    {
        Game.PlayerMortgageProperty(tile);
        SetPropertyText();
    }


    public void SellProperty()
    {
        Game.PlayerSellProperty(tile);
    }


    private void OnDestroy()
    {

    }
}
