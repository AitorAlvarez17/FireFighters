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
    public Material[] hats;
    public Material[] shirts;
    public Material[] hair;

    //visuals
    public GameObject avatar;

    public Lumberjack()
    {

    }

    public void Init(int _id, string username)
    {
        SetUsername(username);
        SetId(_id);
        SetOutfit(_id);
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

    public void SetOutfit(int id)
    {
        Material[] mats = avatar.GetComponent<Renderer>().materials;
        switch (id)
        {
            case 2:
                mats[0] = hats[0];
                mats[2] = hair[0];
                mats[3] = shirts[0];
                break;
            case 3:
                mats[0] = hats[1];
                mats[2] = hair[1];
                mats[3] = shirts[1];
                break;
            case 4:
                mats[0] = hats[2];
                mats[2] = hair[2];
                mats[3] = shirts[2];
                break;
            default:
                Debug.Log("No id supported");
                break;
        }
        avatar.GetComponent<Renderer>().materials = mats;
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
