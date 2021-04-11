/*-------------------------------------------------------------
    Script Name: PlayerScript.cs
    Purpose: Control the player while their on the board stage.
    Author: Logan Ryan
    Last Edit: 11 April 2021
---------------------------------------------------------------
    Copyright 2021 Logan Ryan
-------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //===== TOUCH CONTROLS =====
        // Check if the player is touching the screen
        if (Input.touchCount > 0)
        {
            Debug.Log(Input.touchCount);
        }

        // Mouse controls are to be deleted when releasing a build
        //===== MOUSE CONTROLS =====
        // Check if the player is touching the screen
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            Debug.Log("2");
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Debug.Log("1");
        }
    }
}
