using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

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
    // Message decoded for rendering on screen
    public string messageDecoded = null;
    public Message message = new Message(null, "Server");
    public PlayerInfo playerInfo = new PlayerInfo(new Message(null, ""), { 0, 0, 0 });

    //declare Client's endpoint
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

    public List<EndPoint> UDPClientList = new List<EndPoint>();

    //instanciation both variables and starts server
    void Start()
    {
        serverDirty = false;
        // Get IP and port
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;

        StartServer();
    }

    private void Update()
    {
        ServerActions();
        
    }

    private void ServerActions()
    {
        if (serverDirty == true)
        {
            if (playerInfo.message != null && playerInfo.message.message != null)
            {
                Debug.Log("Message checked and creating:" + message.message + " From: " + message.username);
                CreateMessage(playerInfo.message);
                playerInfo.message.SetMessage(null);
                //print the messages that has been created
            }
            this.gameObject.GetComponent<ServerController>().numberOfPlayers.text = "Number of Players: " + PlayerManager.playersOnline;
            serverDirty = false;
            Debug.Log("Setting Text and Server Dirtyness");
        }
    }

    public void CreateMessage(Message _Message)
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
            Debug.Log("Socket listen for connection");

            //THIS IS SO DIRTY - TODO//
            if (!UDPClientList.Contains(clientEP) && clientEP.ToString() != "")
            {
                Debug.Log("Adding a new remote conection point! :" + clientEP.ToString());
                UDPClientList.Add(clientEP);

                Debug.Log("Size of Client List:" + UDPClientList.Count);
            }

            playerInfo.message = serializer.DeserializeMessage(dataTMP);
            serverDirty = true;
            //This is kind of a ping but we set it as a message but it's just PINGING
            SendData(playerInfo.message);
            Thread.Sleep(100);
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

                Debug.Log("Size of Client List:" + UDPClientList.Count);
            }

            Debug.Log("Socket listening from WHILE");
        //if data tmp header == message send the message otherwise send PlayerInfo


            playerInfo.message = serializer.DeserializeMessage(dataTMP);
            Debug.Log("Sending! B");
            EchoServerMessage(playerInfo.message);
            serverDirty = true;
            Thread.Sleep(100);
        }
    }

    //Main communication funtion. It sends strings when called
    private void SendData(Message _message)
    {
        try
        {
            byte[] dataTMP = new byte[1024];
            playerInfo.message.SetMessage(_message.message);
            Debug.Log("SERVER Sending message to " + clientEP.ToString() + ": " + _message.message);
            dataTMP = serializer.SerializeMessage(message);
            udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, clientEP);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data. Error: " + e.Message);
        }
    }

    public void EchoServerMessage(Message _message)
    {
        //SE LO EST� ENVIANDO A SI MISMO
        try
        {
            Debug.Log("Header: " + _message.header);
            byte[] dataTMP = new byte[1024];
            playerInfo.message.SetMessage(_message.message);
            dataTMP = serializer.SerializeMessage(playerInfo.message);
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

}