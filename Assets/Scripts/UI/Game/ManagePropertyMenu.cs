using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MonopolyGame;

public class ManagePropertyMenu : MonoBehaviour
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PropertyListing _property;
    [SerializeField]
    private Text _text;
    private List<PropertyListing> propertyListing = new List<PropertyListing>();
    private GamePlayer player;


    public void Awake()
    {

    }


    void Start()
    {
        this.gameObject.SetActive(false);
        
    }


    void OnEnable()
    {
        Debug.Log("Loading Manage Property Menu");
        player = Game.GetLocalPlayer();
    }


    void Update()
    {
        GetPlayerProperties();
    }


    public void GetPlayerProperties()
    {
        foreach (Tile tile in player.Properties)
        {
            bool found = false;
            foreach (PropertyListing listing in propertyListing)
            {
                if (tile.name == listing.tile.name)
                    found = true;
                if (listing.tile.ownerName != player.Name)
                    propertyListing.Remove(listing);
            }
            if (!found)
                AddPropertyListing(tile);
        }
    }


    private void AddPropertyListing(Tile tile)
    {
        PropertyListing listing = Instantiate(_property, _content);
        if (listing != null)
        {
            listing.SetPropertyInfo(tile);
            propertyListing.Add(listing);
            Debug.Log("Listed property: " + tile.name);
        }
    }


    public void RemovePropertyListing(PropertyListing listing)
    {
        propertyListing.Remove(listing);
    }


    void OnDisable()
    {
        foreach (PropertyListing listing in propertyListing)
        {
            Destroy(listing.gameObject);
        }
        propertyListing.Clear();
    }
}