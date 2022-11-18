using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

//Do a parent class that is MonoBehaviour and make this heritage from the parent in order to make it virtual for PlayerMovement
public class UDPServer : MonoBehaviour 
{
    // Clients'IP and port
    private string serverIP;
    private int serverPort;

    //Data matrix and number of bytes
    private int recv;
    //private byte[] data = new byte[1024];

    //declare thread and socket
    private Thread serverThread;
    private Socket udpSocket;

    public bool serverDirty = false;
    public bool newConection = false;
    // Message decoded for rendering on screen
    public string messageDecoded = null;
    //check it
    public Player thisPlayer;
    public PlayerPackage message = new PlayerPackage(null, "Server");
    public bool onLine = false;
    public bool initServer = false;

    //declare Client's endpoint
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

    public List<EndPoint> UDPClientList = new List<EndPoint>();

    //This is the brain of the game
    public int[] gameMatrix = new int[4] { 0, 0, 0, 0 };
    public int playersOnline = 0;

    //We need a "WorldElementsMonigotes" List perfectly linked to the dataLayer "keys" or watever.

    //instanciation both variables and starts server
    void Start()
    {
        serverDirty = false;
        playersOnline = UDPClientList.Count;
        // Get IP and port
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;

        StartServer();
    }

    private void Update()
    {
        ServerActions();
    }

    public void SetUsernameAndConnect(string username)
    {
        thisPlayer.username = username;
        onLine = true;
        this.gameObject.GetComponent<ServerController>().clientName.text = thisPlayer.username;
        this.gameObject.GetComponent<ReadServer>().LoginInput.SetActive(false);

    }
    private void ServerActions()
    {
        if (serverDirty == true)
        {
            if (message != null && message.message != null && message.message != "")
            {
                Debug.Log("Message checked and creating:" + message.message + " From: " + message.username);
                CreateMessage(message);
                message.SetMessage(null);
                //print the messages that has been created
            }
            if (newConection == true)
            {
                this.gameObject.GetComponent<WorldController>().CreatePlayer(playersOnline);
                newConection = false;
            }
            this.gameObject.GetComponent<ServerController>().numberOfPlayers.text = "Number of Players: " + PlayerManager.playersOnline;
            if (message.id != 1 && message.id != thisPlayer.id && message.positions[0] != 0f || message.positions[2] != 0f)
            {
                Debug.Log("Server Player ID:" + thisPlayer.id);
                Debug.Log("Message ID:" + message.id);
                UpdateWorld(message.id, message.positions);
            }
            serverDirty = false;
            Debug.Log("Setting Text and Server Dirtyness");
        }
    }

    public void CreateMessage(PlayerPackage _Message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(this.gameObject.GetComponent<ServerController>().messgePrefab, Vector3.zero, Quaternion.identity, this.gameObject.GetComponent<ServerController>().chatBillboard.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_Message.message, _Message.username);
    }
    //closing both the socket and the thread on exit and all coroutines
    private void OnDisable()
    {
        if (udpSocket != null)
            udpSocket.Close();
        if (serverThread != null)
            serverThread.Abort();
    }

    //Initialize socket and thread
    private void StartServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;

