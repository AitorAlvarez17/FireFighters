using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;

public class ClientScript : MonoBehaviour
{
    string IpEndAdress;
    int port = 88;

    //adress where we are going to connect
    IPEndPoint ipep;


    // Start is called before the first frame update
    void Start()
    {
        ipep = new IPEndPoint(IPAddress.Any, port);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
