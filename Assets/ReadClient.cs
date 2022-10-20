using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadClient : MonoBehaviour
{
    [HideInInspector]
    public string clientInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReadInputIP(string input)
    {
        clientInput = input;
        Debug.Log("ReadStringInput: " + input);

        if (clientInput.Contains("."))
        {
            // Get client script and connect to server
            switch (ServerController.MyServerInstance.GetSocketType)
            {
                case ServerController.SocketTypeProtocol.TCP:
                    GameObject.Find("ClientManager").GetComponent<TCPClient>().ConnectToServer(clientInput);
                    break;
                case ServerController.SocketTypeProtocol.UDP:
                    GameObject.Find("ClientManager").GetComponent<UDPClient>().ConnectToServer(clientInput);
                    break;
                default:
                    Debug.Log("Invalid protocol");
                    break;
            }
        }
    }
}
