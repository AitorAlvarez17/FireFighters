using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPackage
{
    public string message;
    public string username;

    public float[] positions = new float[3];

    public int id = -1;

    public int[] worldMatrix = new int[4];

    //this will work as an index
    public int playersOnline = 0;
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
    public PlayerPackage(string _message, string _username,float[] position, int _id, int[] _worldMatrix, int _playersOnline)
    {
        message = _message;
        username = _username;
        positions[0] = position[0];
        positions[1] = position[1];
        positions[2] = position[2];
        id = _id;
        worldMatrix[0] = _worldMatrix[0];
        worldMatrix[1] = _worldMatrix[1];
        worldMatrix[2] = _worldMatrix[2];
        worldMatrix[3] = _worldMatrix[3];
        playersOnline = _playersOnline;
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

    public void SetWorldMatrixPos(int pos, int value)
    {
        worldMatrix.SetValue(value, pos);
    }

    public void SetWorldMatrix(int[] _worldMatrix)
    {
        worldMatrix = _worldMatrix;
    }

    public void SetPlayersOnline(int _value)
    {
        playersOnline = _value;
    }
}