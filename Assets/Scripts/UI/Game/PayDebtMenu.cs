using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MonopolyGame;

public class PayDebtMenu : MonoBehaviour
{
    [SerializeField]
    private Text debtAmountText;
    [SerializeField]
    private Button payDebtButton;
    [SerializeField]
    private Button bankruptcyButton;
    private int debtAmount;
    private GamePlayer player;



    public void Awake()
    {

    }


    void Start()
    {
        
    }


    void Update()
    {
        if (player.GetMoney() > debtAmount)
            payDebtButton.gameObject.SetActive(true);
        else
            payDebtButton.gameObject.SetActive(false);
        if (player.GetFullWorth() > debtAmount)
            bankruptcyButton.gameObject.SetActive(false);
        else
            bankruptcyButton.gameObject.SetActive(true);
    }


    public void SetDebt(int amount)
    {
        player = Game.GetLocalPlayer();
        debtAmount = amount;
        debtAmountText.text = string.Format("Debt owed: ${0}", amount);
    }
    

    public void PayDebt()
    {
        Game.PlayerPayDebt(debtAmount);
    }


    public void Bankruptcy()
    {
        Game.PlayerBankrupt();
    }


    void OnDisable()
    {

    }
}