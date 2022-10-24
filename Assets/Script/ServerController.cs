using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Selects the type of connection and the type of object (Server or client).
public class ServerController : MonoBehaviour
{
    public GameObject serverParent;

    private static ServerController serverInstance;

    public GameObject messgePrefab;
    public GameObject chatBillboard;

    public static ServerController MyServerInstance
    {
        get
        {
            if (serverInstance == null)
            {
                serverInstance = FindObjectOfType<ServerController>();
            }
            return serverInstance;
        }


    }

    public enum SocketTypeProtocol
    {
        TCP,
        UDP
    }

    [SerializeField]
    private SocketTypeProtocol socketType = SocketTypeProtocol.UDP;

    public SocketTypeProtocol GetSocketType { get => socketType; set => socketType = value; }

    public enum ServerType
    {
        Server,
        Client
    }

    [SerializeField]
    private ServerType serverType = ServerType.Server;

    public ServerType GetServerType { get => serverType; set => serverType = value; }

    public string IPServer { get; set; } = "127.0.0.1";
    public int serverPort { get; set; } = 9500;

    // Start is called before the first frame update and selects the type of client and server.
    void Start()
    {
        if (serverType == ServerType.Server)
        {
            if (socketType == SocketTypeProtocol.TCP)
                serverParent.AddComponent<TCPServer>();

            else if (socketType == SocketTypeProtocol.UDP)
                serverParent.AddComponent<UDPServer>();
        }

        if(serverType == ServerType.Client)
        {
            if (socketType == SocketTypeProtocol.TCP)
                serverParent.AddComponent<TCPClient>();

            else if (socketType == SocketTypeProtocol.UDP)
                serverParent.AddComponent<UDPClient>();
        }
    }

    //In charge of printing the received messages.
    void Update()
    {
        if (serverParent.GetComponent<TCPClient>() != null)
        {
            if (serverParent.GetComponent<TCPClient>().messageDecoded != null)
            {
                Debug.Log("Message checked and creating...!: " + serverParent.GetComponent<TCPClient>().messageDecoded);
                CreateMessage(serverParent.GetComponent<TCPClient>().messageDecoded);
                serverParent.GetComponent<TCPClient>().messageDecoded = null;
                //print the messages that has been created
            }
        }

        if (serverParent.GetComponent<TCPServer>() != null)
        {
            if (serverParent.GetComponent<TCPServer>().messageDecoded != null)
            {
                Debug.Log("Message checked and creating...!" + serverParent.GetComponent<TCPServer>().messageDecoded);
                CreateMessage(serverParent.GetComponent<TCPServer>().messageDecoded);
                serverParent.GetComponent<TCPServer>().messageDecoded = null;
                //print the messages that has been created
            }
        }

        if (serverParent.GetComponent<UDPClient>() != null)
        {
            if (serverParent.GetComponent<UDPClient>().messageDecoded != null)
            {
                Debug.Log("Message checked and creating...!: " + serverParent.GetComponent<UDPClient>().messageDecoded);
                CreateMessage(serverParent.GetComponent<UDPClient>().messageDecoded);
                serverParent.GetComponent<UDPClient>().messageDecoded = null;
                //print the messages that has been created
            }
        }

        if (serverParent.GetComponent<UDPServer>() != null)
        {
            if (serverParent.GetComponent<UDPServer>().messageDecoded != null)
            {
                Debug.Log("Message checked and creating:" + serverParent.GetComponent<UDPServer>().messageDecoded);
                CreateMessage(serverParent.GetComponent<UDPServer>().messageDecoded);
                serverParent.GetComponent<UDPServer>().messageDecoded = null;
                //print the messages that has been created
            }
        }
    }

    //called when creating a server to be shown on screen.
    public void CreateMessage(string _message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(messgePrefab, Vector3.zero, Quaternion.identity, chatBillboard.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_message);
    }
}
