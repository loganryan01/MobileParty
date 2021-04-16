/*-----------------------------
    Script Name: GameManager.cs
    Purpose: Control the game.
    Author: Logan Ryan
    Last Edit: 16 April 2021
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
    public TextMeshProUGUI eventUI;
    public GameObject continueButton;
    public EventAction[] eventActions;

    public int coinsToAddOrRemove = 5;
    public string blueSpaceMessage;
    public string redSpaceMessage;
    public int priceForAStar = 20;

    [HideInInspector]
    public SpaceType boardSpace;

    //===== PRIVATE VARIABLES =====
    DiceScript diceScript;
    PlayerScript playerScript;
    GameObject diceBlock;
    int turn = 1;

    [SerializeField]
    GameObject starSpacePrefab;
    [SerializeField]
    GameObject[] boardSpaces;
    GameObject starBoardSpace;
    GameObject starSpace;
    [SerializeField]
    GameObject diceBlockPrefab;
    [SerializeField]
    GameObject player;
    [SerializeField]
    int maximumTurns;
    [SerializeField, Tooltip("Cannot be greater than the number of board spaces or equal to the number of red spaces")]
    int numberOfBlueSpaces;
    [SerializeField, Tooltip("Cannot be greater than the number of board spaces or equal to the number of blue spaces")]
    int numberOfRedSpaces;
    [SerializeField]
    Material[] boardSpaceMaterials;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        RandomiseBoard();
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
        int blueSpaces = 0;
        int redSpaces = 0;
        
        foreach (GameObject boardSpace in boardSpaces)
        {
            int index = Random.Range(0, boardSpaceMaterials.Length);
            

            Material boardSpaceMaterial = boardSpaceMaterials[index];
            
            if (boardSpaceMaterial.name == "BlueSpaceMat" && blueSpaces != numberOfBlueSpaces)
            {
                boardSpace.GetComponent<MeshRenderer>().material = boardSpaceMaterial;
                blueSpaces++;
            }
            else if (boardSpaceMaterial.name == "RedSpaceMat" && redSpaces != numberOfRedSpaces)
            {
                boardSpace.GetComponent<MeshRenderer>().material = boardSpaceMaterial;
                redSpaces++;
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
