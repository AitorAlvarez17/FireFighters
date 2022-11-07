using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Text;
using System;

public class TCPClient : MonoBehaviour
{

    // Servers'IP and port
    private string serverIP;
    private int serverPort;

    //Data matrix and number of bytes
    private int recv;
    private byte[] data = new byte[1024];

    //declare thread and socket
    private Thread Thread;
    private Socket Socket;
    // Message decoded for rendering on screen
    public string messageDecoded = null;

    // Server end point (Ip + Port)
    private IPEndPoint serverIPEP;

    //Client Thread
    public Thread ClientThread1 { get => Thread; set => Thread = value; }


    //instanciation both variables
    void Start()
    {
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;
    }

    //closing both the socket and the thread on exit and all coroutines
    private void OnDisable()
    {
        Debug.Log("CLIENT Closing TCP socket & thread...");

        if (Socket != null)
            Socket.Close();
        if (Thread != null)
            Thread.Abort();

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
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    //set and initialize thread
    private void InitThread()
    {
        Thread = new Thread(new ThreadStart(ClientThread));
        Thread.IsBackground = true;
        Thread.Start();
    }

    //Main thread 
    private void ClientThread()
    {
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        Socket.Connect(serverIPEP);

        //send data
        SendString("Hello from client!");

        //receive
        data = new byte[1024];
        recv = Socket.Receive(data);
        messageDecoded = serializer.DeserializeInfo(data);
        Debug.Log("CLIENT Data from server: " + Encoding.Default.GetString(data, 0, recv));

    }

    //Main communication funtion. It sends strings when called
    public void SendString(string message)
    {
        try
        {
            Debug.Log("CLIENT Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            data = serializer.SerializeInfo(message);
            Socket.Send(data, data.Length, SocketFlags.None);
        }
        catch (Exception e)
        {
            Debug.Log("CLIENT Error sending string: " + e.ToString());
        }
    }

    
}
