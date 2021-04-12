/*-------------------------------------------------------------
    Script Name: PlayerScript.cs
    Purpose: Control the player while their on the board stage.
    Author: Logan Ryan
    Last Edit: 12 April 2021
---------------------------------------------------------------
    Copyright 2021 Logan Ryan
-------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    //===== PUBLIC VARIABLES =====
    public TextMeshProUGUI diceText;
    public float jumpForce = 10;
    public float speed = 5;

    //===== PRIVATE VARIABLES =====
    Rigidbody playerRB;
    bool diceBlockHit = false;
    int moveSpaces = 0;
    int xPosOfSpace = 0;
    bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
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
                }
            }
        }

        // If the player has rolled the dice, move that many spaces
        if (moveSpaces > 0 && isGrounded)
        {
            float step = speed * Time.deltaTime;

            Vector3 targetPos = new Vector3(2 * xPosOfSpace, 1);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            diceText.text = moveSpaces.ToString();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DiceBlock"))
        {
            DiceScript diceScript = collision.gameObject.GetComponent<DiceScript>();

            moveSpaces = diceScript.number;
            xPosOfSpace = moveSpaces;
            
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("BoardSpace"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BoardSpace"))
        {
            moveSpaces--;

            if (moveSpaces == 0)
            {
                string zeroText = "0";
                
                diceText.text = zeroText;
            }
        }
    }
}
