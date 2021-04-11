/*-----------------------------------------------------------
    Script Name: DiceScript.cs
    Purpose: Generate a random number for the player to move.
    Author: Logan Ryan
    Last Edit: 12 April 2021
-------------------------------------------------------------
    Copyright 2021 Logan Ryan
-----------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceScript : MonoBehaviour
{
    //===== PUBLIC VARIABLES =====
    public TextMeshProUGUI diceText;

    //===== PRIVATE VARIABLES =====
    private int number = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Increase the dice number by 1 and if it exceeds 10 then start from 1
        number++;
        number %= 11;

        // If the number is on 0, then change it to 1
        if (number == 0)
        {
            number = 1;
        }

        // Display number
        diceText.text = number.ToString();
    }
}
