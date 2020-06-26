﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private bool canTalk;                   // If character is selectable
    public GameObject message;              // REF choose character message
    public DialogueManager manager;         // REF choose character message
    private bool dialogueInProgress = false;        // If a dialog is currently in progress

    public Dialogue dialogue;

    private void Update()
    {
        if (canTalk && !dialogueInProgress && Input.GetKeyDown(KeyCode.E))
        {
            dialogueInProgress = true;
            manager.StartDialogue(dialogue);
        } 
        else if (canTalk && dialogueInProgress && Input.GetKeyDown(KeyCode.E))
        {
            dialogueInProgress = false;
            manager.EndDialogue();
        }
    }

 

    // Player near talkable character
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canTalk = true;
            message.SetActive(true);
        }
    }



    // Player not near talkable character
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canTalk = false;
            message.SetActive(false);
        }
    }
}
