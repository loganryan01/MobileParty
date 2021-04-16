/*-------------------------------------------------------------
    Script Name: PlayerScript.cs
    Purpose: Control the player while their on the board stage.
    Author: Logan Ryan
    Last Edit: 16 April 2021
---------------------------------------------------------------
    Copyright 2021 Logan Ryan
-------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //===== PUBLIC VARIABLES =====
    public float jumpForce = 10;
    public float speed = 5;

    [HideInInspector]
    public int moveSpaces = 0;
    [HideInInspector]
    public bool arrived = false;
    [HideInInspector]
    public int coins = 0;
    [HideInInspector]
    public int stars = 0;

    //===== PRIVATE VARIABLES =====
    GameManager gameManager;
    Rigidbody playerRB;
    GameObject startingSpace;
    GameObject currentSpace;
    bool diceBlockHit = false;
    bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
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
                if (hit.collider.gameObject.CompareTag("DiceBlock") && !diceBlockHit)
                {
                    // NOTE: Player should jump once
                    diceBlockHit = true;

                    // Make player jump when they touch the dice block
                    playerRB.AddForce(Vector3.up * jumpForce);
                }
            }
        }

        // Mouse controls are to be deleted when releasing a build
        //===== MOUSE CONTROLS =====
        // Check if the player is touching the screen
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {

        }
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // Get the touch position
            Vector2 touchPosition = Input.mousePosition;

            // Check if the player has touched the dice block
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("DiceBlock") && !diceBlockHit)
                {
                    // NOTE: Player should jump once
                    diceBlockHit = true;
                    
                    // Make player jump when they touch the dice block
                    playerRB.AddForce(Vector3.up * jumpForce);

                    isGrounded = false;
                    arrived = false;
                }
            }
        }

        // If the player has rolled the dice, move that many spaces
        if (moveSpaces > 0 && isGrounded)
        {
            float step = speed * Time.deltaTime;

            transform.position += transform.forward * step;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DiceBlock"))
        {
            DiceScript diceScript = collision.gameObject.GetComponent<DiceScript>();

            moveSpaces = diceScript.number;
            
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("BoardSpace"))
        {
            isGrounded = true;

            currentSpace = collision.gameObject;

            if (startingSpace == null)
            {
                startingSpace = currentSpace;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BoardSpace") && 
            startingSpace != other.gameObject.transform.parent.gameObject)
        {
            moveSpaces--;

            // Check what sort of space the player is on
            Material spaceMat = currentSpace.GetComponent<MeshRenderer>().material;

            if (spaceMat.name == "LeftArrowSpaceMat (Instance)")
            {
                transform.Rotate(0, -90, 0);
                moveSpaces++;
            }
            else if (spaceMat.name == "RightArrowSpaceMat (Instance)")
            {
                transform.Rotate(0, 90, 0);
                moveSpaces++;
            }

            if (moveSpaces == 0)
            {
                arrived = true;
                diceBlockHit = false;

                startingSpace = null;

                switch (spaceMat.name)
                {
                    // Add coins when player lands on blue space
                    case "BlueSpaceMat (Instance)":
                        gameManager.boardSpace = GameManager.SpaceType.BLUE;
                        break;
                    // Remove coins when player lands on red space 
                    case "RedSpaceMat (Instance)":
                        gameManager.boardSpace = GameManager.SpaceType.RED;
                        break;
                    // Add a star when the player buys a star
                    case "StarSpaceMat (Instance)":
                        gameManager.boardSpace = GameManager.SpaceType.STAR;
                        break;
                    case "EventSpaceMat (Instance)":
                        gameManager.boardSpace = GameManager.SpaceType.EVENT;
                        break;
                }
            }
            
        }
    }
}
