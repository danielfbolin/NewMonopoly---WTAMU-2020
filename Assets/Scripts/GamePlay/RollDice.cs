using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LateExe;
using Photon.Pun;
using Photon.Realtime;
using MonopolyGame;

public class RollDice : MonoBehaviourPun
{
    Dice dice1;
    Dice2 dice2;
    //public GameObject Route;

    
    public Transform target;
    public Transform camera;

    public Vector3 offset;
    public Vector3 origi_pos = new Vector3(-4.6f, 11.84f, 5.78f);

    public bool changeCamera = false;
    public bool rolled = false;
    public bool rolledPair = false;
    public int timesRolled = 0;

    Stone stone;

    int steps, node, chanceCardNumber, communityChestCardNumber, lastRollValue;
    public string nameNode;
    public GameObject DeedCard;
    public GameObject ChanceCard;
    public GameObject CommunityChestCard;
    public GameObject Button;
    [SerializeField]
    private GameManager gameManager;

    Executer exe;


    void Awake()
    {
        dice1 = GameObject.FindObjectOfType<Dice>();
        dice2 = GameObject.FindObjectOfType<Dice2>();
        stone = GameObject.FindObjectOfType<Stone>();
        //Debug.Log(PhotonNetwork.LocalPlayer.NickName + ": token found: " + stone.name);
        exe = new Executer(this);
    }


    void Start()
    {
        EventManager.onEndTurn += EndTurn;

    }


    public void rollDice()
    {
        if (Game.IsLocalPlayerCurrent())
        {
            dice1.RollDice();
            dice2.RollDice();
        }
    }


    public int GetDiceRollValue()
    {
        return lastRollValue;
    }


    void Update()
    {
        if (Game.GetLocalPlayer() != null)
        {
            if (Game.IsLocalPlayerCurrent() && (rolledPair || (timesRolled == 0 && timesRolled < 3)))
                Button.SetActive(true);
            else
                Button.SetActive(false);
            if (Game.IsLocalPlayerCurrent())
            {
                if (dice1.hasLanded && dice1.thrown && (dice2.hasLanded && dice2.thrown))
                {
                    /*for (int i = 0; i < 40; i++)
                    {
                        HideDeedCard(i);
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        HideChanceCard(i);
                    }*/
                    rolled = true;
                    timesRolled++;
                    dice1.SideValueCheck();
                    dice2.SideValueCheck();
                    dice1.reset();
                    dice2.reset();
                    changeCamera = true;
                }
                if (rolled && (rolledPair || (0 < timesRolled && timesRolled < 3)))
                {
                    if (dice1.diceValue == dice2.diceValue)
                    {
                        rolledPair = true;
                        Debug.Log(Game.GetLocalPlayer().Name + " rolled a double! Roll again.");
                    }
                    else rolledPair = false;
                    steps = dice1.diceValue + dice2.diceValue;
                    lastRollValue = steps;

                    node += steps;
                    if (node > 39)
                    {
                        gameManager.PlayerPassedGo();
                        node -= 40;
                    }
                    //Debug.Log("Steps: " + steps);
                    //int.TryParse (nameNode, out nodeNumber);
                    stone.Movement(steps);


                    if ((node == 7) || (node == 22) || (node == 36))
                    {
                        //display ChanceCard
                        chanceCardNumber = Board.DrawChanceCard();
                        exe.DelayExecute(steps * 0.25f, "DisplayChanceCard", chanceCardNumber);
                    }
                    else if ((node == 2) || (node == 17) || (node == 33))
                    {
                        //display Community Chest
                        communityChestCardNumber = Board.DrawCommunityChestCard();
                        exe.DelayExecute(steps * 0.25f, "DisplayCommunityChestCard", communityChestCardNumber);
                    }
                    else
                    {
                        exe.DelayExecute(steps * 0.25f, "DisplayDeedCard", node);

                    }

                    if ((node % 10) == 0)
                    {
                        gameManager.PlayerLandedOnCorner(node);
                    }

                    rolled = false;

                    /*foreach(int st in chanceCard_list) 
                    { 
                        Debug.Log("list: " +st+" ");
                    }*/

                    if (rolledPair && timesRolled == 3)
                    {
                        // player must go to jail
                        // maybe use an event handler
                        Game.PlayerGoToJail();
                    }
                }
            }
        }
    }


    //Deedcard
    public void DisplayDeedCard(int number)
    {
        Transform t = DeedCard.transform.GetChild(number);
        t.gameObject.SetActive(true);
        gameManager.PlayerLandedOnTile(number);
        exe.DelayExecute(4.125f, "HideDeedCard", t);
    }
    public void HideDeedCard(Transform t)
    {
        t.gameObject.SetActive(false);
    }


    //Chance Card
    public void DisplayChanceCard(int cardNumber)
    {
        Transform t = ChanceCard.transform.GetChild(cardNumber);
        t.gameObject.SetActive(true);
        gameManager.PlayerDrawChanceCard(cardNumber);
        exe.DelayExecute(4.125f, "HideChanceCard", t);
    }
    public void HideChanceCard(Transform t)
    {
        t.gameObject.SetActive(false);
    }
    

    //Community chest
    public void DisplayCommunityChestCard(int cardNumber)
    {
        Transform t = CommunityChestCard.transform.GetChild(cardNumber);
        t.gameObject.SetActive(true);
        gameManager.PlayerDrawCommunityChestCard(cardNumber);
        exe.DelayExecute(4.125f, "HideCommunityChestCard", t);
    }
    public void HideCommunityChestCard(Transform t)
    {
        t.gameObject.SetActive(false);
    }
    
    
    void EndTurn()
    {
        //Debug.Log("RollDice.EndTurn() called");
        base.photonView.RPC("RPC_EndTurn", RpcTarget.All);
    }


    [PunRPC]
    private void RPC_EndTurn()
    {
        timesRolled = 0;
        rolled = false;
        rolledPair = false;
        if (Game.IsLocalPlayerCurrent())
            Button.SetActive(true);
        else
            Button.SetActive(false);
    }


    void OnDisable()
    {
        EventManager.onEndTurn -= EndTurn;
    }

    
    void FixedUpdate()
    {
        /*if(changeCamera == true)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp (camera.position, desiredPosition,0.125f);
            camera.position = smoothedPosition ;
        }

        if(stone.steps == 0)
        {
            camera.position = origi_pos;
        }*/
    }
}
