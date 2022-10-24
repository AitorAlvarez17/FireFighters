using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadClient : MonoBehaviour
{
    [HideInInspector]
    public string clientInput;
    bool ipSent = false;

    public void ReadInputIP(string input)
    {
        //Sets client's input
        clientInput = input;
        Debug.Log("ReadStringInput: " + input);

        if (clientInput.Contains(".") && !ipSent) //Checks if the ip has been sent.
        {
            //Sets ip as sent.
            ipSent = true;

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
        else
        {
            //In charge od sending the message after connecting with the server.
            switch (ServerController.MyServerInstance.GetSocketType)
            {
                case ServerController.SocketTypeProtocol.TCP:
                    GameObject.Find("ClientManager").GetComponent<TCPClient>().SendString(clientInput);
                    break;
                case ServerController.SocketTypeProtocol.UDP:
                    GameObject.Find("ClientManager").GetComponent<UDPClient>().SendString(clientInput);
                    break;
                default:
                    Debug.Log("Invalid protocol");
                    break;
            }
        }
    }
}
