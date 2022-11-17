using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public GameObject playerGO;

    public List<Transform> spawnPoints = new List<Transform>();

    public Dictionary<int, Lumberjack> worldDolls = new Dictionary<int, Lumberjack>();

    int pos = 0;
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
        GameObject playerPref = Instantiate(playerGO, spawnPoints[key].position, Quaternion.identity);
        playerPref.GetComponent<Lumberjack>().Init(key);
        worldDolls.Add(key, playerPref.GetComponent<Lumberjack>());
        pos = key;
        pos++;
        //playerGO.GetComponent<Lumberjack>().Init(key);
    }

    public void MovePlayer(int _key, float[] _positions)
    {
        worldDolls[_key].Move(_positions);
    }

    public void WelcomeClient(int _key)
    {
        Debug.Log("PlayersBrainDictionary Count" + PlayerManager.PlayersBrainDictionary.Values.Count);
        
        for (int i = 1; i < PlayerManager.playersOnline; i++)
        {
            if (i != _key)
            {
                CreatePlayer(i);
            }
        }
    }
}
