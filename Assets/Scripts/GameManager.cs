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
    public enum SpaceType
    {
        NONE,
        BLUE,
        RED,
        EVENT,
        STAR
    }
    
    //===== PUBLIC VARIABLES =====
    public TextMeshProUGUI diceText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI starQuestion;
    public GameObject starUI;
    public GameObject continueButton;
    public SpaceType boardSpace;

    public int coinsToAddOrRemove = 5;
    public int priceForAStar = 20;

    //===== PRIVATE VARIABLES =====
    DiceScript diceScript;
    PlayerScript playerScript;
    GameObject diceBlock;
    int turn = 1;

    [SerializeField]
    GameObject diceBlockPrefab;
    [SerializeField]
    GameObject player;
    [SerializeField]
    int maximumTurns;
    

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
        // ===== DICE CONTROLS =====
        // Display the dice number if the dice has not been rolled
        if (GameObject.FindGameObjectWithTag("DiceBlock"))
        {
            diceText.text = diceScript.number.ToString();
        }
        else
        {
            // Otherwise display the player's number of spaces left to move
            diceText.text = playerScript.moveSpaces.ToString();

            // If the player has no more move spaces
            if (playerScript.moveSpaces == 0)
            {
                string zeroText = "0";

                diceText.text = zeroText;
            }
        }

        // Instantiate a new dice block when arrived at the final square
        if (playerScript.arrived && diceBlock == null)
        {
            Vector3 playerPosition = new Vector3(player.transform.position.x,
                                                 player.transform.position.y + 2.75f,
                                                 player.transform.position.z);

            diceBlock = Instantiate(diceBlockPrefab, playerPosition, Quaternion.identity);
            diceScript = diceBlock.GetComponent<DiceScript>();

            // Move to the next turn
            turn++;
        }

        //===== TURN CONTROLS ====
        // Display current turn
        turnText.text = "Turn: " + turn + "/" + maximumTurns;

        // When the player finishes last turn, the game is finished
        if (turn > maximumTurns)
            Debug.Log("Game Over");

        //===== COIN CONTROLS =====
        if (playerScript.coins < 0)
        {
            playerScript.coins = 0;
        }

        switch (boardSpace)
        {
            case SpaceType.BLUE:
                playerScript.coins += coinsToAddOrRemove;
                break;
            case SpaceType.RED:
                playerScript.coins -= coinsToAddOrRemove;
                break;
        }

        
        coinText.text = "Coins: " + playerScript.coins;

        //====== STAR CONTROLS =====
        if (playerScript.stars < 0)
        {
            playerScript.stars = 0;
        }

        if (boardSpace == SpaceType.STAR)
        {
            Time.timeScale = 0;
            starQuestion.text = "Do you want to buy the star?";
            starUI.SetActive(true);
            continueButton.SetActive(false);
        }

        starText.text = "Stars: " + playerScript.stars;

        boardSpace = SpaceType.NONE;
    }

    public void ObtainStar()
    {
        if (playerScript.coins >= priceForAStar)
        {
            starQuestion.text = "Congratulations, You got a star!";
            playerScript.coins -= priceForAStar;
            playerScript.stars++;
        }
        else if (playerScript.coins < priceForAStar)
        {
            starQuestion.text = "Sorry but you don't have enough to buy the star!";
        }

        continueButton.SetActive(true);
    }

    public void RefuseStar()
    {
        Time.timeScale = 1;
        starUI.SetActive(false);
    }
}
