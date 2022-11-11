using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Player
{
    public string username;
    public bool onLine;
    public int id;
    public bool dirty;
    public float[] positions = new float[3] { 0, 0, 0 };
    public Player(string _username, bool onLine, int _id)
    {
        this.username = _username;
        this.onLine = onLine;
        this.id = _id;
        this.dirty = false;
        
    }
}

public static class PlayerManager
{
    [SerializeField]
    public static Dictionary<int, Player> Players = new Dictionary<int, Player>();

    private static bool playersDirty;

    public static bool serverDirty;
    public static bool playerDirty;

    public static int playersOnline = 0;


    // Start is called before the first frame update
    
    public static Player AddPlayer(string name)
    {
        playersOnline++;
        Player newPlayer = new Player(name + playersOnline.ToString(), true, playersOnline);
        Debug.Log("New Player Added!");
        newPlayer.id = playersOnline;
        Players.Add(playersOnline, newPlayer);
        

        return newPlayer;
    }


    public static void JoinPlayer(string username)
    {
        playersDirty = true;
        playerDirty = true;
    }


}
