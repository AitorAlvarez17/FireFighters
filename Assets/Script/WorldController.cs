using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSawmill
{
    public Lumberjack lumberjack;
    public Fireplace firePlace;

    public PlayerSawmill(Lumberjack  _lumberjack, Fireplace _firePlace)
    {
        lumberjack = _lumberjack;
        firePlace = _firePlace;
    }
}

public class WorldController : MonoBehaviour
{

    public GameObject playerGO;
    public GameObject fireGO;

    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> firePoints = new List<Transform>();

    public Dictionary<int, PlayerSawmill> worldDolls = new Dictionary<int, PlayerSawmill>();


    int pos = 0;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePlayer(int key, string username)
    {
        //Lumberjack
        Debug.Log("New Lumberjack! KEY:" + key);
        GameObject playerPref = Instantiate(playerGO, spawnPoints[pos].position, Quaternion.identity);
        playerPref.GetComponent<Lumberjack>().Init(key, username);
        playerPref.transform.localScale = new Vector3(1.88f, 1.88f, 1.88f);

        worldDolls.Add(key, playerPref.GetComponent<Lumberjack>());
        pos++;
        //playerGO.GetComponent<Lumberjack>().Init(key);

        //Create fireplace
        GameObject firePref = Instantiate(fireGO, firePoints[key - 1].position, Quaternion.identity);
        firePref.GetComponent<Fireplace>().Init(key, "Fire" + key.ToString());



        worldDolls.Add(key, new PlayerSawmill(playerPref.GetComponent<Lumberjack>(), firePref.GetComponent<Fireplace>()));

    }

    public void MovePlayer(int _key, float[] _positions)
    {
        if (worldDolls.ContainsKey(_key))
            worldDolls[_key].lumberjack.Move(_positions);
        else
            Debug.Log("Key" + _key + "was not supported!");
    }

    public void HealFire(int _key, int amount)
    {
        worldDolls[_key].firePlace.Heal(amount);
    }

    public void WelcomeClient(int[] worldMatrix, int _key)
    {
        foreach (int index in worldMatrix)
        {
            if (index != 0 && index != _key)
            {
                CreatePlayer(index, username);
            }
        }
    }
}
