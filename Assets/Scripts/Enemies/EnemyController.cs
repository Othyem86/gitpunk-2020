﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Variabeln Bewegung
    [Header("Movement")]
    public float moveSpeed;                 // REF Beweguntsgeschwindigkeit
    public Rigidbody2D theRB;               // REF Kollisionskörper Spieler
    private Vector3 moveDirection;          // Bewegungsrichtung des Gegners
    public Animator anim;                   // REF Animation

    // Variabeln Verfolgen
    [Header("Chase Player")]
    public bool shouldChasePlayer;          // REF ob Gegner Spieler verfolgen soll
    public float rangeToChasePlayer;        // REF Verfolgungsradius

    // Variabeln Fliehen
    [Header("Run Away")]
    public bool shouldRunAway;              // REF ob Gegner vor Spieler fliehen soll
    public float rangeToRunAway;            // REF Fluchtradius

    // Variabeln Zufallbewegung
    [Header("Wander")]
    public bool shouldWander;               // REF ob Gegner schwiefen soll
    public float wanderLength;              // REF wie lange Gegner schweifen soll
    public float pauseLength;               // REF wie lange Gegner pausen soll
    private float wanderCounter;            // Countdown bis zur nächsten Pause
    private float pauseCounter;             // Countdown bis zur nächsten Bewegung
    private Vector3 wanderDirection;        // Bewegungsrichtung

    // Variabeln Patroullieren
    [Header("Patrolling")]
    public bool shouldPatrol;               // REF ob Gegner patroullieren soll
    public Transform[] patrolPoints;        // REF Array aller Patroullienpunkte
    private int currentPatrolPoint;         // nächster Zielpunkt 

    // Variabeln Hitpoints
    [Header("Hitpoints")]
    public int health = 150;                // REF Hitpoints
    public GameObject[] deathSplatters;     // REF Todanimation
    public GameObject hitEffect;            // REF Treffereffekt

    // Variabeln Schiessen
    [Header("Shooting")]
    public bool shouldShoot;                // Ob es schiessen soll
    public GameObject bullet;               // REF Kugel
    public Transform firePoint;             // REF Kugelursprung
    public float fireRate;                  // REF Schussfrequenz
    private float fireCounter;              // Countdown bis zur nächsten Kugel
    public float shootRange;                // REF Schussreichweite
    public SpriteRenderer enemyBody;        // REF Renderer Körper Gegner

    // Variabeln für Random Drops
    [Header("Drops")]
    public bool shouldDropItem;             // REF ob es ein Drop geben soll
    public GameObject[] itemsToDrop;        // REF Array von Drops
    public float itemDropPercent;           // REF Chancen ein Drop zu erstellen


    // Start is called before the first frame update
    void Start()
    {
        // Am Start des Raumes, Zufällige Pausenzeit generieren
        if (shouldWander)
        {
            pauseCounter = Random.Range(pauseLength * 0.75f, pauseLength * 1.25f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Schiessen und Bewegen je nach Gegnertyp
        if (enemyBody.isVisible && PlayerController.instance.gameObject.activeInHierarchy)
        {
            // Gegner bewegen
            moveDirection = Vector3.zero;
            EnemyShoot();
            EnemyMove();

            // Bewegungsrichtung normalisieren, Geschiwindigkeit Kollisionskörper berechnen
            moveDirection.Normalize();
            theRB.velocity = moveDirection * moveSpeed;
        }
        else
        {
            theRB.velocity = Vector3.zero;
        }

        // Gegner animieren
        AnimateEnemy();
    }



    //
    //  METHODEN
    //

    // Methode Gegner Schaden
    public void DamageEnemy(int damage)
    {
        // Schaden vom HP abziehen und Shadenanimation generieren
        health -= damage;
        AudioManager.instance.PlaySFX(2);
        Instantiate(hitEffect, transform.position, transform.rotation);

        // Zerstöre Gegners wenn Hitpoints Null sind und Gegnerreste generieren
        if (health <= 0)
        {
            Destroy(gameObject);
            AudioManager.instance.PlaySFX(1);

            // Reste hinterlassen
            int selectedSplatter = Random.Range(0, deathSplatters.Length);
            int rotation = Random.Range(0, 360);
            Instantiate(deathSplatters[selectedSplatter], transform.position, Quaternion.Euler(0f, 0f, rotation));

            // Drop erstellen
            if (shouldDropItem)
            {
                // Zufallszahl zwischen 0-100 generieren
                float dropChance = Random.Range(0f, 100f);

                // Falls der Zufallszahl kleiner ist als die Chancen zum Drop => Drop erstellen
                if (dropChance < itemDropPercent)
                {
                    int randomItem = Random.Range(0, itemsToDrop.Length);
                    Instantiate(itemsToDrop[randomItem], transform.position, transform.rotation);
                }
            }
        }
    }



    // Methode Verfolgen + Schweifen / Patroullieren
    private void EnemyMove()
    {
        // Spieler Verfolgen wenn Spieler in Verfolgungsradius
        if (shouldChasePlayer && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChasePlayer)
        {
            moveDirection = PlayerController.instance.transform.position - transform.position;
        }
        else if (shouldRunAway && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToRunAway)
        {
            moveDirection = transform.position - PlayerController.instance.transform.position;
        }
        else if (shouldWander)
        {
            EnemyWander();
        }
        else if (shouldPatrol)
        {
            EnemyPatroll();
        }
    }



    // Methode Schweifen
    private void EnemyWander()
    {
        // Bewegen zwischen Pausen
        if (wanderCounter > 0)
        {
            wanderCounter -= Time.deltaTime;
            moveDirection = wanderDirection;

            if (wanderCounter <= 0)
            {
                pauseCounter = Random.Range(pauseLength * 0.75f, pauseLength * 1.25f);
            }
        }

        // Pausen zwischen Bewegungen
        if (pauseCounter > 0)
        {
            pauseCounter -= Time.deltaTime;

            if (pauseCounter <= 0)
            {
                wanderCounter = Random.Range(wanderLength * 0.75f, wanderLength * 1.25f);
                wanderDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            }
        }
    }



    // Methode Patroullieren
    private void EnemyPatroll()
    {
        moveDirection = patrolPoints[currentPatrolPoint].position - transform.position;

        // Wenn Ziel erreich, nächstes Ziel wählen
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 0.2f)
        {
            currentPatrolPoint++;

            // Am ende der Zielliste, zum Angfang der Zielliste springen
            if (currentPatrolPoint >= patrolPoints.Length)
            {
                currentPatrolPoint = 0;
            }
        }
    }



    // Methode Schiessen
    private void EnemyShoot()
    {
        // Schiessen wenn Spieler in Reicheweite
        if (shouldShoot && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < shootRange)
        {
            fireCounter -= Time.deltaTime;

            if (fireCounter <= 0)
            {
                fireCounter = fireRate;
                Instantiate(bullet, firePoint.position, firePoint.rotation);
                AudioManager.instance.PlaySFX(13);
            }
        }
    }



    // Methode Gegner animieren
    private void AnimateEnemy()
    {
        // Animations-switch für Stillstand und Bewegung
        if (moveDirection != Vector3.zero)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}