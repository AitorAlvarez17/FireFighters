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
    // Clients'IP and port
    private string sIp;
    private int sPort;

    //Data matrix and number of bytes
    private byte[] data = new byte[1024];
    private int recv;

    //declare Client's endpoint
    private IPEndPoint clientIPEP;

    //declare thread and socket
    private Socket socket;
    private Thread thread;

    // Message decoded for rendering on screen
    public string messageDecoded = null;
    public bool open = false;

    //instanciation both variables and starts server
    void Start()
    {
        sIp = ServerController.MyServerInstance.IPServer;
        sPort = ServerController.MyServerInstance.serverPort;

        StartServer();
    }

    //closing both the socket and the thread on exit and all coroutines
    private void OnDisable()
    {
        Debug.Log("SERVER Closing TCP socket & thread...");

        if (socket != null)
            socket.Close();
        if (thread != null)
            thread.Abort();

        StopAllCoroutines();
    }

    //Initialize socket and thread
    public void StartServer()
    {
        open = true;
        InitSocket();
        InitThread();
    }

    //set socket
    private void InitThread()
    {
        thread = new Thread(new ThreadStart(ServerThread));
        thread.IsBackground = true;
        thread.Start();
    }

    //set and initialize thread
    private void InitSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    //Main thread 
    private void ServerThread()
    {

        IPAddress ipAddress = IPAddress.Parse(sIp);
        clientIPEP = new IPEndPoint(ipAddress, sPort);

        //try the socket's bind, if not debugs
        try
        {
            socket.Bind(clientIPEP);
            Debug.Log("SERVER TCP socket bound to " + clientIPEP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to bind socket: " + e.Message);
        }

        //try the socket's bind, if not debugs
        socket.Listen(10);
        Socket clientSocket = socket.Accept();

        //loops the receive system. Messy but functional
        while (open) // Look at Promises, Async, Await
        {
            Debug.Log("Server looping into the Listen While Loop");
            try
            {
                //RECEIVE DATA
                recv = clientSocket.Receive(data);
                Debug.Log("SERVER Client Message: " + Encoding.Default.GetString(data, 0, recv));
                messageDecoded = Encoding.Default.GetString(data, 0, recv);
            }
            catch (Exception e)
            {
                Debug.Log("SERVER ERROR Failed to receive data: " + e.Message);
            }

            SendString(clientSocket ,"Hey, I have received your message");

        }
    }

    //Main communication funtion. It sends strings when called
    private void SendString(Socket socket, string message)
    {
        try
        {
            Debug.Log("SERVER Sending message to client: " + message);
            data = Encoding.Default.GetBytes(message);
            socket.Send(data);
        }
        catch (Exception e)
        {
            Debug.Log("SERVER ERROR Failed to send data: " + e.Message);
        }
    }

    
}
