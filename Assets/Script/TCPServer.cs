using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private string sIp;
    private int sPort;

    private byte[] data = new byte[1024];
    private int recv;

    private IPEndPoint clientIPEP;
    private EndPoint clientEP;

    private Socket socket;
    private Thread thread;
  



    void Start()
    {
        sIp = ServerController.MyServerInstance.IPServer;
        sPort = ServerController.MyServerInstance.serverPort;

        StartServer();
    }

    private void OnDisable()
    {
        if (socket != null)
            socket.Close();
        if (thread != null)
            thread.Abort();

        StopAllCoroutines();
    }

    public void StartServer()
    {
        InitSocket();
        InitThread();
    }
    private void InitThread()
    {
        thread = new Thread(new ThreadStart(ServerThread));
        thread.IsBackground = true;
        thread.Start();
    }
    private void InitSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    private void ServerThread()
    {
        IPAddress ipAddress = IPAddress.Parse(sIp);
        clientIPEP = new IPEndPoint(ipAddress, sPort);
        clientEP = (EndPoint)clientIPEP;
        
        try
        {
            socket.Bind(clientIPEP);
            Debug.Log("[SERVER] TCP socket bound to " + clientIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to bind socket: " + e.Message);
        }
        
        socket.Listen(10);
        while (true) // Look at Promises, Async, Await
        {
            Socket clientSocket = socket.Accept();
            try
            {
                recv = clientSocket.Receive(data);
                Debug.Log("SERVER Client Message: " + Encoding.Default.GetString(data, 0, recv));
            }
            catch (Exception e)
            {
                Debug.Log("SERVER ERROR Failed to receive data: " + e.Message);
            }

            SendData(clientSocket ,"Heyyy client, I have received your message");

            clientSocket.Close();
        }
    }
    
    private void SendData(Socket socket, string message)
    {
        try
        {
            Debug.Log("[SERVER] Sending message to client: " + message);
            data = Encoding.Default.GetBytes(message);
            socket.Send(data);
        }
        catch (Exception e)
        {
            Debug.Log("[ERROR SERVER] Failed to send data: " + e.Message);
        }
    }
}
