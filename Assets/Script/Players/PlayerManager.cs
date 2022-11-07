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

    public Player(string _username, bool onLine)
    {
        this.username = _username;
        this.onLine = onLine;
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
    
    public static void AddPlayer(Player newPlayer)
    {
        playersOnline++;
        Players.Add(playersOnline, newPlayer);
        serverDirty = true;
    }
    public static void JoinPlayer(string username)
    {
        playersDirty = true;
        playerDirty = true;

    }


}
