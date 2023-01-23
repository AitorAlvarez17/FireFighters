using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;

public class UDPClient : MonoBehaviour
{
    //CSP Vars
    public float timeStamp;
    public float RTT;
    public int PP = 100;

    public bool newRtt = false;
    // Servers'IP and port
    private string serverIP;
    private int serverPort;

    private TextMeshProUGUI matrixDebug;

    //Data matrix and number of bytes
    private int recv;
    private byte[] data = new byte[1024];

    public bool justConnected = false;

    // Message decoded for rendering on screen
    public string messageDecoded = "";

    //declare thread and socket
    private Thread clientThread;
    private Thread receiveThread = null;
    private Thread sendThread = null;

    private Socket udpSocket;

    // Server end point (Ip + Port)
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;

    public Player thisPlayer;
    public PlayerPackage receiveMessage = new PlayerPackage(null, "");
    public PlayerPackage sendMessage = new PlayerPackage(null, "");

    //This is the brain of the game
    public Tuple<int, int>[] gameMatrix = new Tuple<int, int>[4] { Tuple.Create(0, 100), Tuple.Create(0, 100), Tuple.Create(0, 100), Tuple.Create(0, 100) };
    public int playersOnline = 99;

    byte[] testBytes = new byte[1024];
    string testString = "";

    public bool isMoving = false;
    public bool fireChanged = false;
    public bool debugMatrix = false;
    public bool newConection = true;
    public bool startGame = false;
    public bool newMessage = false;

    //instanciation both variables
    void Start()
    {
        matrixDebug = ServerController.MyServerInstance.numberOfPlayers;
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;
        playersOnline = 99;

        
        //testBytes = serializer.SerializePlayerInfo(playerInfo);
    }

    public void Awake()
    {
        //PlayerManager.AddPlayer();
    }

