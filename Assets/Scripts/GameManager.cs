/*-----------------------------
    Script Name: GameManager.cs
    Purpose: Control the game.
    Author: Logan Ryan
    Last Edit: 14 April 2021
-------------------------------
    Copyright 2021 Logan Ryan
-----------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //===== PUBLIC VARIABLES =====
    public TextMeshProUGUI diceText;

    //===== PRIVATE VARIABLES =====
    DiceScript diceScript;
    PlayerScript playerScript;

    GameObject diceBlock;

    [SerializeField]
    GameObject diceBlockPrefab;
    [SerializeField]
    GameObject player;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 playerPosition = new Vector3(player.transform.position.x,
                                             player.transform.position.y + 2.75f,
                                             player.transform.position.z);

        diceBlock = Instantiate(diceBlockPrefab, playerPosition, Quaternion.identity);

        diceScript = GameObject.FindGameObjectWithTag("DiceBlock").GetComponent<DiceScript>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Display the dice number if the dice has not been rolled
        if (GameObject.FindGameObjectWithTag("DiceBlock"))
        {
            diceText.text = diceScript.number.ToString();
        }
        else
        {
            // Otherwise display the player's number of spaces left to move
            diceText.text = playerScript.moveSpaces.ToString();

            if (playerScript.moveSpaces == 0)
            {
                string zeroText = "0";

                diceText.text = zeroText;
            }
        }
        
        // TODO - Instantiate a new dice block when arrived at the final square
        if (playerScript.arrived && diceBlock == null)
        {
            Vector3 playerPosition = new Vector3(player.transform.position.x, 
                                                 player.transform.position.y + 2.75f, 
                                                 player.transform.position.z);

            diceBlock = Instantiate(diceBlockPrefab, playerPosition, Quaternion.identity);
            diceScript = diceBlock.GetComponent<DiceScript>();
        }
    }
}
