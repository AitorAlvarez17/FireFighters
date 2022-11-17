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
    public PlayerPackage message = new PlayerPackage(null, "");

    //public PlayerInfo playerInfo = new PlayerInfo(new PlayerPackage("test", "Username", thisPlayer.positions));

    byte[] testBytes = new byte[1024];
    string testString = "";

    //instanciation both variables
    void Start()
    {
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;
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
                WelcomeWorld();
                Debug.Log("Creating Server repre");
                justConnected = false;
            }
            if (message != null && message.message != null && message.message != "")
            {
                Debug.Log("Message checked and creating...!: " + message.message + "From Client:" + message.username);
                //Later on take it from PlayerManager! Now just hard-took it for debug purposes.
                //You can easily acces to the player with the key (index) of it
                CreateMessage(message);
                message.SetMessage(null);

                //print the messages that has been created
            }

            Debug.Log("Setting Text and dirtyness");
            this.gameObject.GetComponent<ServerController>().clientName.text = this.gameObject.GetComponent<UDPClient>().thisPlayer.username;
            //clientIndex.text = serverParent.GetComponent<UDPClient>().thisPlayer.onLine;
            thisPlayer.dirty = false;
        }
    }
    //closing both the socket and the thread on exit and all coroutines
    private void OnDisable()
    {

        //testString = serializer.DeserializePlayerInfo(testBytes).message.message;
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

        //Really good place for name personalization
        thisPlayer = PlayerManager.AddPlayer("Player");
        message.SetUsername(thisPlayer.username);
        message.SetId(thisPlayer.id);
        Debug.Log("Resending the hello string!");
        SendString("Hi! I just connected...");

        // Receive from server
        try
        {
            recv = udpSocket.Receive(data);
            message = serializer.DeserializePackage(data);
            Debug.Log("Receiving! A");
            justConnected = true;
            thisPlayer.dirty = true;

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
            message.SetMessage(_message);
            message.SetUsername(thisPlayer.username);
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + _message + "From:" + message.username);
            data = serializer.SerializePackage(message);
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
                message = serializer.DeserializePackage(dataTMP);
                thisPlayer.dirty = true;

                //Update world
                //PlayerManager.UpdatePlayerPosition(message.Key, message.positions);

                Debug.Log("[CIENT] Receive data!: " + message.message);

                Debug.Log("[CLIENT] Received Movement!" + message.positions[0] + message.positions[1]+ message.positions[2]);
                Debug.Log("[CLIENT] Received Id!" + message.id);
            }
        }
        catch(Exception e)
        {
            Debug.Log("Recieve(): Error receiving: " + e);
        }
    }


    public void PingMovement(float[] packageMovement)
    {
        Debug.Log("Message: " + message.message);
        Debug.Log("Username: " + message.username);
        Debug.Log("Pos X: " + message.positions[0]);
        try
        {
            message.SetMessage("");
            message.SetPositions(packageMovement);
            message.SetUsername(thisPlayer.username);
            message.SetId(thisPlayer.id);
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + packageMovement[0] + "From:" + message.username);
            data = serializer.SerializePackage(message);
            recv = udpSocket.SendTo(data, data.Length, SocketFlags.None, serverEP);

            //carefull with data as it keeps setted, this can be so confusing if you cross it with a local dataTMP value, just to know.
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    public void WelcomeWorld()
    {
        this.gameObject.GetComponent<WorldController>().WelcomeClient(thisPlayer.id);
    }
}
