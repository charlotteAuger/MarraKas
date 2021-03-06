﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFlower : MonoBehaviour {

    public GameObject spottedParticle;
    Renderer rend;
    ParticleSystem part;
    BoxCollider col;
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public float timerLife;
    float lifeTime = 4.0f;
    bool decreaseOpacity = false;
    public float speed;

	// Use this for initialization
	void Start () {
        timerLife = Time.time;
        part = GetComponent<ParticleSystem>();
        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        /*var sh = part.shape;
        sh.scale += new Vector3(0.25f, 0.1f, 0);
        col.size = sh.scale;*/
        transform.position += direction * speed;
        transform.localScale += new Vector3(0.2f, 0.2f, 0);
        transform.position -= transform.up*0.1f;
        if (Time.time - timerLife > lifeTime && !decreaseOpacity)
        {
            decreaseOpacity = true;
            //part.Stop();
        }
        if (decreaseOpacity)
        {
            rend.material.SetFloat("_visibility", rend.material.GetFloat("_visibility") - 0.01f);
            if (rend.material.GetFloat("_visibility") <= 0)
                Destroy(gameObject);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spot"))
        {
            Instantiate(spottedParticle, other.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }
}
