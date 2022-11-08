using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


//Selects the type of connection and the type of object (Server or client).
public class ServerController : MonoBehaviour
{
    public GameObject serverParent;

    private static ServerController serverInstance;

    public GameObject messgePrefab;
    public GameObject chatBillboard;

    public TextMeshProUGUI numberOfPlayers;

    public TextMeshProUGUI clientName;
    //public TextMeshProUGUI clientIndex;

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
                //CreateMessage(serverParent.GetComponent<TCPClient>().messageDecoded);
                serverParent.GetComponent<TCPClient>().messageDecoded = null;
                //print the messages that has been created
            }
        }

        if (serverParent.GetComponent<TCPServer>() != null)
        {
            ServerActions();

            if (serverParent.GetComponent<TCPServer>().messageDecoded != null)
            {
                Debug.Log("Message checked and creating...!" + serverParent.GetComponent<TCPServer>().messageDecoded);
                //CreateMessage(serverParent.GetComponent<TCPServer>().messageDecoded, "Server");
                serverParent.GetComponent<TCPServer>().messageDecoded = null;
                //print the messages that has been created
            }
        }

        if (serverParent.GetComponent<UDPClient>() != null)
        {
            PlayerActions();
            if (serverParent.GetComponent<UDPClient>().message != null && serverParent.GetComponent<UDPClient>().message.message != null)
            {
                Debug.Log("Message checked and creating...!: " + serverParent.GetComponent<UDPClient>().message.message + "From:" + serverParent.GetComponent<UDPClient>().message.username);
                //Later on take it from PlayerManager! Now just hard-took it for debug purposes.
                //You can easily acces to the player with the key (index) of it
                CreateMessage(serverParent.GetComponent<UDPClient>().message);
                serverParent.GetComponent<UDPClient>().message.SetMessage(null);
                //print the messages that has been created
            }
        }

        if (serverParent.GetComponent<UDPServer>() != null)
        {
            ServerActions();
            if (serverParent.GetComponent<UDPServer>().message != null && serverParent.GetComponent<UDPServer>().message.message != null)
            {
                Debug.Log("Message checked and creating:" + serverParent.GetComponent<UDPServer>().message.message + " From: " + serverParent.GetComponent<UDPServer>().message.username);
                CreateMessage(serverParent.GetComponent<UDPServer>().message);
                serverParent.GetComponent<UDPServer>().message.SetMessage(null);
                //print the messages that has been created
            }
        }

        
    }

    //we do this here as it is object-centered and we need a Monobehavioural script with reference to the exact client-player reference.
    private void PlayerActions()
    {
        if (PlayerManager.playerDirty == true)
        {
            clientName.text = serverParent.GetComponent<UDPClient>().thisPlayer.username;
            //clientIndex.text = serverParent.GetComponent<UDPClient>().thisPlayer.onLine;
            PlayerManager.playerDirty = false;
        }
    }
    private void ServerActions()
    {
        if (PlayerManager.serverDirty == true)
        {
            numberOfPlayers.text = "Number of Players: " + PlayerManager.playersOnline;
            PlayerManager.serverDirty = false;
        }
    }
    //called when creating a server to be shown on screen.

    public void CreateMessage(Message _Message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(messgePrefab, Vector3.zero, Quaternion.identity, chatBillboard.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_Message.message, _Message.username);
    }
}
