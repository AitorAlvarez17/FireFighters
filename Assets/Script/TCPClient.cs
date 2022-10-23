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
    private string serverIP;
    private int serverPort;

    private int recv;
    private byte[] data = new byte[1024];

    private Thread Thread;

    private Socket Socket;

    public string messageDecoded = null;
    private IPEndPoint serverIPEP;

    public Thread ClientThread1 { get => Thread; set => Thread = value; }

    void Start()
    {
        serverIP = ServerController.MyServerInstance.IPServer;
        serverPort = ServerController.MyServerInstance.serverPort;
    }

    private void OnDisable()
    {
        if (Socket != null)
            Socket.Close();
        if (Thread != null)
            Thread.Abort();
    }

    public void ConnectToServer(string ip = null, int port = 0)
    {
        if (ip != null)
            serverIP = ip;
        if (port != 0)
            serverPort = port;

        InitSocket();
        InitThread();
    }

    private void InitSocket()
    {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    private void InitThread()
    {
        Thread = new Thread(new ThreadStart(ClientThread));
        Thread.IsBackground = true;
        Thread.Start();
    }

    private void ClientThread()
    {
        IPAddress ipAddress = IPAddress.Parse(serverIP);
        serverIPEP = new IPEndPoint(ipAddress, serverPort);
        Socket.Connect(serverIPEP);

        SendString("Hello from client!");

        data = new byte[1024];
        recv = Socket.Receive(data);
        messageDecoded = Encoding.Default.GetString(data, 0, recv);
        Debug.Log("CLIENT Data from server: " + Encoding.Default.GetString(data, 0, recv));

        //Debug.Log("CLIENT Closing TCP socket...");
        //Socket.Close();
    }

    public void SendString(string message)
    {
        try
        {
            Debug.Log("CLIENT Sending to server: " + serverIPEP.ToString() + " Message: " + message);
            data = Encoding.Default.GetBytes(message);
            Socket.Send(data, data.Length, SocketFlags.None);
        }
        catch (Exception e)
        {
            Debug.Log("CLIENT Error sending string: " + e.ToString());
        }
    }

    
}
