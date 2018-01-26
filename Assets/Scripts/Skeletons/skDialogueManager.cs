﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skDialogueManager : MonoBehaviour {

	public Memento memento;
	public skSpawner dialogueContainer;
	skDialogueUI UIDialogueText;

	public string[] spawnDialogues = new string[3];
	public float[] timeSpawnDialogues = new float[3];

	public string[] casualDialogues = new string[3];
	public float[] timeCasualDialogues = new float[3];

	public string[] hintDialogues = new string[3];
	public float[] timeHintDialogues = new float[3];
	public skBehaviour mySkBehaviour;

	float distancePreDialogue = 16;
	float distanceDialogue = 6;
	float actualDistance;
	float timer;
	string playerZone; //(NoDialogue, PreDialogue, Dialogue)
	public string dialogueState = "NoDialogue"; //(NoDialogue, PreDialogue, Dialogue, OutDialogue, InstantDialogue)
	public string dialogueType; //(Spawn, Casual, Hint)
    string dialogueName;

    Animator olala;
    /*public AnimationCurve animCurveBoxMove;
    RectTransform textContainerTransform;
    float positionDown = -513;
    float positionUp = -392;*/


    void Awake()
    {
        olala = GameObject.Find("BoxDialogueContainer").GetComponent<Animator>();
        /*textContainerTransform = GameObject.Find("BoxDialogueContainer").GetComponent<RectTransform>();
        print(textContainerTransform.name);*/
        StartCoroutine (CheckDistanceToPlayer ());
	} // Start checkDistance coroutine

	void Update(){
		switch(dialogueState){
		    case "PreDialogue": //At the end you can no longer have the Dialogue
                timer -= Time.deltaTime;
				if(timer<=0){
					UIDialogueText.ClearDisplay ();
                    olala.SetBool("opened", false);
                    dialogueState = "NoDialogue";
                    mySkBehaviour.MoveToRubble();
                    PlayerController.pc.beingTalkedTo = null;
			    }
			    break;
            case "OutDialogue": // At the end you can have a PreDialogue again when entering in the PreDialogue Zone
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
					UIDialogueText.ClearDisplay ();
                    olala.SetBool("opened", false);
                    dialogueState = "NoDialogue";
                    mySkBehaviour.MoveToRubble();
                    PlayerController.pc.beingTalkedTo = null;
                }
                break;
            case "Dialogue": // At the end you can have an event and you have no more outdialogue when you go somewhere else
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
					UIDialogueText.ClearDisplay ();
                    olala.SetBool("opened", false);
                    print("here comes an event ?");
                    dialogueState = "NoDialogue";
                    mySkBehaviour.MoveToRubble();
                    PlayerController.pc.beingTalkedTo = null;
                }
                break;
		}
	} // decrease timers and apply events 

	public void ShowDialogueSetTimer(string typeDialogue /*(Spawn, Casual, Hint)*/, int whichDialogue)
    {
        olala.SetBool("opened", true);
        PlayerController.pc.beingTalkedTo = gameObject;
		mySkBehaviour.state = "Talking";
		switch(typeDialogue){
		    case "Spawn":
				UIDialogueText.StartDisplaying (spawnDialogues [whichDialogue], dialogueName);
                timer = timeSpawnDialogues[whichDialogue];
                if (whichDialogue == 1)
                    mySkBehaviour.Invoke("MoveToRubble", timer);
			    break;
			case "Casual":
				UIDialogueText.StartDisplaying (casualDialogues [whichDialogue], dialogueName);
                timer = timeCasualDialogues[whichDialogue];
                break;
			case "Hint":
				UIDialogueText.StartDisplaying (hintDialogues [whichDialogue], dialogueName);
                timer = timeHintDialogues[whichDialogue];
                break;
		}
	} //Said in title

	IEnumerator CheckDistanceToPlayer(){
		while(true){
			yield return new WaitForSeconds (0.5f);
			actualDistance = Vector3.Distance (transform.position, PlayerController.pc.transform.position);

			//QUAND ON EST LOIN DU PNJ------------------------------------------------------------------------------------------
			if(actualDistance>=distancePreDialogue && playerZone!="NoDialogue"){
				playerZone = "NoDialogue";
			}

			//QUAND ON RENTRE DANS LA ZONE POUR SE FAIRE INTERPELLER------------------------------------------------------------
			else if(actualDistance<=distancePreDialogue && actualDistance>distanceDialogue && playerZone!="PreDialogue"){
				//et qu'on était loin du pnj --> PreDialogue
				if(playerZone=="NoDialogue")
                {
                    StartPreDialogue();
                }
				//et qu'on était proche du pnj --> OutDialogue
				else if(playerZone=="Dialogue"){
                    StartOutDialogue();
				}
				playerZone = "PreDialogue";
			}

			//QUAND ON EST PROCHE DU PNJ---------------------------------------------------------------------------------------
			else if(actualDistance<=distanceDialogue && playerZone!="Dialogue")
            {
                StartDialogue();
				playerZone = "Dialogue";
			}
		}
	} //check distance and choose which type of dialogue should be said by the pnj

    void StartPreDialogue()
    {
        if ((PlayerController.pc.beingTalkedTo == null || PlayerController.pc.beingTalkedTo == gameObject) && dialogueState == "NoDialogue")
        {
            if (Random.Range(0, 2) == 0)
                dialogueType = "Casual";
            else
                dialogueType = "Hint";
            ShowDialogueSetTimer(dialogueType, 0);
            dialogueState = "PreDialogue";
        }
    } // engage PreDialogue && Set le type de dialogue

    public void StartDialogue()
    {
        if ((PlayerController.pc.beingTalkedTo == null || PlayerController.pc.beingTalkedTo == gameObject) && ((dialogueState == "NoDialogue" && dialogueType == "Spawn") || dialogueState == "PreDialogue" || dialogueState == "InstantDialogue"))
        {
            ShowDialogueSetTimer(dialogueType, 1);
            dialogueState = "Dialogue";
        }
    } // engage Dialogue si entre dans la zone et que le pnj était encore en state "PreDialogue"

    void StartOutDialogue()
    {
        if ((PlayerController.pc.beingTalkedTo == null || PlayerController.pc.beingTalkedTo == gameObject) && dialogueState == "Dialogue")
        {
            ShowDialogueSetTimer(dialogueType, 2);
            dialogueState = "OutDialogue";
        }
    } // engage OutDialogue si on sort de la zone Dialogue alors qu'il était en train de parler

   /* IEnumerator DisplayBoxMove(bool up)
    {
        float actualTime = 0f;
        Vector3 pos = textContainerTransform.anchoredPosition;
        Vector3 newPosition;
        Vector3 oldPosition;
        if (up)
        {
            newPosition = new Vector3(pos.x, positionUp, pos.z);
            oldPosition = new Vector3(pos.x, positionDown, pos.z);
        }
        else
        {
            newPosition = new Vector3(pos.x, positionDown, pos.z);
            oldPosition = new Vector3(pos.x, positionUp, pos.z);
        }
        while (actualTime < 0.37f)
        {
            actualTime += 0.02f;
            textContainerTransform.anchoredPosition = Vector3.Lerp(oldPosition, newPosition, animCurveBoxMove.Evaluate(actualTime));
            yield return new WaitForSeconds(0.02f);
        }
    }*/


    //A NOTER : QUAND ON BARK PRES D'UN PNJ, CELUI-CI NOUS SORT UNE PHRASE DE DIALOGUE DE TYPE "DIALOGUE"
    

    public void SetMemento(Memento mementoParam)
	{
		memento = mementoParam;
        dialogueName = memento.name;
		SetDialogues ();
	}
	public void SetUIDialogueText(skDialogueUI text){
		UIDialogueText = text;
	}
	void SetDialogues(){
		spawnDialogues = dialogueContainer.dialogues [memento.ID].spawnDialogues;
		timeSpawnDialogues = dialogueContainer.dialogues [memento.ID].timeSpawnDialogues;

		hintDialogues = dialogueContainer.dialogues [memento.ID].hintDialogues;
		timeHintDialogues = dialogueContainer.dialogues [memento.ID].timeHintDialogues;

		casualDialogues = dialogueContainer.dialogues [memento.ID].casualDialogues;
		timeCasualDialogues = dialogueContainer.dialogues [memento.ID].timeCasualDialogues;
	}
}