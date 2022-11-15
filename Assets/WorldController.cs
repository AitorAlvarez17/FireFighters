using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public GameObject playerGO;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePlayer()
    {
        Instantiate(playerGO, Vector3.zero, Quaternion.identity, this.gameObject.transform);
    }
}
