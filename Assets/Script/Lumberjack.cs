using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lumberjack : MonoBehaviour
{
    // Start is called before the first frame update
    public int internalId;
    public Transform trans;
    public TextMeshPro textInfo;
    public string Username;

    public Lumberjack()
    {

    }

    public void Init(int _id, string username)
    {
        SetUsername(username);
        SetId(_id);
    }
    public void SetId(int _id)
    {
        internalId = _id;
        textInfo.text += "Key ID:" + _id + "\n";
    }
    public void SetUsername(string username)
    {
        Username = username;
        textInfo.text += "Name: " + username + "\n"; 
    }

    void Start()
    {
        trans = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(float[] _positions)
    {
        Debug.Log("Moving Doll" + internalId + "to:" + _positions[0] + _positions[2]);
        trans.position = new Vector3(_positions[0], this.gameObject.transform.position.y, _positions[2]);
    }
}
