using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string message;
    public string username;

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
