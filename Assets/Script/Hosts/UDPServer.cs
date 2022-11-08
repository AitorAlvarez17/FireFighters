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

    // Message decoded for rendering on screen
    public string messageDecoded = null;
    public Message message = new Message(null, "Server");

    //declare Client's endpoint
    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

    //instanciation both variables and starts server
    void Start()
    {
        // Get IP and port
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;

        StartServer();
    }

    private void Update()
    {
        if (message != null && message.message != null)
        {
            Debug.Log("Message checked and creating:" + message.message + " From: " + message.username);
            CreateMessage(message);
            message.SetMessage(null);
            //print the messages that has been created
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
            Debug.Log("SERVER Message received from " + clientEP.ToString() + ": Message: " + serializer.DeserializeMessage(dataTMP).message + " Username: " + serializer.DeserializeMessage(dataTMP).username);
            Debug.Log("Receiving Message from Try Section!");
            message = serializer.DeserializeMessage(dataTMP);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to receive data: " + e.Message);
        }

        //loops the receive system. Messy but functional
        while (true)// Look at Promises, Async, Await
        {
            byte[] dataTMP = new byte[1024];
            //Carefull with this, there is a bug because we fullfill the byte[] buffer
            string s = BitConverter.ToString(dataTMP);
            //Debug.Log("Bytes: " + s);
            recv = udpSocket.ReceiveFrom(dataTMP, ref clientEP);
            Debug.Log("Receiving Message from While Section!");
            message = serializer.DeserializeMessage(dataTMP);
            SendData(message);
        }
    }

    //Main communication funtion. It sends strings when called
    private void SendData(Message _message)
    {
        try
        {
            byte[] dataTMP = new byte[1024];
            //message.SetMessage(_message);
            Debug.Log("SERVER Sending message to " + clientEP.ToString() + ": " + _message.message);
            dataTMP = serializer.SerializeMessage(message);
            udpSocket.SendTo(dataTMP, dataTMP.Length, SocketFlags.None, clientEP);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data. Error: " + e.Message);
        }
    }
}
