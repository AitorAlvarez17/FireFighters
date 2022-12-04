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
            if (justConnected == true)
            {
                receiveMessage.SetUsername("Player" + thisPlayer.id);
                WelcomeWorld();
                DebugMatrix();
                Debug.Log("Creating Server repre");
                justConnected = false;
            }
            if (receiveMessage != null && receiveMessage.message != null && receiveMessage.message != "")
            {
                //Debug.Log("Message checked and creating...!: " + receiveMessage.message + "From Client:" + receiveMessage.username);
                CreateMessage(receiveMessage);
                receiveMessage.SetMessage(null);
            }
            if (receiveMessage.positions[0] != 0f || receiveMessage.positions[2] != 0f && isMoving == true)
            {
                //Debug.Log("This player ID (check):" + thisPlayer.id);
                //Debug.Log("Received message ID: " + receiveMessage.id);
                UpdateWorld(1, receiveMessage.id, receiveMessage.positions);
            }
            if (fireChanged == true)
            {
                UpdateWorld(2, receiveMessage.id, receiveMessage.positions, receiveMessage.worldMatrix, receiveMessage.fireLife);
                fireChanged = false;
            }

            //Debug.Log("Setting Text and dirtyness");
            this.gameObject.GetComponent<ServerController>().clientName.text = this.gameObject.GetComponent<UDPClient>().thisPlayer.username;
            thisPlayer.dirty = false;
        }
    }
    //closing both the socket and the thread on exit and all coroutines
    private void OnDisable()
    {

        Debug.Log("Test JSON Serialization:" + testString);
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

                if (receiveMessage.id == thisPlayer.id)
                {
                    //Debug.Log("Not Moving, this was MINE");
                    if (receiveMessage.amount > 0)
                    {
                        Debug.Log("PingPong Received Fireplace: [ID: " + receiveMessage.fireID + "], [ACTION " + receiveMessage.fireAction + "], [AMOUNT: " + receiveMessage.amount + "");
                        //here confirm prediction!
                        fireChanged = true;
                    }

                    isMoving = false;
                }
                else
                {
                    //Debug.Log("This is not MINE!");
                    if (receiveMessage.amount > 0)
                    {
                        Debug.Log("Server Received Fireplace: [ID: " + receiveMessage.fireID + "], [ACTION " + receiveMessage.fireAction + "], [AMOUNT: " + receiveMessage.amount + "");
                        //here mimetize data!
                        fireChanged = true;
                    }

                    isMoving = true;
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


    public void PingMovement(float[] packageMovement)
    {
        try
        {
            byte[] dataTMP = new byte[1024];
            sendMessage.SetMessage("");
            sendMessage.SetPositions(packageMovement);
            sendMessage.SetUsername(thisPlayer.username);
            sendMessage.SetId(thisPlayer.id);
            //Debug.Log("Pinging Mov from Client ID: " + sendMessage.id);

            //Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + packageMovement[0] + "From:" + message.username);
            dataTMP = serializer.SerializePackage(sendMessage);
            udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, serverEP);

            //carefull with data as it keeps setted, this can be so confusing if you cross it with a local dataTMP value, just to know.
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
            byte[] dataTMP = new byte[1024];
            //ping to everybody;
            sendMessage.SetFireAction(_id, _action, _amount, _life);
            dataTMP = serializer.SerializePackage(sendMessage);
            udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, serverEP);

            //this is dangerous! as receiveMessage on ServerWill keep the same until next update, be sure that receivedMessage doesn't stuck the the old values
            sendMessage.SetFireAction(_id, 0, 0, -1);

            //sendMessage.SetFireAction(0, 0);
            Debug.Log("Interacting with Fireplace: [ID: " + _id + "], [ACTION " + _action + "], [AMOUNT: " + _amount + "");
        }
        catch (Exception ex)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + ex.ToString());
        }
    }

    public void WelcomeWorld()
    {
        Debug.Log("The number of players online is:" + receiveMessage.playersOnline);
        playersOnline = receiveMessage.playersOnline;
        gameMatrix = receiveMessage.worldMatrix;
        //this bc is the second pos but 1 in index
        thisPlayer.id = playersOnline;

        Debug.Log("Client was welcomed to world, ID:" + thisPlayer.id);
        this.gameObject.GetComponent<WorldController>().WelcomeClient(gameMatrix, thisPlayer.id);
    }

    public void UpdateWorld(int action, int _key, float[] _positions = null, Tuple<int, int>[] newMatrix = null, int _life = 0)
    {
        switch (action)
        {
            case 1:
                this.gameObject.GetComponent<WorldController>().MovePlayer(_key, _positions);
                break;
            case 2:
                //here i copy the life directly bc it has been checked by server and you need no longer comprovation, also
                //if we want to override a client prediciton is nice to just equal the life to the new one.
                UpdateGameMatrix(_key, newMatrix);
                this.gameObject.GetComponent<WorldController>().SetFireLife(_key, _life);
                break;
            default:
                break;
        }
    }

    public void UpdateGameMatrix(int _key, Tuple<int, int>[] newMatrix)
    {
        gameMatrix[_key] = newMatrix[_key];
        DebugMatrix();
    }
    public void DebugMatrix()
    {
        matrixDebug.text = "GAME MATRIX: \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[0].Item1 + "]" + "[LIFE: " + gameMatrix[0].Item2 + "] \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[1].Item1 + "]" + "[LIFE: " + gameMatrix[1].Item2 + "] \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[2].Item1 + "]" + "[LIFE: " + gameMatrix[2].Item2 + "] \n";
        matrixDebug.text += "Matrix [ID: " + gameMatrix[3].Item1 + "]" + "[LIFE: " + gameMatrix[3].Item2 + "] \n";
    }
}
