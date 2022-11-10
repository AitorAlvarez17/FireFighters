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

    // Message decoded for rendering on screen
    public string messageDecoded = "";
    public Message message = new Message(null, "");

    //declare thread and socket
    private Thread clientThread;
    private Socket udpSocket;

    // Server end point (Ip + Port)
    private IPEndPoint serverIPEP;
    private EndPoint serverEP;

    public Player thisPlayer;

    //Debug Purposes
    public int playerKey = -1;

    //instanciation both variables
    void Start()
    {
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;
    }

    public void Awake()
    {
        //PlayerManager.AddPlayer();
    }

    private void Update()
    {
        PlayerActions();
    }
    public void CreateMessage(Message _Message)
    {
        GameObject newMessage = new GameObject();
        newMessage = Instantiate(this.gameObject.GetComponent<ServerController>().messgePrefab, Vector3.zero, Quaternion.identity, this.gameObject.GetComponent<ServerController>().chatBillboard.transform);
        newMessage.GetComponent<MessageHolder>().SetMessage(_Message.message, _Message.username);
    }
    private void PlayerActions()
    {
        if (thisPlayer != null && thisPlayer.dirty == true)
        {
            if (message != null && message.message != null)
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
        playerKey = thisPlayer.id;
        SendString("Hi! I just connected...");

        // Receive from server
        try
        {
            recv = udpSocket.Receive(data);
            message = serializer.DeserializeMessage(data);
            Debug.Log("Receiving! A");
            thisPlayer.dirty = true;
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
            Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + " Message: " + _message + "From:" + message.username);
            data = serializer.SerializeMessage(message);
            recv = udpSocket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        }
    }

    public void PingMovement()
    {
        Debug.Log("Position on X in Player Class: " + thisPlayer.positions[0]);
        Debug.Log("Position on Z in Player Class: " + thisPlayer.positions[2]);
        //try
        //{
        //    Debug.Log("[CLIENT] Sending to server: " + serverIPEP.ToString() + "Position");
        //    data = serializer.SerializePlayerInfo(thisPlayer.positions);
        //    recv = udpSocket.SendTo(data, data.Length, SocketFlags.None, serverEP);
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("[CLIENT] Failed to send message. Error: " + e.ToString());
        //}
    }
}
