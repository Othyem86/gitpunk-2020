﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    // Instanzierung der Klasse
    public static PlayerHealthController instance;

    public int currentHealth;               // REF aktuelle Hitpoints
    public int maxHealth;                   // REF maximale Hitpoints

    public float damageInvinceLength = 1f;  // REF Zeit unverletzbar
    private float invinceCount;             // Countdown



    // Wie Start(), nur davor
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        UIController.instance.healthSlider.maxValue = maxHealth;
        updateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (invinceCount > 0)
        {
            invinceCount -= Time.deltaTime;
            
            // Wenn nicht mehr unverletzbar, dann Spielertransparenz ausschalten
            if (invinceCount <= 0)
            {
                SetBodyAlpha(1f);
            }
        }
    }


    // Funktion Schadenanrichtung an Spieler und dessen Tod
    public void DamagePlayer()
    {
        // Schadenanrichtung wenn NICHT Unverletzbar
        if (invinceCount <= 0)
        {      
            // Nach schaden, startet den Unverletzbar-Countdown
            currentHealth--;
            invinceCount = damageInvinceLength;

            // Schadeneffekt BodySprite - Transparenz einschalten
            SetBodyAlpha(0.5f);

            // Check ob Spieler Tod ist
            if (currentHealth <= 0)
            {
                PlayerController.instance.gameObject.SetActive(false);
                UIController.instance.deathScreen.SetActive(true);
            }

            updateHealthUI();
        }
    }


    // Funktion zur Aktualisierung des Hitpoint-UIs 
    private void updateHealthUI()
    {
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }


    // Funtkion Schadeneffekt BodySprite: Transparenz ein-/ausschalten
    private void SetBodyAlpha(float alphaValue)
    {
        PlayerController.instance.bodySR.color = new Color(
        PlayerController.instance.bodySR.color.r,
        PlayerController.instance.bodySR.color.b,
        PlayerController.instance.bodySR.color.g,
                alphaValue); ;
    }
}