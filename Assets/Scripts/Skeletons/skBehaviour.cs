﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class skBehaviour : MonoBehaviour {
	
	NavMeshAgent navMesh;
	Rigidbody skRb;
	RubbleManager targetRubble;

	public Memento memento;
	public skDialogueManager mySkDialogueManager;
	public Transform targetSpot;
	public string state = "Nothing";

	void Start () 
	{
		navMesh = GetComponent<NavMeshAgent> ();
		skRb = GetComponent<Rigidbody> ();
		targetRubble = FindObjectOfType<RubbleManager> ();
		targetSpot = targetRubble.GetSpotTransform ();
		if (targetSpot == null)
		{
			targetSpot = Camera.main.GetComponent<skSpotManager> ().GetSpotTransform ();
		}
		Invoke ("MoveToRubble", 2.0f);
	}

	
	// Update is called once per frame
	void Update () {
		switch(state){
		case "Moving":
			if (targetSpot != null && navMesh.remainingDistance <= 0.1) 
			{
				navMesh.ResetPath ();
				transform.rotation = targetSpot.rotation;
			}
			break;
		case "Talking":
			transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (PlayerController.pc.transform.position - transform.position), 0.1f);
			break;
		case "Idle":

			break;
		}
	}

	public void MoveToRubble()
	{
		if (targetSpot != null)
		{
			state = "Moving";
			navMesh.SetDestination (targetSpot.position);
		}
	}

	public void SetMemento (Memento givenMemento)
	{
		memento = givenMemento;
	}
		
}
