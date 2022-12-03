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

    public void CreatePlayer(int key)
    {
        //Lumberjack
        Debug.Log("New Lumberjack! KEY:" + key);
        GameObject playerPref = Instantiate(playerGO, spawnPoints[pos].position, Quaternion.identity);
        playerPref.GetComponent<Lumberjack>().Init(key);
        playerPref.transform.localScale = new Vector3(1.88f, 1.88f, 1.88f);
        
        //playerGO.GetComponent<Lumberjack>().Init(key);

        //Create fireplace
        GameObject firePref = Instantiate(fireGO, firePoints[key - 1].position, Quaternion.identity);
        firePref.GetComponent<Fireplace>().Init(key, "Fire" + key.ToString());


        pos++;
        worldDolls.Add(key, new PlayerSawmill(playerPref.GetComponent<Lumberjack>(), firePref.GetComponent<Fireplace>()));

    }

    public void CreatePlayer(int key, bool interacter = false)
    {

        //Lumberjack
        Debug.Log("New Lumberjack! KEY:" + key);
        GameObject playerPref = Instantiate(playerGO, spawnPoints[pos].position, Quaternion.identity);
        playerPref.GetComponent<Lumberjack>().Init(key, interacter);
        playerPref.transform.localScale = new Vector3(1.88f, 1.88f, 1.88f);

        //playerGO.GetComponent<Lumberjack>().Init(key);

        //Create fireplace
        GameObject firePref = Instantiate(fireGO, firePoints[key - 1].position, Quaternion.identity);
        firePref.GetComponent<Fireplace>().Init(key, "Fire" + key.ToString());


        pos++;
        worldDolls.Add(key, new PlayerSawmill(playerPref.GetComponent<Lumberjack>(), firePref.GetComponent<Fireplace>()));

    }

    public void MovePlayer(int _key, float[] _positions)
    {
        if (worldDolls.ContainsKey(_key))
            worldDolls[_key].lumberjack.Move(_positions);
        else
            Debug.Log("Key" + _key + "was not supported!");
    }

    public void UpdateFire(int _key, int type, int amount)
    {
        worldDolls[_key].firePlace.HealBar(type, amount);
    }

    public void WelcomeClient(Tuple<int, int>[] worldMatrix, int _key)
    {
        foreach (var index in worldMatrix)
        {
            if (worldDolls.ContainsKey(index.Item1))
                return;
            
            if (index.Item1 != 0 && index.Item1 != _key)
            {
                CreatePlayer(index.Item1);
            }
            if (index.Item1 == _key)
            {
                CreatePlayer(_key, true);
                this.gameObject.GetComponent<PlayerMovement>().player = worldDolls[_key].lumberjack.gameObject;
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().target = worldDolls[_key].lumberjack.transform;
            }
        }
    }
}
