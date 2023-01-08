using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LookAtCamera : MonoBehaviour
{
    public Transform cameraTrans;
    public TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        text = this.transform.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        this.transform.rotation = Quaternion.RotateTowards(text.transform.rotation, rotation, 360f);
    }
}
