using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSawmill
{
    public Lumberjack lumberjack;
    public Fireplace firePlace;
    public GameObject movementPredicter;

    public PlayerSawmill(Lumberjack  _lumberjack, Fireplace _firePlace, GameObject _predictioner)
    {
        lumberjack = _lumberjack;
        firePlace = _firePlace;
        movementPredicter = _predictioner;
    }
}

public class WorldController : MonoBehaviour
{
    public GameObject playerGO;
    public GameObject fireGO;
    public GameObject predictGO;

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

    public void CreatePlayer(int key, bool reckoning)
    {
        //Lumberjack
        Debug.Log("New Lumberjack! KEY:" + key);
        GameObject playerPref = Instantiate(playerGO, spawnPoints[pos].position, Quaternion.identity);
        playerPref.GetComponent<Lumberjack>().Init(key, reckoning);
        playerPref.transform.localScale = new Vector3(1.88f, 1.88f, 1.88f);

        GameObject predictPref = Instantiate(predictGO, spawnPoints[pos].position, Quaternion.identity);
        //playerGO.GetComponent<Lumberjack>().Init(key);

        //Create fireplace
        GameObject firePref = Instantiate(fireGO, firePoints[key - 1].position, Quaternion.identity);
        firePref.GetComponent<Fireplace>().Init(key, "Fire" + key.ToString());


        pos++;
        worldDolls.Add(key, new PlayerSawmill(playerPref.GetComponent<Lumberjack>(), firePref.GetComponent<Fireplace>(), predictPref));
    }

    public void CreatePlayer(int key, bool reckoning, bool interacter = false)
    {

        //Lumberjack
        Debug.Log("New Lumberjack! KEY:" + key);
        GameObject playerPref = Instantiate(playerGO, spawnPoints[pos].position, Quaternion.identity);
        playerPref.GetComponent<Lumberjack>().Init(key, reckoning, interacter);
        playerPref.transform.localScale = new Vector3(1.88f, 1.88f, 1.88f);
        Vector3 spawnOrientation = new Vector3(0f, 90 * (worldDolls.Count + 1) + 45, 0f);

        GameObject predictPref = Instantiate(predictGO, spawnPoints[pos].position, Quaternion.identity);

        playerPref.transform.Rotate(spawnOrientation, Space.World);
        //playerGO.GetComponent<Lumberjack>().Init(key);

        //Create fireplace
        GameObject firePref = Instantiate(fireGO, firePoints[key - 1].position, Quaternion.identity);
        firePref.GetComponent<Fireplace>().Init(key, "Fire" + key.ToString());


        pos++;
        worldDolls.Add(key, new PlayerSawmill(playerPref.GetComponent<Lumberjack>(), firePref.GetComponent<Fireplace>(), predictPref));

    }

    public void SetReckoningRTTS(float _newRtt)
    {
        foreach (PlayerSawmill item in worldDolls.Values)
        {
            item.lumberjack.rtt = _newRtt;
        }
    }
    public void DeletePlayer(int key)
    {
        GameObject.Destroy(worldDolls[key].lumberjack.transform.gameObject);
        GameObject.Destroy(worldDolls[key].firePlace.transform.gameObject);
        worldDolls.Remove(key);
    }

    public void MovePlayer(int _key, float[] _positions, float[] _directions, float IP)
    {
        if (worldDolls.ContainsKey(_key))
            worldDolls[_key].lumberjack.Move(_positions, new Vector3(_directions[0], _directions[1], _directions[2]), IP);
        else
            Debug.Log("Key" + _key + "was not supported!");
    }

    public void SetFireLife(int _key, int life)
    {
        worldDolls[_key].firePlace.SetLife(life);
    }

    public void UpdateFires(Tuple<int, int>[] gameMatrix)
    {
        foreach (var item in gameMatrix)
        {
            if (item.Item1 != 0)
            {
                Debug.Log("Updating FIRE " + item.Item1 + "");
                if (worldDolls.ContainsKey(item.Item1))
                    worldDolls[item.Item1].firePlace.SetLife(item.Item2);
                else
                    Debug.Log("Doll was not in the dictionary");
            }
            else
            {
                Debug.Log("Item was 0");
            }
        }
    }

    public void WelcomeClient(Tuple<int, int>[] worldMatrix, int _key)
    {

        foreach (var index in worldMatrix)
        {
            Debug.Log("Representing client with key" + index.Item1);

            if (worldDolls.ContainsKey(index.Item1))
            {
                Debug.Log("Doll with" + index.Item1 + "already exists");
                continue;
            }
            if (index.Item1 != 0 && index.Item1 != _key)
            {
                //Dead Reckoning only hides latency for REMOTE users
                Debug.Log("Creating doll with key" + index.Item1);
                CreatePlayer(index.Item1, true);
            }
            if (index.Item1 == _key)
            {
                CreatePlayer(_key, false, true);
                this.gameObject.GetComponent<PlayerMovement>().player = worldDolls[_key].movementPredicter.gameObject;
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().target = worldDolls[_key].lumberjack.transform;
            }
        }
    }
}
