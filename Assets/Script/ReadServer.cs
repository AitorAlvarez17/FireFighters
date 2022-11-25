using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadServer : MonoBehaviour
{
    public string clientInput;
    public bool ipSent = false;

    public GameObject LoginInput;

    public void ReadInputIP(string input)
    {
        //Sets client's input
        clientInput = input;
        Debug.Log("ReadStringInput: " + input);

        //In charge od sending the message after connecting with the server.
        switch (ServerController.MyServerInstance.GetSocketType)
        {
            case ServerController.SocketTypeProtocol.TCP:
                //this.gameObject.GetComponent<TCPServer>().SetUsernameAndConnect(clientInput);
                break;
            case ServerController.SocketTypeProtocol.UDP:
                //this.gameObject.GetComponent<UDPServer>().SetUsernameAndConnect(clientInput);
                break;
            default:
                Debug.Log("Invalid protocol");
                break;
        }

    }
}
