﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigManager : MonoBehaviour {
	private bool digInput;
	public ParticleSystem digFX;
	public Canvas partPreview;
	PartsReference pRef;
	public PlayerController player;
	public Animator anim;
	SpotManager newSpotManager;
	Rigidbody newRbBaril;
	PalmiersScripts newPalmier;
	bool diggedSomething;

	public AudioSource aS;

	public AudioClip[] digChunks;

	void Start()
	{
		pRef = Camera.main.GetComponent<PartsReference> ();
	}

	void Update()
	{
		if (digInput){
			StartCoroutine (Dig ("Nothing", 0.5f));
			print ("oui je passe par ici :'(");
		}
	}//DIG s'il n'y avait rien dans l'area

	void LateUpdate(){
		if (Input.GetButtonDown ("Dig") && !digInput && PlayerController.controlsAble && PlayerController.pc.landed) {
			digInput = true;
		} else if (digInput) {
			digInput = false;
		}
	}//Check l'Input

	void OnTriggerStay(Collider other)
	{
		float distance = Vector3.Distance (other.transform.position, transform.position);

        //SPOT------------------------------------------------------------------------------------------------
		if (other.tag == "Spot" && digInput && distance<3f) 
		{
			newSpotManager = other.transform.GetComponent<SpotManager> ();
			StartCoroutine(Dig ("Spot", 1.5f));
			digInput = false;
		}
        //RUBBLE----------------------------------------------------------------------------------------------
		else if (other.tag == "Rubble" && digInput && distance<6f) 
		{
			RubbleManager rubMan = other.gameObject.GetComponent<RubbleManager> ();
			if (rubMan.readyToDig)
			{
				rubMan.StartCoroutine ("RubbleClear");
				PlayerController.controlsAble = false;
				anim.SetTrigger ("DigTrigger");
			}
			digInput = false;
		}
		//BARIL------------------------------------------------------------------------------------------
		else if(other.tag == "Baril" && digInput && distance < 3.5f && other.GetComponent<Rigidbody>().isKinematic)
		{
			newRbBaril = other.GetComponent<Rigidbody>();
			StartCoroutine (Dig("Baril", 1f));
			digInput = false;
        }
		else if(other.tag == "Palmier" && digInput && distance < 3f){
			print ("nice");
			StartCoroutine (Dig("Palmier", 1f));
			digInput = false;
			newPalmier = other.GetComponent<PalmiersScripts> ();
		}
	}

	IEnumerator Dig(string type, float duration) 
	{
		//BEGINNING--------------------------------------------------------
		PlayerController.controlsAble = false;
		anim.SetTrigger ("DigTrigger");
		ParticleSystem instDigFX = Instantiate (digFX, transform.position, Quaternion.identity); //Instantiate FX
		Destroy(instDigFX, duration);
		Coroutine soundCo = StartCoroutine (DiggingSound(duration));

		switch(type){
		case "Spot":
			Debug.Log ("You digged out the " + newSpotManager.type + " number " + newSpotManager.part);
			Camera.main.GetComponent<InventoryManager> ().playerInventory.AddItem (newSpotManager.type, newSpotManager.part);			
			break;
		}

        //DURING
		yield return new WaitForSeconds (duration);

		//AFTER---------------------------------------------------------------

		switch(type){
		case "Spot":
                //PartPreview (newSpotManager.type, newSpotManager.part); //Instantiate canvas with preview of the sk part
                PartPreviewOverlay();
			Destroy (newSpotManager.transform.gameObject);
			newSpotManager = null;
                break;
		case "Baril":
			newRbBaril.freezeRotation = false;
			newRbBaril.isKinematic = false;
			newRbBaril.AddForce (new Vector3 (0, 50, 0));
			break;
		case "Palmier":
			newPalmier.NoixDeCocoFall ();
			newPalmier = null;
			break;
		}

		anim.SetTrigger ("DigOverTrigger");
		if(type!="Spot")
			PlayerController.controlsAble = true;
    }

	/*public void PartPreview(int type, int part) //Spawn canvas with preview
	{
		Canvas newCanvas = Instantiate (partPreview);
		newCanvas.worldCamera = Camera.main;
		GameObject foundPart;
		switch (type) 
		{
			case 0:
				foundPart = Instantiate (pRef.GetPrefabFromReference (type, part), newCanvas.GetComponentInChildren<RectTransform> ()); //spawn right sk part
				foundPart.transform.localPosition = new Vector3 (0, -353, -53);
				foundPart.transform.localRotation = Quaternion.Euler(new Vector3 (-5, -16, 0));
				foundPart.transform.localScale = new Vector3 (200, 200, 200);
				break;

			case 1 :
				foundPart = Instantiate (pRef.GetPrefabFromReference (type, part), newCanvas.GetComponentInChildren<RectTransform> ()); //spawn right sk part
				foundPart.transform.localPosition = new Vector3 (0, -127, -80);
				foundPart.transform.localScale = new Vector3 (100, 100, 100);
				break;

			case 2 :
				foundPart = Instantiate (pRef.GetPrefabFromReference (type, part), newCanvas.GetComponentInChildren<RectTransform> ()); //spawn right sk part
				foundPart.transform.localPosition = new Vector3 (0, -56, -84);
			foundPart.transform.localRotation = Quaternion.Euler(new Vector3 (0, 196, 0));
				foundPart.transform.localScale = new Vector3 (100, 100, 100);
				break;

		}
	}*/

	public void PartPreviewOverlay()
	{
		Instantiate (partPreview);
	}


	IEnumerator DiggingSound (float duration)
	{
		float time = 0;
		while (time < duration) 
		{
			aS.pitch = Random.Range (0.95f, 1.05f);
			aS.PlayOneShot(digChunks[Random.Range(0,3)]);
			while (aS.isPlaying) 
			{
				time += 0.1f;
				yield return new WaitForSeconds (0.1f);
			}
		}
		aS.pitch = 1;
	}
}

