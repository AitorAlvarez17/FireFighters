using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Three : MonoBehaviour
{
    public GameObject GC;
    public int life;
    public int phase;
    private bool recolected;
    public AudioClip FX;
    AudioSource audioSource;


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

    bool entered;

    private void Awake()
    {
        life = 100;
        phase = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        GC = GameObject.FindGameObjectWithTag("GameController");
        audioSource = gameObject.GetComponent<AudioSource>();
        entered = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        // Debug.Log("other" + other);
        if(other.transform.tag == "Lumber" && entered==false)
        {
            audioSource.PlayOneShot(FX);
            entered = true;

        }


    }

    public void OnTriggerExit(Collider other)
    {
        entered = false;
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
