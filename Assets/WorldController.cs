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
        Debug.Log("New Lumberjack! KEY:" + key);
        GameObject playerPref = Instantiate(playerGO, spawnPoints[pos].position, Quaternion.identity);
        playerPref.GetComponent<Lumberjack>().Init(key);
        playerPref.transform.localScale = new Vector3(1.88f, 1.88f, 1.88f);
        worldDolls.Add(key, playerPref.GetComponent<Lumberjack>());
        pos++;
        //playerGO.GetComponent<Lumberjack>().Init(key);
    }

    public void MovePlayer(int _key, float[] _positions)
    {
        if (worldDolls.ContainsKey(_key))
            worldDolls[_key].Move(_positions);
        else
            Debug.Log("Key" + _key + "was not supported!");
    }

    public void WelcomeClient(int[] worldMatrix, int _key)
    {
        foreach (int index in worldMatrix)
        {
            if (index != 0 && index != _key)
            {
                CreatePlayer(index);
            }
        }
    }
}
