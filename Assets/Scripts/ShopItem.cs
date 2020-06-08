﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    // Variabeln Shop
    public GameObject buyMessage;       // REF Kauftext
    private bool inBuyZone;             // Ob Spieler in Kauffläche steht

    // Variabeln Kaufgegenstand
    [Header("Item parameters")]
    public bool isHealthRestore;        // REF Ob Kaufgegenstand Heilen ist
    public bool isHealthUpgrade;        // REF Ob Kaufgegenstand Upgrade Hitpoints ist
    public bool isWeapon;               // REF Ob Kaufgegenstand Waffe ist
    public int itemCost;                // REF Preis des Kaufgegenstands


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BuyItem();
    }



    // Methode Kauftext deaktivieren wenn Spieler reingeht
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            buyMessage.SetActive(true);
            inBuyZone = true;
        }
    }


    // Methode Kauftext deaktivieren wenn Spieler rausgeht
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            buyMessage.SetActive(false);
            inBuyZone = false;
        }
    }


    // Methode Heilen kaufen
    private void BuyItem()
    {
        if (inBuyZone)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (LevelManager.instance.currentCoins >= itemCost)
                {
                    LevelManager.instance.SpendCoins(itemCost);

                    if (isHealthRestore)
                    {
                        PlayerHealthController.instance.HealPlayer(PlayerHealthController.instance.maxHealth);
                    }
                }
            }
        }
    }
}
