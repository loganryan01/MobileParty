/*-----------------------------------------
    Script Name: CameraScript.cs
    Purpose: Follow the player when moving.
    Author: Logan Ryan
    Last Edit: 12 April 2021
-------------------------------------------
    Copyright 2021 Logan Ryan
-----------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //===== PUBLIC VARIABLES =====
    public GameObject player;

    //===== PRIVATE VARIABLES =====
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;

        gameObject.transform.position = new Vector3(playerPos.x, transform.position.y, transform.position.z);
    }
}
