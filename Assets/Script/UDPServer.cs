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
    private byte[] data = new byte[1024];

    //declare thread and socket
    private Thread serverThread;
    private Socket udpSocket;

    // Message decoded for rendering on screen
    public string messageDecoded = null;

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
            recv = udpSocket.ReceiveFrom(data, ref clientEP);
            Debug.Log("SERVER Message received from " + clientEP.ToString() + ": " + Encoding.Default.GetString(data, 0, recv));
            messageDecoded = Encoding.Default.GetString(data, 0, recv);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to receive data: " + e.Message);
        }

        //loops the receive system. Messy but functional
        while (true)// Look at Promises, Async, Await
        {
            recv = udpSocket.ReceiveFrom(data, ref clientEP);
            messageDecoded = Encoding.Default.GetString(data, 0, recv);
            SendData("received message");

        }
    }

    //Main communication funtion. It sends strings when called
    private void SendData(string message)
    {
        try
        {
            Debug.Log("SERVER Sending message to " + clientEP.ToString() + ": " + message);
            data = Encoding.ASCII.GetBytes(message);
            udpSocket.SendTo(data, data.Length, SocketFlags.None, clientEP);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data. Error: " + e.Message);
        }
    }
}
