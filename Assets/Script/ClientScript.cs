using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System;
using UnityEngine;
using TCP;

public class ClientScript : MonoBehaviour
{
    string IpEndAdress;
    int port = 88;

    //adress where we are going to connect
    IPEndPoint ipep;

    IPEndPoint ipep2;

    TCPSocket c = new TCPSocket();

    // Start is called before the first frame update
    void Start()
    {
        c.Client("127.0.0.1", 27000);
        c.Send("TEST!");

        Console.WriteLine("Closed Server \n Press any key to exit");
        Console.ReadKey();
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

    
}
