using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadClient : MonoBehaviour
{
    [HideInInspector]
    public string clientInput;
    [HideInInspector]
    public string clientUsername = "";
    public bool ipSent = false;
    public GameObject LoginPanel;

    public void Awake()
    {
        LoginPanel.SetActive(true);
    }

    public void ReadInputIP(string input)
    {
        //Sets client's input
        clientInput = input;
        Debug.Log("ReadStringInput: " + input);

        //In charge od sending the message after connecting with the server.
        if (!ipSent)
            return;

        switch (ServerController.MyServerInstance.GetSocketType)
        {
            case ServerController.SocketTypeProtocol.TCP:
                this.gameObject.GetComponent<TCPClient>().SendString(clientInput);
                break;
            case ServerController.SocketTypeProtocol.UDP:
                this.gameObject.GetComponent<UDPClient>().SendString(clientInput);
                break;
            default:
                Debug.Log("Invalid protocol");
                break;
        }
    }

    public void ReadUsernameC(string input)
    {
        //Sets client's input
        clientUsername = input;
        Debug.Log("Username: " + input);

        
    }

    public void Connect()
    {
        if (clientUsername != "" && clientInput.Contains(".") && !ipSent)
        {
            switch (ServerController.MyServerInstance.GetSocketType)
            {
                case ServerController.SocketTypeProtocol.TCP:
                        ipSent = true;
                        this.gameObject.GetComponent<TCPClient>().ConnectToServer(clientInput, clientUsername);
                        LoginPanel.SetActive(false);
                    break;
                case ServerController.SocketTypeProtocol.UDP:
                        ipSent = true;
                        this.gameObject.GetComponent<UDPClient>().ConnectToServer(clientInput, clientUsername);
                        LoginPanel.SetActive(false);
                    break;
                default:
                    Debug.Log("Invalid protocol");
                    break;
            }
        }
    }
}
