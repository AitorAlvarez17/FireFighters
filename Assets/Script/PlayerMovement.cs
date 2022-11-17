using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    public float speed;

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
    }
    // Update is called once per frame
    void Update()
    {
        if (serverType != 3)
        {
            if (Input.GetKey(KeyCode.W))
            {
                Debug.Log("KEY CODE on an infinte loop");
                //Z+
                player.transform.position += new Vector3(0, 0, 5 * Time.deltaTime * speed);

                if (serverType == 0)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions[2] = player.transform.position.z;
                    this.gameObject.GetComponent<UDPClient>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions[2] = player.transform.position.z;
                    this.gameObject.GetComponent<UDPServer>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                //Z-
                player.transform.position += new Vector3(0, 0, -5 * Time.deltaTime * speed);

                if (serverType == 0)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions[2] = player.transform.position.z;
                    this.gameObject.GetComponent<UDPClient>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions[2] = player.transform.position.z;
                    this.gameObject.GetComponent<UDPServer>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                //X-
                player.transform.position += new Vector3(-5 * Time.deltaTime * speed, 0, 0);
                if (serverType == 0)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions[0] = player.transform.position.x;
                    this.gameObject.GetComponent<UDPClient>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions[0] = player.transform.position.x;
                    this.gameObject.GetComponent<UDPServer>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                //X+
                player.transform.position += new Vector3(5 * Time.deltaTime * speed, 0, 0);

                if (serverType == 0)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions[0] = player.transform.position.x;
                    this.gameObject.GetComponent<UDPClient>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPClient>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
                if (serverType == 1)
                {
                    PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions[0] = player.transform.position.x;
                    this.gameObject.GetComponent<UDPServer>().PingMovement(PlayerManager.PlayersBrainDictionary[this.gameObject.GetComponent<UDPServer>().thisPlayer.id].positions);
                    WalkingAnimation();
                }
            }
        }
    }

    public void WalkingAnimation()
    {

    }
}


