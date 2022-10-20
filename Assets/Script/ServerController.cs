using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{
    public GameObject serverParent;

    private static ServerController serverInstance;

    public GameObject messgePrefab;
    public GameObject chatParent;

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

    // Start is called before the first frame update
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
            CreateMessage("Hello");
            if (socketType == SocketTypeProtocol.TCP)
                serverParent.AddComponent<TCPClient>();

            else if (socketType == SocketTypeProtocol.UDP)
                serverParent.AddComponent<UDPClient>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (serverParent.GetComponent<TCPClient>().strings[] > 0)
        {
            //print the messages that has been created
        }
    }

    public void CreateMessage(string _message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(GameObject.Find("ClientManager").GetComponent<ServerController>().messgePrefab, Vector3.zero, Quaternion.identity, GameObject.Find("ClientManager").GetComponent<ServerController>().chatParent.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_message);
    }
}
