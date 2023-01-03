using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPackage
{
    public string message;
    public string username;

    public float[] positions = new float[3];
    public float[] movementDirection = new float[3];
    public float velocity = 0f;

    public int id = -1;

    //int of the 4 players
    public Tuple<int, int>[] worldMatrix = new Tuple<int, int>[4];

    //this will work as an index
    public int playersOnline = 0;

    public int fireAction = 0;
    public int amount = 0;
    public int fireID = 0;
    public int fireLife = 0;

    public int[] fireLifeMatrix = new int[4];

    public float timeStamp = 0f;

    public bool gameStarted = false;
    //Message constructor
    public PlayerPackage(string _message, string _username)
    {
        message = _message;
        username = _username;
        positions[0] = 0f;
        positions[1] = 0f;
        positions[2] = 0f;
        //timeStamp needed
    }   

    //position constructor
    public PlayerPackage(string _message, string _username,float[] position, float[] _movementDirection, int _id, Tuple<int, int>[] _worldMatrix, int _playersOnline, int _fireAction, int _amount, int _fireId, int _fireLife, float _timeStamp, bool _gameStarted, float _vel)
    {
        message = _message;
        username = _username;
        positions[0] = position[0];
        positions[1] = position[1];
        positions[2] = position[2];

        movementDirection[0] = _movementDirection[0];
        movementDirection[1] = _movementDirection[1];
        movementDirection[2] = _movementDirection[2];

        velocity = _vel;
        id = _id;
        worldMatrix[0] = _worldMatrix[0];
        worldMatrix[1] = _worldMatrix[1];
        worldMatrix[2] = _worldMatrix[2];
        worldMatrix[3] = _worldMatrix[3];
        playersOnline = _playersOnline;
        fireAction = _fireAction;
        amount = _amount;
        fireID = _fireId;
        fireLife = _fireLife;
        timeStamp = _timeStamp;
        gameStarted = _gameStarted;
    }

    //Empty message only with identification info 
    public PlayerPackage(string _username, int _id, Tuple<int, int>[] _matrix)
    {
        message = "";
        username = _username;
        positions[0] = 0f;
        positions[1] = 0f;
        positions[2] = 0f;

        movementDirection[0] = 0f;
        movementDirection[1] = 0f;
        movementDirection[2] = 0f;

        velocity = 0f;
        id = _id;
        worldMatrix[0] = _matrix[0];
        worldMatrix[1] = _matrix[1];
        worldMatrix[2] = _matrix[2];
        worldMatrix[3] = _matrix[3];
        playersOnline = 0;
        fireAction = 0;
        amount = 0;
        fireID = _id;
        fireLife = 0;
        timeStamp = 0;
        gameStarted = false;
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

    public void SetFireAction(int _id, int _action, int _amount, int _life)
    {
        fireAction = _action;
        amount = _amount;
        fireID = _id;
        fireLife = _life;
    }

    public void ClearCharge()
    {
        amount = 0;
    }

    public void SetWorldMatrixPos(int pos, int value)
    {
        worldMatrix.SetValue(value, pos);
    }

    public void SetWorldMatrix(Tuple<int, int>[] _worldMatrix)
    {
        worldMatrix = _worldMatrix;
    }

    public void SetPlayersOnline(int _value)
    {
        playersOnline = _value;
    }

    public void SetTimeStamp(float _timeStamp)
    {
        timeStamp = _timeStamp;
    }

    public void SetVelocity(float _vel)
    {
        velocity = _vel;
    }
    public void SetDirection(float[] _dir)
    {
        movementDirection[0] = _dir[0];
        movementDirection[1] = _dir[1];
        movementDirection[2] = _dir[2];
    }

    public void SetGameState(bool _gameStarted)
    {
        Debug.Log("Setting game state to" + _gameStarted);
        gameStarted = _gameStarted;
    }

}
