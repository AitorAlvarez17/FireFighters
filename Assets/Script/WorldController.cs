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

    public void SetFireLife(int _key, int life)
    {
        worldDolls[_key].firePlace.SetLife(life);
    }

    public void UpdateFires(Tuple<int, int>[] gameMatrix)
    {
        foreach (var item in gameMatrix)
        {
            Debug.Log("Updating fire" + item.Item1);

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
        Debug.Log("Element 1 in matrix:" + worldMatrix[0].Item1);
        Debug.Log("Element 2 in matrix:" + worldMatrix[1].Item1);
        Debug.Log("Element 3 in matrix:" + worldMatrix[2].Item1);
        Debug.Log("Element 4 in matrix:" + worldMatrix[3].Item1);

        foreach (var index in worldMatrix)
        {
            Debug.Log("Representing client with key" + index.Item1);

            if (worldDolls.ContainsKey(index.Item1))
            {
                Debug.Log("Doll with" + index.Item1 + "already exists");
                return;
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
