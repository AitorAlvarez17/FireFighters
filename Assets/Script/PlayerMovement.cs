using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float rotationSpeed;
    public bool isMoving = false;

    //Client = 0
    //ServeR = 1
    // Start is called before the first frame update
    void Start()
    {

        isMoving = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.gameObject.GetComponent<UDPClient>().thisPlayer == null || this.gameObject.GetComponent<ServerController>().gameStarted == false)
            return;

        if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
        {
            isMoving = false;
            return;
        }
            
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        if (movementDirection == Vector3.zero)
        {
            isMoving = false;
        }
        else
        {
            //this is a Prediction
            SmoothRotation(movementDirection);
            player.transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
            isMoving = true;


            this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[0] = player.transform.transform.position.x;
            this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[1] = player.transform.transform.position.y;
            this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[2] = player.transform.transform.position.z;
            WalkingAnimation();

        }

        if (isMoving == false)
            return;

        //Debug.Log("Movement Direction:" + movementDirection);
        float[] movementDirectionSerializable = new float[3];

        this.gameObject.GetComponent<UDPClient>().thisPlayer.directions[0] = movementDirection.x;
        this.gameObject.GetComponent<UDPClient>().thisPlayer.directions[1] = movementDirection.y;
        this.gameObject.GetComponent<UDPClient>().thisPlayer.directions[2] = movementDirection.z;

        //movementDirectionSerializable[0] = movementDirection.x;
        //movementDirectionSerializable[1] = movementDirection.y;
        //movementDirectionSerializable[2] = movementDirection.z;

        this.gameObject.GetComponent<UDPClient>().PingMovement(this.gameObject.GetComponent<UDPClient>().thisPlayer.positions, this.gameObject.GetComponent<UDPClient>().thisPlayer.directions);

        movementDirection = Vector3.zero;
    }

    public void WalkingAnimation()
    {

    }

    public void SmoothRotation(Vector3 directions)
    {
        Quaternion rotation = Quaternion.LookRotation(directions, Vector3.up);
        player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}