    private void Update()
    {
        timeStamp = Time.realtimeSinceStartup;
        //Debug.Log(timeStamp + "ms");

        PlayerActions();

        
        if (thisPlayer != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.charge.SumWood(5);
                this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.PrintDebug();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.charge.SumWater(5);
                this.gameObject.GetComponent<WorldController>().worldDolls[thisPlayer.id].lumberjack.PrintDebug();
            }
        }
        
    }

    public void CreateMessage(PlayerPackage _Message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(this.gameObject.GetComponent<ServerController>().messgePrefab, Vector3.zero, Quaternion.identity, this.gameObject.GetComponent<ServerController>().chatBillboard.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_Message.message, _Message.username);
    }

    private void PlayerActions()
    {
        if (thisPlayer != null && thisPlayer.dirty == true)
        {
            if (startGame == true && this.transform.GetComponent<ServerController>().gameStarted == false)
            {
                this.transform.GetComponent<ServerController>().gameStarted = true;
                this.transform.GetComponent<ServerController>().HideInfo();
                startGame = false;
            }
            if (justConnected == true)
            {
                //carefull
                WelcomeWorld();
                sendMessage.SetUsername("Player" + thisPlayer.id);
                sendMessage.SetId(thisPlayer.id);
                justConnected = false;
                debugMatrix = true;
            }
            if (newConection == true && justConnected == false)
            {
                this.gameObject.GetComponent<WorldController>().WelcomeClient(gameMatrix, thisPlayer.id);
                debugMatrix = true;
                newConection = false;
            }
            if (newMessage == true)
            {
                //Debug.Log("Message checked and creating...!: " + receiveMessage.message + "From Client:" + receiveMessage.username);
                CreateMessage(receiveMessage);
                sendMessage.SetMessage("");
                newMessage = false;
            }
            if (newRtt == true)
            {
                this.gameObject.GetComponent<WorldController>().SetReckoningRTTS(RTT);
                newRtt = false;
            }
            if (isMoving == true)
            {
                MoveWorld(receiveMessage.id, receiveMessage.positions, receiveMessage.movementDirection);
            }
            if (fireChanged == true)
            {
                Debug.Log("Fire changed");
                this.gameObject.GetComponent<WorldController>().UpdateFires(gameMatrix);
                sendMessage.ClearCharge();
                debugMatrix = true;
                fireChanged = false;
            }
            if (debugMatrix == true)
            {
                DebugMatrix();
                debugMatrix = false;
            }

            this.gameObject.GetComponent<ServerController>().clientName.text = this.gameObject.GetComponent<UDPClient>().thisPlayer.username;
            thisPlayer.dirty = false;
        }
    }
    //closing both the socket and the thread on exit and all coroutines
    private void OnDisable()
    {
        Debug.Log("CLIENT Closing TCP socket & thread...");

        if (udpSocket != null)
            udpSocket.Close();
        if (clientThread != null)
            clientThread.Abort();

        StopAllCoroutines();
    }

    //Initialize socket and thread
    public void ConnectToServer(string ip = null, string username = "", int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;
        if (username != "")
            Debug.Log("Username");

        Debug.Log("Connected To Server!");

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
        clientThread = new Thread(ClientThread);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    //Main thread 
    private void ClientThread()
    {
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        serverEP = (EndPoint)serverIPEP;

        //Here the client is Blind, he doesn't know anything of the world
        thisPlayer = new Player("Player" + playersOnline.ToString(), true, playersOnline, 0, 0);
        sendMessage.SetUsername(thisPlayer.username);
        sendMessage.SetId(thisPlayer.id);
        sendMessage.SetPositions(thisPlayer.positions);
        sendMessage.SetWorldMatrix(gameMatrix);
        sendMessage.SetPlayersOnline(0);
        sendMessage.SetMessage("Hi! I just connected...");
        //Debug.Log("Resending the hello string!");
        SendString("Hi! I just connected...");

        // Receive from server and initialize the world
        try
        {
            recv = udpSocket.Receive(data);
            receiveMessage = serializer.DeserializePackage(data);
            Debug.Log("Receiving Back To Client");
            justConnected = true;
            thisPlayer.dirty = true;

            isMoving = false;


            

            receiveThread = new Thread(Receive);
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    
    //Main communication funtion. It sends strings when called
    public void SendString(string _message)
    {
        try
        {
            Debug.Log("Server Ep:" + serverEP);
            
            sendMessage.SetUsername("Player" + thisPlayer.id);
            sendMessage.SetMessage(_message);
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + _message + "From:" + sendMessage.username);
            data = serializer.SerializePackage(sendMessage);
            recv = udpSocket.SendTo(data, data.Length, SocketFlags.None, serverEP);

            //carefull with data as it keeps setted, this can be so confusing if you cross it with a local dataTMP value, just to know.
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    private void Send()
    {
        try
        {
            while (true)
            {
                Debug.Log("Sending");
                byte[] dataTMP = new byte[1024];

                sendMessage.SetTimeStamp(timeStamp);

                dataTMP = serializer.SerializePackage(sendMessage);
                udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, serverEP);

                //carefull with data as it keeps setted, this can be so confusing if you cross it with a local dataTMP value, just to know.
                //PP is the time between sent packets and is used right here.
                Thread.Sleep(PP);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error sending" + ex);
        }
    }

    private void Receive()
    {
        try
        {
            EndPoint Remote = (EndPoint)serverIPEP;
            while (true)
            {
                int recv;
                byte[] dataTMP = new byte[1024];

                recv = udpSocket.ReceiveFrom(dataTMP, ref Remote);
                receiveMessage = serializer.DeserializePackage(dataTMP);
                thisPlayer.dirty = true;

                RTT = timeStamp - receiveMessage.timeStamp;
                RTT = RTT / 2;

                if (receiveMessage.playersOnline > playersOnline)
                {
                    gameMatrix = receiveMessage.worldMatrix;
                    newConection = true;
                    Debug.Log("Players Online in the receive message: " + receiveMessage.playersOnline);
                    Debug.Log("From" + receiveMessage.id);
                    Debug.Log("Receiving in " + thisPlayer.id);
                    playersOnline = receiveMessage.playersOnline;
                }

                if (receiveMessage != null && receiveMessage.message != null && receiveMessage.message != "")
                {
                    newMessage = true;
                }
                //time in ms
                //RTT calculates the time that a packed lasts to go from client to server and comeback
                //we use RTT / 2 to calculate the avg time of traveling of client - server
                

                //Debug.Log("New RTT" + RTT + "Ms" + "from a packet of Player" + receiveMessage.id);


                if (receiveMessage.gameStarted == true)
                {
                    startGame = true;

                    sendThread = new Thread(Send);
                    sendThread.Start();
                }

                if (receiveMessage.id == thisPlayer.id)
                {
                    //RTT 
                    
                    if (RTT > 0)
                    {
                        Debug.LogWarning("New RTT from [ME] value: " + RTT + "s" + "at: " + timeStamp + "s playtime.");
                        newRtt = true;
                    }
                    //CHECK PP WITH TIMESTAMP
                    //Debug.Log("this was MINE");
                    if (receiveMessage.amount > 0)
                    {
                        Debug.Log("PingPong Received Fireplace: [ID: " + receiveMessage.fireID + "], [ACTION " + receiveMessage.fireAction + "], [AMOUNT: " + receiveMessage.amount + "");
                        //here confirm prediction!
                        gameMatrix = receiveMessage.worldMatrix;
                        fireChanged = true;
                    }

                    if (receiveMessage.velocity > 0f)
                    {
                        isMoving = true;
                    }
                    else
                    {
                        isMoving = false;
                    }
                    
                }
                else
                {
                    if (RTT > 0)
                    {
                        Debug.LogWarning("New RTT from [" + receiveMessage.id + "] with" + RTT + "s" + "at: " + timeStamp + "s playtime.");
                        newRtt = true;
                    }
                    //Debug.Log("this was NOT MINE" + "ID Receiving [" + receiveMessage.id + "]" + "Internal ID ["+ thisPlayer.id + "]" + "Fire life of receivingID" + receiveMessage.fireID);
                    //Debug.Log("This is not MINE!");
                    if (receiveMessage.amount > 0)
                    {
                        Debug.Log("Server Received Fireplace: [ID: " + receiveMessage.fireID + "], [ACTION " + receiveMessage.fireAction + "], [AMOUNT: " + receiveMessage.amount + "");
                        //here mimetize data!
                        gameMatrix = receiveMessage.worldMatrix;
                        fireChanged = true;
                    }

                    if (receiveMessage.velocity > 0f)
                    {
                        isMoving = true;
                    }
                    else
                    {
                        isMoving = false;
                    }
                }
                //Debug.Log("[CIENT] Receive data!: " + receiveMessage.message);
                //Debug.Log("[CLIENT] Received Id!" + receiveMessage.id);
            }
        }
        catch(Exception e)
        {
            Debug.Log("Recieve(): Error receiving: " + e);
        }
    }


    public void PingMovement(float[] packageMovement, float[]movementDirection, float _vel)
    {
        try
        {
            sendMessage.SetMessage("");
            sendMessage.SetPositions(packageMovement);
            sendMessage.SetDirection(movementDirection);
            sendMessage.SetVelocity(_vel);
            sendMessage.SetUsername(thisPlayer.username);
            sendMessage.SetId(thisPlayer.id);
            sendMessage.SetWorldMatrix(gameMatrix);
            sendMessage.SetPlayersOnline(playersOnline);


            //Debug.Log("Sending from" + sendMessage.id + "Movement with PlayersOnline:" + sendMessage.playersOnline);

            //Debug.Log("Pinging Mov from Client ID: " + sendMessage.id);

            //Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + packageMovement[0] + "From:" + message.username);
            
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    public void PingFireAction(int _id, int _action, int _amount, int _life)
    {
        try
        {
            //byte[] dataTMP = new byte[1024];
            //ping to everybody;
            sendMessage.SetFireAction(_id, _action, _amount, _life);

            //this is dangerous! as receiveMessage on ServerWill keep the same until next update, be sure that receivedMessage doesn't stuck the the old values

            //sendMessage.SetFireAction(_id, 0, 0, _life);
            Debug.Log("Pinging Fireplace: [ID: " + _id + "], [ACTION " + _action + "], [AMOUNT: " + _amount + "");

        }
        catch (Exception ex)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + ex.ToString());
        }
    }

    public void WelcomeWorld()
    {
        Debug.Log("[WELCOME WORLD], The number of players online is:" + receiveMessage.playersOnline);
        playersOnline = receiveMessage.playersOnline;
        gameMatrix = receiveMessage.worldMatrix;
        //this bc is the second pos but 1 in index
        thisPlayer.id = playersOnline;

        Debug.Log("Client was welcomed to world, ID:" + thisPlayer.id);
        this.gameObject.GetComponent<WorldController>().WelcomeClient(gameMatrix, thisPlayer.id);
    }


    public void MoveWorld(int _key, float[] _positions = null, float[] _directions = null, Tuple<int, int>[] newMatrix = null, int _life = -1)
    {
        this.gameObject.GetComponent<WorldController>().MovePlayer(_key, _positions, _directions, PP);
    }

    //public void UpdateGameMatrix(int _key, Tuple<int, int>[] newMatrix)
    //{
    //    //here there is a little controversy as gameMatrix element id-1 is copying newMatrix element id so there is a 1 "step" difference
    //    gameMatrix[_key - 1] = newMatrix[_key];
    //    debugMatrix = true;
    //}
    public void DebugMatrix()
    {
        Debug.Log("Debugging Matrix!");
        matrixDebug.text = "GAME MATRIX: \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[0].Item1 + "]" + "[LIFE: " + gameMatrix[0].Item2 + "] \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[1].Item1 + "]" + "[LIFE: " + gameMatrix[1].Item2 + "] \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[2].Item1 + "]" + "[LIFE: " + gameMatrix[2].Item2 + "] \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[3].Item1 + "]" + "[LIFE: " + gameMatrix[3].Item2 + "] \n";
    }

}
