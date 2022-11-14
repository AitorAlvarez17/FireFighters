using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public Message message;
    public float[3] positions;

    public PlayerInfo(Message _message, float[3] _positions)
    {
        message = _message;
        new float[3] positions = _positions;
    }
}


public class Message
{
    public string message;
    public string username;
    public string header;

    public Message(string _header, string _message, string _username)
    {
        header = _header;
        message = _message;
        username = _username;
    }

    public Message(string _message, string _username)
    {
        message = _message;
        username = _username;
    }

   
    public void SetMessage(string _message)
    {
        message = _message;
    }

    public void SetUsername(string _username)
    {
        username = _username;
    }
}

