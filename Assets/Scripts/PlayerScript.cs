/*-------------------------------------------------------------
    Script Name: PlayerScript.cs
    Purpose: Control the player while their on the board stage.
    Author: Logan Ryan
    Last Edit: 14 April 2021
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

    //[HideInInspector]
    public int moveSpaces = 22;
    [HideInInspector]
    public bool arrived = false;
    //[HideInInspector]
    public int coins = 20;
    [HideInInspector]
    public int stars = 0;

    //===== PRIVATE VARIABLES =====
    GameManager gameManager;
    Rigidbody playerRB;
    GameObject startingSpace;
    GameObject currentSpace;
    Vector3 posOfSpace;
    bool diceBlockHit = false;
    int xPosOfSpace = 0;
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

            transform.position = Vector3.MoveTowards(transform.position, posOfSpace, step);

            // On Arrival Function
            if (Vector3.Distance(transform.position, posOfSpace) < 0.001f)
            {
                moveSpaces--;

                arrived = true;
                diceBlockHit = false;

                startingSpace = null;

                // Check what sort of space the player is on
                Material spaceMat = currentSpace.GetComponent<MeshRenderer>().material;

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
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DiceBlock"))
        {
            DiceScript diceScript = collision.gameObject.GetComponent<DiceScript>();

            moveSpaces = diceScript.number;
            xPosOfSpace = moveSpaces * 2;

            posOfSpace = new Vector3(transform.position.x + xPosOfSpace, 1);
            
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

            // Check if the player is on the star space
            Material spaceMat = other.gameObject.GetComponentInParent<MeshRenderer>().material;

            if (spaceMat.name == "StarSpaceMat (Instance)")
            {
                gameManager.boardSpace = GameManager.SpaceType.STAR;
            }
        }
    }
}
