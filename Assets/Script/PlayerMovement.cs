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
            if (Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.D) == false)
            {
                isMoving = false;
            }

            if (Input.GetKey(KeyCode.W) == true)
            {
                //Debug.Log("KEY CODE on an infinte loop");
                //Z+
                player.transform.position += new Vector3(0, 0, 5 * Time.deltaTime * speed);
                isMoving = true;

                if (serverType == 0)
                {
                    this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[2] = player.transform.position.z;
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    this.gameObject.GetComponent<UDPServer>().thisPlayer.positions[2] = player.transform.position.z;
                    WalkingAnimation();
                }
            }
            if (Input.GetKey(KeyCode.S) == true)
            {
                //Z-
                player.transform.position += new Vector3(0, 0, -5 * Time.deltaTime * speed);
                isMoving = true;

                if (serverType == 0)
                {
                    this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[2] = player.transform.position.z;
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    this.gameObject.GetComponent<UDPServer>().thisPlayer.positions[2] = player.transform.position.z;
                    WalkingAnimation();
                }
            }
            if (Input.GetKey(KeyCode.A) == true)
            {
                //X-
                player.transform.position += new Vector3(-5 * Time.deltaTime * speed, 0, 0);
                isMoving = true;

                if (serverType == 0)
                {
                    this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[0] = player.transform.position.x;
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    this.gameObject.GetComponent<UDPServer>().thisPlayer.positions[0] = player.transform.position.x;
                    WalkingAnimation();
                }
            }
            if (Input.GetKey(KeyCode.D) == true)
            {
                //X+
                player.transform.position += new Vector3(5 * Time.deltaTime * speed, 0, 0);
                isMoving = true;

                if (serverType == 0)
                {
                    this.gameObject.GetComponent<UDPClient>().thisPlayer.positions[0] = player.transform.position.x;
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    this.gameObject.GetComponent<UDPServer>().thisPlayer.positions[0] = player.transform.position.x;
                    WalkingAnimation();
                }
            }

            if (isMoving == false)
                return;

            if (serverType == 0)
                this.gameObject.GetComponent<UDPClient>().PingMovement(this.gameObject.GetComponent<UDPClient>().thisPlayer.positions);
            if (serverType == 1)
                this.gameObject.GetComponent<UDPServer>().PingMovement(this.gameObject.GetComponent<UDPServer>().thisPlayer.positions);
        }
    }

    public void WalkingAnimation()
    {

    }
}


