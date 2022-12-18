using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

//Do a parent class that is MonoBehaviour and make this heritage from the parent in order to make it virtual for PlayerMovement
public class UDPServer : MonoBehaviour
{
    public float timeStamp;
    // Clients'IP and Port
    private string serverIP;
    private int serverPort;

    private TextMeshProUGUI matrixDebug;
    private TextMeshProUGUI IpText;

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

    // This is the brain of the game - the first int stands for the ID of the Sawmill (Lumberjack + Fireplace) and the seconds stands for the health of it
    public Tuple<int, int>[] gameMatrix = new Tuple<int, int>[4] { Tuple.Create(0,100), Tuple.Create(0, 100), Tuple.Create(0, 100), Tuple.Create(0, 100) };
    public int playersOnline = 0;
    public bool thisPlayerSetup = false;


    //Client
    public bool isMoving = false;
    public bool fireChanging = false;
    public bool debugMatrix = false;

    // Instanciation both variables and starts server
    void Start()
    {
        serverDirty = false;
        playersOnline = UDPClientList.Count;

        matrixDebug = this.gameObject.GetComponent<ServerController>().numberOfPlayers;
        IpText = this.gameObject.GetComponent<ServerController>().IpText;

        // Get IP and port
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;

        StartServer();
    }

    private void Update()
    {
        timeStamp = Time.realtimeSinceStartup;
        //Debug.Log(timeStamp + "ms");

        ServerActions();

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.charge.SumWood(5);
        //    this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.PrintDebug();
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.charge.SumWater(5);
        //    this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.PrintDebug();
        //}
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
        //UpdateGameMatrix(playersOnline);
        sendMessage.SetWorldMatrix(gameMatrix);
        sendMessage.SetPlayersOnline(playersOnline);
        serverDirty = true;
        thisPlayerSetup = true;
    }

    private void ServerActions()
    {
        if (serverDirty == false)
            return;
            

            //Well positioned function
            if (initServer == true)
            {
                  IpText.text = "IP:" + serverIP;
                  this.gameObject.GetComponent<UDPClient>().ConnectToServer(serverIP, "Pending");
                  //EchoData(rece); /*<- Here echo the players online to all clients and in Client create the players*/
                  ////this.gameObject.GetComponent<WorldController>().CreatePlayer(playersOnline);
                  ////DebugMatrix();
                  initServer = false;
                //Debug.Log("Message checked and creating:" + receivedMessage.message + " From: " + receivedMessage.username);
            }
           
            serverDirty = false;
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
            receivedMessage = serializer.DeserializePackage(dataTMP);

            if (!UDPClientList.Contains(clientEP) && clientEP.ToString() != "")
            {
                Debug.Log("New client added to the list");
                UDPClientList.Add(clientEP);
                UpdateGameMatrix(UDPClientList.Count);
                ModifyReceivedMessage();
            }
            // Comunicate to the client what his new id is
            serverDirty = true;
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
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 9050);
                EndPoint clientEP = (EndPoint)(sender);

                int recv = 0;
                byte[] dataTMP = new byte[1024];
                // Carefull with this, there is a bug because we fullfill the byte[] buffer
                recv = udpSocket.ReceiveFrom(dataTMP, ref clientEP);
                receivedMessage = serializer.DeserializePackage(dataTMP);

                if (!UDPClientList.Contains(clientEP) && clientEP.ToString() != "")
                {
                    Debug.Log("Adding a new remote conection point! :" + clientEP.ToString());
                    UDPClientList.Add(clientEP);
                    UpdateGameMatrix(UDPClientList.Count);
                    ModifyReceivedMessage();
                }

                if (receivedMessage.amount > 0)
                {
                    fireChanging = true;
                    //here we change the matrix with the new life
                    UpdateFireMatrix(receivedMessage.fireID, receivedMessage.fireAction, receivedMessage.amount, receivedMessage.fireLife);
                    //and here we change the receivedMessage for pingPong comeback
                    ModifyReceivedMessage();
                }
                Debug.Log("[SERVER] Received message ID:" + receivedMessage.id);

                EchoData(receivedMessage);

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
                //Debug.Log("SERVER Sending message to " + ip.ToString() + ": " + _message.message);
                udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, ip);
                //Debug.Log("Echo to: " + ip);
            }
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data. Error: " + e.Message);
        }
    }


    //public void UpdateWorld(int _key, float[] _positions)
    //{
    //    this.gameObject.GetComponent<WorldController>().MovePlayer(_key, _positions);
    //}

    #region Pings
    public void PingGameStarted(bool state)
    {
        byte[] dataTMP = new byte[1024];
        try
        {
            Debug.Log("GAME STARTED!");
            sendMessage.SetMessage("");
            sendMessage.SetGameState(state);
            //Debug.Log("Sending from Ping Server: ID: " + sendMessage.id);
            EchoData(sendMessage);
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    #endregion

    #region UpdateMatrix
    public void UpdateGameMatrix(int id)
    {
        // gameMatrix[id] is the DATA value // id + 1 is the VISUAL VALUE ... id's will be 1,2,3,4 not 0,1,2,3
        gameMatrix[id - 1] = Tuple.Create(id, 100);
        playersOnline++;
        

        // We tell the client his position is the X on the matrix
        Debug.Log("Players Online Updating and Setting:" + playersOnline);
    }

    public void UpdateFireMatrix(int _fireID, int _type, int amount, int life)
    {
        //nice place for ANTICHEATING comprovations - SECURING the message
        int _newLife = life;
        switch (_type)
        {
            case 0:
                Debug.Log("No charge");
                break;
            case 1:
                _newLife = life - amount;
                _newLife += amount;
                break;
            case 2:
                _newLife = life + amount;
                _newLife -= amount;
                break;
            default:
                break;
        }

        gameMatrix[_fireID - 1] = Tuple.Create(_fireID, _newLife);
        debugMatrix = true;
        serverDirty = true;
    }

    public void ModifyReceivedMessage()
    {
        receivedMessage.SetWorldMatrix(gameMatrix);
        receivedMessage.SetPlayersOnline(playersOnline);
    }

    public void DebugMatrix()
    {
        Debug.Log("Debuging the Matrix");

        matrixDebug.text = "GAME MATRIX: \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[0].Item1 + "]" + "[LIFE: " + gameMatrix[0].Item2 + "]\n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[1].Item1 + "]" + "[LIFE: " + gameMatrix[1].Item2 + "]\n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[2].Item1 + "]" + "[LIFE: " + gameMatrix[2].Item2 + "]\n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[3].Item1 + "]" + "[LIFE: " + gameMatrix[3].Item2 + "]\n";
    }

    #endregion
}
