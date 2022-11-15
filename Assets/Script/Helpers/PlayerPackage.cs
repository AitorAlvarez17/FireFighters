using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPackage
{
    public string message;
    public string username;

    public float[] positions = new float[3];

    public int id = -1;
    //Message constructor
    public PlayerPackage(string _message, string _username)
    {
        message = _message;
        username = _username;
        positions[0] = 0f;
        positions[1] = 0f;
        positions[2] = 0f;
    }   

    //position constructor
    public PlayerPackage(string _message, string _username,float[] position, int _id)
    {
        message = _message;
        username = _username;
        positions[0] = position[0];
        positions[1] = position[1];
        positions[2] = position[2];
        id = _id;
    }

    public void SetMessage(string _message)
    {
        message = _message;
    }

    public void SetUsername(string _username)
    {
        username = _username;
    }

    public void SetPositions(float[] _positions)
    {
        positions[0] = _positions[0];
        positions[1] = _positions[1];
        positions[2] = _positions[2];
    }

    public void SetId(int _id)
    {
        id = _id;
    }
}
