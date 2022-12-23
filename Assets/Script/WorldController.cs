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
        Vector3 spawnOrientation = new Vector3(0f, 90 * (worldDolls.Count + 1) + 45, 0f);

        playerPref.transform.Rotate(spawnOrientation, Space.World);

        //Create fireplace
        GameObject firePref = Instantiate(fireGO, firePoints[key - 1].position, Quaternion.identity);
        firePref.GetComponent<Fireplace>().Init(key, "Fire" + key.ToString());


        pos++;
        worldDolls.Add(key, new PlayerSawmill(playerPref.GetComponent<Lumberjack>(), firePref.GetComponent<Fireplace>()));

    }

    public void DeletePlayer(int key)
    {
        GameObject.Destroy(worldDolls[key].lumberjack.transform.gameObject);
        GameObject.Destroy(worldDolls[key].firePlace.transform.gameObject);
        worldDolls.Remove(key);
    }

    public void MovePlayer(int _key, float[] _positions, float[] _directions)
    {
        if (worldDolls.ContainsKey(_key))
            worldDolls[_key].lumberjack.Move(_positions, new Vector3(_directions[0], _directions[1], _directions[2]));
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
                worldDolls[item.Item1].firePlace.SetLife(item.Item2);
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
                Debug.Log("Creating doll with key" + index.Item1);
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
