﻿/*-------------------------------------------------------------
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
            // Get the touch position
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            // Check if the player has touched the dice block
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("DiceBlock"))
                {
                    Debug.Log("Touching Dice Block");
                }
            }
        }

        // Mouse controls are to be deleted when releasing a build
        //===== MOUSE CONTROLS =====
        // Check if the player is touching the screen
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {

        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            // Get the touch position
            Vector2 touchPosition = Input.mousePosition;

            // Check if the player has touched the dice block
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("DiceBlock"))
                {
                    Debug.Log("Touching Dice Block");

                    // TODO - Make player jump when they touch the dice block
                }
            }
        }
    }
}