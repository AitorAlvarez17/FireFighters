using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerManager : MonoBehaviour
{

    [Serializable]
    public class Player
    {
        public string username;
        public bool onLine;
    }

    [SerializeField]
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();

    private bool playersDirty;

    public bool serverDirty;
    public bool playerDirty;

    private int playersOnline = 1;


    // Start is called before the first frame update
    void Start()
    {
        AddPlayer(playersOnline);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(int playersOnline)
    {
        //Players.Add(playersOnline, )
    }
    public void JoinPlayer(string username)
    {
        playersDirty = true;
        playerDirty = true;

    }
}
