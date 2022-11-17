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

    public string IPServer { get; set; } = "192.168.68.108";
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
           // ServerActions();

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
            //PlayerActions();
            
        }

        if (serverParent.GetComponent<UDPServer>() != null)
        {
            //ServerActions();
            
        }

        
    }

    //we do this here as it is object-centered and we need a Monobehavioural script with reference to the exact client-player reference.
    //private void PlayerActions()
    //{
    //    if (PlayerManager.playerDirty == true)
    //    {
    //        clientName.text = this.gameObject.GetComponent<UDPClient>().thisPlayer.username + " " + this.gameObject.GetComponent<UDPClient>().thisPlayer.id;
    //        //clientIndex.text = serverParent.GetComponent<UDPClient>().thisPlayer.onLine;
    //        PlayerManager.playerDirty = false;
    //    }
    //}
    
    //called when creating a server to be shown on screen.

    public void CreateMessage(PlayerPackage _Message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(messgePrefab, Vector3.zero, Quaternion.identity, chatBillboard.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_Message.message, _Message.username);
    }
}
