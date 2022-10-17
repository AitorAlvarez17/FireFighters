using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            player.transform.position += new Vector3(0, 0, 5 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            player.transform.position += new Vector3(0, 0, -5 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            player.transform.position += new Vector3(-5 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            player.transform.position += new Vector3(5 * Time.deltaTime, 0, 0);
        }

    }
}
