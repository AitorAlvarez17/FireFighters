using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : MonoBehaviour
{
    public GameObject GC;
    public int internalID;
    public string fireName;

    public float life;

    public Fireplace()
    {

    }

    public void Init(int _key, string _name)
    {
        GC = GameObject.FindGameObjectWithTag("GameController");
        internalID = _key;
        fireName = _name;
        life = 100f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Heal(int _amount)
    {
        life += _amount;
        //Ping Life();
    }

    public void OnTriggerEnter(Collider other)
    {
        //get the action from the lumberjack and put it into it
        if (GC.GetComponent<UDPClient>() != null)
        {
            GC.GetComponent<UDPClient>().PingFireAction(0, 1);

        }
        else if (GC.GetComponent<UDPServer>() != null)
        {
            GC.GetComponent<UDPServer>().PingFireAction(0, 1);
        }
        //PingFireAction(int action, int amount);

    }


}
