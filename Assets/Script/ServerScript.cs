using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System;
using UnityEngine;
using TCP;

public class ServerScript : MonoBehaviour
{
    TCPSocket s = new TCPSocket();
    // Start is called before the first frame update
    void Start()
    {
        s.Server("127.0.0.1", 27000);
        s.Receive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        s._socket.Close();
    }
}
