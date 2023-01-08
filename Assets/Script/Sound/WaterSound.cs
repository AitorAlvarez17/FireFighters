using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSound : MonoBehaviour
{

    public AudioClip FX;
    AudioSource audioSource;
    bool entered;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        entered = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        // Debug.Log("other" + other);
        if (other.transform.tag == "Lumber" && entered == false)
        {
            audioSource.PlayOneShot(FX);
            entered = true;

        }


    }

    public void OnTriggerExit(Collider other)
    {
        entered = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
