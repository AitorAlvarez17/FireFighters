using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    // Servers'IP and port
    private string serverIP;
    private int serverPort;

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
    public int[] gameMatrix = new int[4] { 0, 0, 0, 0 };
    public int playersOnline = 99;

    byte[] testBytes = new byte[1024];
    string testString = "";

    public bool isMoving = false;

    //instanciation both variables
    void Start()
    {
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
                Debug.Log("Creating Server repre");
                justConnected = false;
            }
            if (receiveMessage != null && receiveMessage.message != null && receiveMessage.message != "")
            {
                Debug.Log("Message checked and creating...!: " + receiveMessage.message + "From Client:" + receiveMessage.username);
                CreateMessage(receiveMessage);
                receiveMessage.SetMessage(null);
            }
            if (receiveMessage.positions[0] != 0f || receiveMessage.positions[2] != 0f && isMoving == true)
            {
                Debug.Log("This player ID (check):" + thisPlayer.id);
                Debug.Log("Received message ID: " + receiveMessage.id);
                UpdateWorld(receiveMessage.id, receiveMessage.positions);
            }

            Debug.Log("Setting Text and dirtyness");
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
    public void ConnectToServer(string ip = null, int port = 0)
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
        thisPlayer = new Player("Player" + playersOnline.ToString(), true, playersOnline);
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
            Debug.Log("Receiving Back To Client from TRY");
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
            while(true)
            {
                int recv;
                byte[] dataTMP = new byte[1024];

                recv = udpSocket.Receive(dataTMP);
                receiveMessage = serializer.DeserializePackage(dataTMP);
                thisPlayer.dirty = true;

                if (receiveMessage.id == thisPlayer.id)
                {
                    Debug.Log("Not Moving, this was MINE");
                    isMoving = false;
                }
                else
                {
                    Debug.Log("This is not MINE!");
                    isMoving = true;
                }

                Debug.Log("[CIENT] Receive data!: " + receiveMessage.message);
                Debug.Log("[CLIENT] Received Id!" + receiveMessage.id);
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
            Debug.Log("Pinging Mov from Client ID: " + sendMessage.id);

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

    public void WelcomeWorld()
    {
        Debug.Log("The number of players online is:" + receiveMessage.playersOnline);
        playersOnline = receiveMessage.playersOnline;
        gameMatrix = receiveMessage.worldMatrix;
        //this bc is the second pos but 1 in index
        thisPlayer.id = playersOnline;
        this.gameObject.GetComponent<PlayerMovement>().player.GetComponent<Lumberjack>().Init(thisPlayer.id, thisPlayer.username);
        Debug.Log("Client was welcomed to world, ID:" + thisPlayer.id);
        this.gameObject.GetComponent<WorldController>().WelcomeClient(gameMatrix, thisPlayer.id, receiveMessage.username);
    }

    public void UpdateWorld(int _key, float[] _positions)
    {
        this.gameObject.GetComponent<WorldController>().MovePlayer(_key, _positions);
    }
}
