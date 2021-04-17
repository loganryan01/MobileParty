/*-----------------------------
    Script Name: GameManager.cs
    Purpose: Control the game.
    Author: Logan Ryan
    Last Edit: 17 April 2021
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
    
    [Header("Dice Settings")]
    public TextMeshProUGUI diceText;
    [SerializeField]
    GameObject diceBlockPrefab;
    DiceScript diceScript;
    GameObject diceBlock;

    [Header("Coin Settings")]
    public TextMeshProUGUI coinText;
    public int coinsToAddOrRemove = 5;
    public string blueSpaceMessage;
    public string redSpaceMessage;

    [Header("Star Settings")]
    public TextMeshProUGUI starText;
    public TextMeshProUGUI starQuestion;
    public int priceForAStar = 20;
    [SerializeField]
    GameObject starSpacePrefab;
    public GameObject starUI;
    public GameObject continueButton;
    GameObject starBoardSpace;
    GameObject starSpace;

    [Header("Event Spaces Settings")]
    public TextMeshProUGUI eventUI;
    public EventAction[] eventActions;

    [Header("Player Settings")]
    [SerializeField]
    GameObject player;
    PlayerScript playerScript;

    [Header("Board Spaces Settings")]
    public TextMeshProUGUI turnText;
   
    GameObject[] boardSpaces;
    int turn = 1;
    [SerializeField]
    int maximumTurns;
    public Material[] boardSpaceMaterials;
    [HideInInspector]
    public int[] numberOfSpaces;
    public List<int> number;
    [HideInInspector]
    public bool randomiseBoard = true;
    [HideInInspector]
    public int numberOfBlueSpaces;
    [HideInInspector]
    public int numberOfRedSpaces;
    [HideInInspector]
    public SpaceType boardSpace;


    private void Awake()
    {
        DontDestroyOnLoad(this);

        // Get all the board spaces
        boardSpaces = GameObject.FindGameObjectsWithTag("BoardSpace");

        if (randomiseBoard)
        {
            RandomiseBoard();
        }
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
            diceText.gameObject.SetActive(true);
        }
        else
        {
            // Otherwise display the player's number of spaces left to move
            diceText.text = playerScript.moveSpaces.ToString();

            // If the player has no more move spaces
            if (playerScript.moveSpaces == 0)
            {
                diceText.gameObject.SetActive(false);
            }
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
                StartCoroutine(BlueSpace());
                break;
            case SpaceType.RED:
                StartCoroutine(RedSpace());
                break;
        }

        
        coinText.text = "Coins: " + playerScript.coins;

        //====== STAR CONTROLS =====
        // Spawn star space if it doesn't exists
        if (!GameObject.Find("StarSpace(Clone)"))
        {
            SpawnStarSpace();
        }

        if (playerScript.stars < 0)
        {
            playerScript.stars = 0;
        }

        if (boardSpace == SpaceType.STAR)
        {
            diceText.gameObject.SetActive(false);
            Time.timeScale = 0;
            starQuestion.text = "Do you want to buy the star?";
            starUI.SetActive(true);
            continueButton.SetActive(false);
        }

        starText.text = "Stars: " + playerScript.stars;

        //===== EVENT SPACE CONTROLS =====
        if (boardSpace == SpaceType.EVENT)
        {
            // Choose a random event from the list of events
            int eventAction = Random.Range(0, eventActions.Length);

            // Implement the event
            StartCoroutine(EventSpace(eventAction));
        }

        // Reset the boardSpace variable 
        boardSpace = SpaceType.NONE;
    }

    void RandomiseBoard()
    {
        // For each space on the board
        foreach (GameObject boardSpace in boardSpaces)
        {
            // Choose what type of board space it will be
            int index = Random.Range(0, boardSpaceMaterials.Length);

            // If there are meant to be no more blue spaces then make it a red space and vice versa
            if (index == 0 && numberOfBlueSpaces == 0)
            {
                index = 1;
            }
            else if (index == 1 && numberOfRedSpaces == 0)
            {
                index = 0;
            }

            // Get the material for the board space
            Material boardSpaceMaterial = boardSpaceMaterials[index];
            
            // Set the material based on what type of space it is
            if (boardSpaceMaterial.name == "BlueSpaceMat" && numberOfBlueSpaces != 0)
            {
                boardSpace.GetComponent<MeshRenderer>().material = boardSpaceMaterial;
                numberOfBlueSpaces--;
            }
            else if (boardSpaceMaterial.name == "RedSpaceMat" && numberOfRedSpaces != 0)
            {
                boardSpace.GetComponent<MeshRenderer>().material = boardSpaceMaterial;
                numberOfRedSpaces--;
            }
        }
    }

    void SpawnStarSpace()
    {
        int index = Random.Range(0, boardSpaces.Length);

        while (starBoardSpace == boardSpaces[index])
        {
            index = Random.Range(0, boardSpaces.Length);
        }

        starBoardSpace = boardSpaces[index];

        starSpace = Instantiate(starSpacePrefab, starBoardSpace.transform.position, starBoardSpace.transform.rotation);

        starBoardSpace.SetActive(false);
    }

    public void ObtainStar()
    {
        if (playerScript.coins >= priceForAStar)
        {
            starQuestion.text = "Congratulations, You got a star!";
            playerScript.coins -= priceForAStar;
            playerScript.stars++;

            starBoardSpace.SetActive(true);
            Destroy(starSpace);
            SpawnStarSpace();
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

        diceText.gameObject.SetActive(true);

        if (playerScript.arrived)
        {
            SpawnDiceBlock();
            turn++;
        }
        
    }

    void SpawnDiceBlock()
    {
        Vector3 playerPosition = new Vector3(player.transform.position.x,
                                                player.transform.position.y + 2.75f,
                                                player.transform.position.z);

        diceBlock = Instantiate(diceBlockPrefab, playerPosition, Quaternion.identity);
        diceScript = diceBlock.GetComponent<DiceScript>();
    }

    IEnumerator BlueSpace()
    {
        eventUI.gameObject.SetActive(true);
        eventUI.text = blueSpaceMessage;

        playerScript.coins += coinsToAddOrRemove;

        yield return new WaitForSeconds(3);

        eventUI.gameObject.SetActive(false);
        SpawnDiceBlock();
        turn++;
    }

    IEnumerator RedSpace()
    {
        eventUI.gameObject.SetActive(true);
        eventUI.text = redSpaceMessage;

        playerScript.coins -= coinsToAddOrRemove;

        yield return new WaitForSeconds(3);

        eventUI.gameObject.SetActive(false);
        SpawnDiceBlock();
        turn++;
    }

    IEnumerator EventSpace(int index)
    {
        eventUI.gameObject.SetActive(true);
        eventUI.text = eventActions[index].message;
        
        playerScript.coins += eventActions[index].coinsToAdd;
        playerScript.coins -= eventActions[index].coinsToRemove;
        playerScript.stars += eventActions[index].starsToAdd;
        playerScript.stars -= eventActions[index].starsToRemove;

        yield return new WaitForSeconds(3);

        eventUI.gameObject.SetActive(false);
        SpawnDiceBlock();
        turn++;
    }
}
