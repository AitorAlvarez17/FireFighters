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
    // Clients'IP and Port
    private string serverIP;
    private int serverPort;

    // Data matrix and number of bytes
    private int recv;

    // Declare Thread and Socket
    private Thread serverThread;
    private Thread receiveThread = null;
    private Socket udpSocket;

    public bool serverDirty = false;
    public bool newConection = false;

    // Message decoded for rendering on screen
    public string messageDecoded = null;

    public Player thisPlayer;

    // As the game is threaded we gotta difference from message Receive and message Send
    public PlayerPackage receivedMessage = new PlayerPackage(null, "Server");
    public PlayerPackage sendMessage = new PlayerPackage(null, "Server");

    public bool onLine = false;
    public bool initServer = false;

    // Declare Client's endpoint
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

    public List<EndPoint> UDPClientList = new List<EndPoint>();

    // This is the brain of the game
    public int[] gameMatrix = new int[4] { 0, 0, 0, 0 };
    public int playersOnline = 0;
    public bool thisPlayerSetup = false;

    public bool isMoving = false;

    // Instanciation both variables and starts server
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
        //thisPlayer.username = username;
        //onLine = true;

        //this.gameObject.GetComponent<ServerController>().clientName.text = thisPlayer.username;
        //this.gameObject.GetComponent<ReadServer>().LoginInput.SetActive(false);
        //this.gameObject.GetComponent<PlayerMovement>().player.GetComponent<Lumberjack>().Init(thisPlayer.id, thisPlayer.username);

    }

    public void SetServerInfo()
    {
        sendMessage.SetUsername(thisPlayer.username);
        sendMessage.SetId(thisPlayer.id);
        sendMessage.SetPositions(thisPlayer.positions);
        UpdateGameMatrix(playersOnline);
        sendMessage.SetWorldMatrix(gameMatrix);
        sendMessage.SetPlayersOnline(playersOnline);
        serverDirty = true;
        thisPlayerSetup = true;
    }
    private void ServerActions()
    {
        if (serverDirty == false)
            return;

            if (thisPlayerSetup == true)
            {
                this.gameObject.GetComponent<PlayerMovement>().player.GetComponent<Lumberjack>().Init(sendMessage.id, sendMessage.username);
                thisPlayerSetup = false;
            }
            if (receivedMessage != null && receivedMessage.message != null && receivedMessage.message != "")
            {
                Debug.Log("Message checked and creating:" + receivedMessage.message + " From: " + receivedMessage.username);
                CreateMessage(receivedMessage);
                receivedMessage.SetMessage(null);
            }
            if (newConection == true)
            {
                this.gameObject.GetComponent<WorldController>().CreatePlayer(playersOnline, receivedMessage.username);
                newConection = false;
            }
            this.gameObject.GetComponent<ServerController>().numberOfPlayers.text = "Number of Players: " + PlayerManager.playersOnline;
            if (receivedMessage.positions[0] != 0f || receivedMessage.positions[2] != 0f && isMoving == true)
            {
                Debug.Log("Server Player ID:" + thisPlayer.id);
                Debug.Log("Message ID:" + receivedMessage.id);
                UpdateWorld(receivedMessage.id, receivedMessage.positions);
            }
            serverDirty = false;
            Debug.Log("Setting Text and Server Dirtyness");
        
    }

    public void CreateMessage(PlayerPackage _Message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(this.gameObject.GetComponent<ServerController>().messgePrefab, Vector3.zero, Quaternion.identity, this.gameObject.GetComponent<ServerController>().chatBillboard.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_Message.message, _Message.username);
    }

    // Closing both the socket and the thread on exit and all coroutines
    private void OnDisable()
    {
        if (udpSocket != null)
            udpSocket.Close();
        if (serverThread != null)
            serverThread.Abort();
    }

    // Initialize socket and thread
    private void StartServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;

        InitSocket();
        InitThread();
    }

    // Set socket
    private void InitSocket()
    {
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    // Set and initialize thread
    private void InitThread()
    {
        serverThread = new Thread(ServerThread);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    // Main thread 
    private void ServerThread()
    {
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        clientIPEP = new IPEndPoint(ipAddress, serverPort);
        clientEP = (EndPoint)clientIPEP;

        thisPlayer = new Player("Player" + (playersOnline + 1).ToString(), true, (playersOnline + 1), 0, 0);
        Debug.Log("BEGINNING OF THE GENERAL SERVER THREAD");
        SetServerInfo();
        initServer = true;

        // Try the socket's bind, if not debugs
        try
        {
            udpSocket.Bind(clientIPEP);
            Debug.Log("SERVER UDP socket bound to " + clientIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to bind socket: " + e.Message);
        }

        // FIRST RECEIVE
        try
        {
            byte[] dataTMP = new byte[1024];
            recv = udpSocket.ReceiveFrom(dataTMP, ref clientEP);

            if (!UDPClientList.Contains(clientEP) && clientEP.ToString() != "")
            {
                UDPClientList.Add(clientEP);
                UpdateGameMatrix(UDPClientList.Count);

            }

            // Welcome Message!
            receivedMessage = serializer.DeserializePackage(dataTMP);
            ModifyReceivedMessage();

            // Comunicate to the client what his new id is
            serverDirty = true;
            newConection = true;
            isMoving = false;
            SendData(receivedMessage);

            receiveThread = new Thread(Receive);
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to receive data: " + e.Message);
        }

        // This has to be a diferent thread
        // loops the receive system. Messy but functional

    }

    private void Receive()
    {
        try
        {
            while (true)// Look at Promises, Async, Await
            {
                byte[] dataTMP = new byte[1024];
                // Carefull with this, there is a bug because we fullfill the byte[] buffer
                recv = udpSocket.ReceiveFrom(dataTMP, ref clientEP);

                if (!UDPClientList.Contains(clientEP) && clientEP.ToString() != "")
                {
                    Debug.Log("Adding a new remote conection point! :" + clientEP.ToString());
                    UDPClientList.Add(clientEP);
                    newConection = true;
                }

                if (receivedMessage.id == thisPlayer.id)
                {
                    Debug.Log("Not Moving, this was MINE");
                    isMoving = false;
                }
                else
                {
                    Debug.Log("This is not MINE!");
                    isMoving = true;
                }

                receivedMessage = serializer.DeserializePackage(dataTMP);
                ModifyReceivedMessage();
                Debug.Log("[SERVER] Received message ID:" + receivedMessage.id);
                EchoData(receivedMessage);

                Debug.Log("Fire action: " + receivedMessage.fireAction + "With an amount of" + receivedMessage.amount);

                serverDirty = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Recieve(): Error receiving: " + e);
        }
    }

    // Main communication funtion. It sends strings when called
    private void SendData(PlayerPackage _message)
    {
        try
        {
            byte[] dataTMP = new byte[1024];
            Debug.Log("SERVER Sending message to " + clientEP.ToString() + ": " + _message.message);
            dataTMP = serializer.SerializePackage(_message);
            udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, clientEP);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data. Error: " + e.Message);
        }
    }

    public void EchoData(PlayerPackage _message)
    {
        try
        {
            byte[] dataTMP = new byte[1024];
            dataTMP = serializer.SerializePackage(_message);
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

    public void UpdateWorld(int _key, float[] _positions)
    {
        this.gameObject.GetComponent<WorldController>().MovePlayer(_key, _positions);
    }

    public void PingMovement(float[] packageMovement)
    {
        byte[] dataTMP = new byte[1024];
        try
        {
            sendMessage.SetMessage("");
            sendMessage.SetPositions(packageMovement);
            sendMessage.SetUsername(thisPlayer.username);
            sendMessage.SetId(thisPlayer.id);
            //Debug.Log("Sending from Ping Server: ID: " + sendMessage.id);
            EchoData(sendMessage);
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    public void PingFireAction(int action, int amount)
    {
        try
        {
            byte[] dataTMP = new byte[1024];
            //ping to everybody;
            sendMessage.SetFireAction(action, amount);
            EchoData(sendMessage);
            Debug.Log("Interacting with fireplace");
        }
        catch (Exception ex)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + ex.ToString());
        }
    }

    public void UpdateGameMatrix(int id)
    {
        // gameMatrix[id] is the DATA value // id + 1 is the VISUAL VALUE ... id's will be 1,2,3,4 not 0,1,2,3
        gameMatrix[id] = id + 1;
        playersOnline++;
        Debug.Log("Matrix pos 0" + gameMatrix[0]);
        Debug.Log("Matrix pos 1" + gameMatrix[1]);
        Debug.Log("Matrix pos 2" + gameMatrix[2]);
        Debug.Log("Matrix pos 3" + gameMatrix[3]);
        
        // We tell the client his position is the X on the matrix
        Debug.Log("Players Online Updating and Setting:" + playersOnline);
    }

    public void ModifyReceivedMessage()
    {
        receivedMessage.SetWorldMatrix(gameMatrix);
        receivedMessage.SetPlayersOnline(playersOnline);
    }
}