        InitSocket();
        InitThread();
    }

    //set socket
    private void InitSocket()
    {
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    //set and initialize thread
    private void InitThread()
    {
        serverThread = new Thread(ServerThread);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    //Main thread 
    private void ServerThread()
    {
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        clientIPEP = new IPEndPoint(ipAddress, serverPort);
        clientEP = (EndPoint)clientIPEP;

        thisPlayer = new Player("Player" + (playersOnline + 1).ToString(), true, (playersOnline + 1));
        Debug.Log("BEGINNING OF THE GENERAL SERVER THREAD");
        message.SetUsername(thisPlayer.username);
        message.SetId(thisPlayer.id);
        UpdateGameMatrix(playersOnline);
        initServer = true;

        //try the socket's bind, if not debugs
        try
        {
            udpSocket.Bind(clientIPEP);
            Debug.Log("SERVER UDP socket bound to " + clientIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to bind socket: " + e.Message);
        }

        //listens for a connection, if not debugs
        try
        {
            byte[] dataTMP = new byte[1024];
            recv = udpSocket.ReceiveFrom(dataTMP, ref clientEP);
            //Debug.Log("SERVER Message received from " + clientEP.ToString() + ": Message: " + serializer.DeserializeMessage(dataTMP).message + " Username: " + serializer.DeserializeMessage(dataTMP).username);
            //Debug.Log("Socket listen for connection");

            //THIS IS SO DIRTY - TODO//
            if (!UDPClientList.Contains(clientEP) && clientEP.ToString() != "")
            {
                //Debug.Log("Adding a new remote conection point! :" + clientEP.ToString());
                UDPClientList.Add(clientEP);

                //Debug.Log("Size of Client List:" + UDPClientList.Count);
            }

            //Welcome Message!
            message = serializer.DeserializePackage(dataTMP);
            //Comunicate to the client what his new id is
            serverDirty = true;
            UpdateGameMatrix(UDPClientList.Count);
            newConection = true;
            SendData(message);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to receive data: " + e.Message);
        }

        //This has to be a diferent thread
        //loops the receive system. Messy but functional
        while (true)// Look at Promises, Async, Await
        {
            byte[] dataTMP = new byte[1024];
            //Carefull with this, there is a bug because we fullfill the byte[] buffer
            recv = udpSocket.ReceiveFrom(dataTMP, ref clientEP);

            //THIS IS SO DIRTY - TODO//
            if (!UDPClientList.Contains(clientEP) && clientEP.ToString() != "")
            {
                Debug.Log("Adding a new remote conection point! :" + clientEP.ToString());
                UDPClientList.Add(clientEP);
                newConection = true;

                //Debug.Log("Size of Client List:" + UDPClientList.Count);
            }

            Debug.Log("Socket listening from WHILE");
            message = serializer.DeserializePackage(dataTMP);
            Debug.Log("This player ID:" + thisPlayer.id);
            //EchoData(message);
            
            serverDirty = true;
            
        }
    }

    //Main communication funtion. It sends strings when called
    private void SendData(PlayerPackage _message)
    {
        try
        {
            byte[] dataTMP = new byte[1024];
            message.SetMessage(_message.message);
            Debug.Log("SERVER Sending message to " + clientEP.ToString() + ": " + _message.message);
            dataTMP = serializer.SerializePackage(message);
            udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, clientEP);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data. Error: " + e.Message);
        }
    }

    public void EchoData(PlayerPackage _message)
    {
        //SE LO ESTÀ ENVIANDO A SI MISMO
        try
        {
            byte[] dataTMP = new byte[1024];
            message.SetMessage(_message.message);
            dataTMP = serializer.SerializePackage(message);
            foreach (EndPoint ip in UDPClientList)
            {
                Debug.Log("SERVER Sending message to " + ip.ToString() + ": " + _message.message);
                udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, ip);
                Debug.Log("Echo to: " + ip);
            }
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data. Error: " + e.Message);
        }
    }

    public void CreateMonigote()
    {
        //If a new client has connected create the visual representation.

        //Every Client needs to have a list of monigotes that are linked to the SAME key as the data layer Dictionary.

        //If a message containing movement and a key arrives we modify the Monigote with the same key.
    }

    public void UpdateWorld(int _key, float[] _positions)
    {
        this.gameObject.GetComponent<WorldController>().MovePlayer(_key, _positions);
    }

    public void PingMovement(float[] packageMovement)
    {
        byte[] dataTMP = new byte[1024];
        try
        {
            message.SetMessage("");
            message.SetPositions(packageMovement);
            message.SetUsername(thisPlayer.username);
            message.SetId(thisPlayer.id);
            Debug.Log("Sending from Ping Server: ID: " + message.id);
            EchoData(message);
            //Debug.Log("[CLIENT] Sending to server: " + clientIPEP.ToString() + " Message: " + packageMovement[0] + "From:" + message.username);
            //dataTMP = serializer.SerializePackage(message);
            //recv = udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, clientEP);
            //carefull with data as it keeps setted, this can be so confusing if you cross it with a local dataTMP value, just to know.
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    public void UpdateGameMatrix(int id)
    {
        //gameMatrix[id] is the DATA value // id + 1 is the VISUAL VALUE ... id's will be 1,2,3,4 not 0,1,2,3
        gameMatrix[id] = id + 1;
        playersOnline++;
        Debug.Log("Matrix pos 0" + gameMatrix[0]);
        Debug.Log("Matrix pos 1" + gameMatrix[1]);
        Debug.Log("Matrix pos 2" + gameMatrix[2]);
        Debug.Log("Matrix pos 3" + gameMatrix[3]);
        message.SetWorldMatrix(gameMatrix);
        //we tell the client his position is the X on the matrix
        Debug.Log("Players Online Updating and Setting:" + playersOnline);
        message.SetPlayersOnline(playersOnline);
    }
}
