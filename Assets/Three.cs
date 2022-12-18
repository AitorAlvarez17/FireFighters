using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Three : MonoBehaviour
{
    public GameObject GC;
    public int life;
    public int phase;
    private bool recolected;

    public bool Recolected
    {
        get
        {
            return recolected;
        }
        set
        {
            if (value != recolected)
            {
                life -= Damage();
            }
            recolected = value;
        }
    }

    private void Awake()
    {
        life = 100;
        phase = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        GC = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
       // Debug.Log("other" + other);

        if (other.transform.GetComponent<Lumberjack>() == null)
            return;

        GC.GetComponent<WorldController>().worldDolls[other.transform.GetComponent<Lumberjack>().internalId].lumberjack.charge.SumWood(5);
        GC.GetComponent<WorldController>().worldDolls[other.transform.GetComponent<Lumberjack>().internalId].lumberjack.PrintDebug();
    }

    public int Damage()
    {
        NotifyPhase(0);
        return 0;
    }

    public bool NotifyPhase(int damageTaken)
    {
        //change phase
        return true;
    }
}
