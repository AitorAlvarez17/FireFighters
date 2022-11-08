using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<UDPClient>().thisPlayer != null && this.gameObject.GetComponent<UDPClient>().thisPlayer.onLine == true)
        {
            if (Input.GetKey(KeyCode.W))
            {
                //Z+
                player.transform.position += new Vector3(0, 0, 5 * Time.deltaTime * speed);
                PlayerManager.Players[this.gameObject.GetComponent<UDPClient>().playerKey].positions[2] = player.transform.position.z;
                this.gameObject.GetComponent<UDPClient>().PingMovement();

            }
            if (Input.GetKey(KeyCode.S))
            {
                //Z-
                player.transform.position += new Vector3(0, 0, -5 * Time.deltaTime * speed);
                PlayerManager.Players[this.gameObject.GetComponent<UDPClient>().playerKey].positions[2] = player.transform.position.z;
                this.gameObject.GetComponent<UDPClient>().PingMovement();

            }
            if (Input.GetKey(KeyCode.A))
            {
                //X-
                player.transform.position += new Vector3(-5 * Time.deltaTime * speed, 0, 0);
                PlayerManager.Players[this.gameObject.GetComponent<UDPClient>().playerKey].positions[0] = player.transform.position.x;
                this.gameObject.GetComponent<UDPClient>().PingMovement();

            }
            if (Input.GetKey(KeyCode.D))
            {
                //X+
                player.transform.position += new Vector3(5 * Time.deltaTime * speed, 0, 0);
                PlayerManager.Players[this.gameObject.GetComponent<UDPClient>().playerKey].positions[0] = player.transform.position.x;
                this.gameObject.GetComponent<UDPClient>().PingMovement();

            }
        }
    }
}
