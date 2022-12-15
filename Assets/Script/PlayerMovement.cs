using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public bool isMoving = false;

    //Client = 0
    //ServeR = 1
    public int serverType = 0;
    // Start is called before the first frame update
    void Start()
    {
        switch (this.gameObject.GetComponent<ServerController>().GetServerType)
        {
            case ServerController.ServerType.Server:
                serverType = 1;
                break;
            case ServerController.ServerType.Client:
                serverType = 0;
                break;
            default:
                serverType = 3;
                break;
        }

        isMoving = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (serverType == 0)
        {
            if (this.gameObject.GetComponent<UDPClient>().thisPlayer == null)
                return;
        }
        else
        {
            if (this.gameObject.GetComponent<UDPServer>().thisPlayer == null)
                return;
        }

        

        if (serverType != 3)
        {
            if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
                return;

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
                player.transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
                isMoving = true;

                if (serverType == 0)
                {
                    this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[0] = player.transform.position.x;
                    this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[1] = player.transform.position.y;
                    this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[2] = player.transform.position.z;
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    this.gameObject.GetComponent<UDPServer>().thisPlayer.positions[0] = player.transform.position.x;
                    this.gameObject.GetComponent<UDPServer>().thisPlayer.positions[1] = player.transform.position.y;
                    this.gameObject.GetComponent<UDPServer>().thisPlayer.positions[2] = player.transform.position.z;
                    WalkingAnimation();
                }
            }

            if (isMoving == false)
                return;

            if (serverType == 0)
                this.gameObject.GetComponent<UDPClient>().PingMovement(this.gameObject.GetComponent<UDPClient>().thisPlayer.positions);
            if (serverType == 1)
                this.gameObject.GetComponent<UDPServer>().PingMovement(this.gameObject.GetComponent<UDPServer>().thisPlayer.positions);

            movementDirection = Vector3.zero;
        }
    }

    public void WalkingAnimation()
    {

    }
}


