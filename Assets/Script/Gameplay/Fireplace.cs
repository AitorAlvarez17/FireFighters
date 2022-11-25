using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : MonoBehaviour
{
    public int internalID;
    public string fireName;

    public Fireplace()
    {

    }

    public void Init(int _key, string _name)
    {
        internalID = _key;
        fireName = _name;
    }

    public float life;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
