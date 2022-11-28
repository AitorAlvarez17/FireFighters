using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Three : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter()
    {

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