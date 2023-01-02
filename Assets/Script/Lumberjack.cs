using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charge
{
    private int amount;

    //0 for nothing, 1 for wood, 2 for water
    private int type;


    public Charge(int _amount, int _type)
    {
        amount = _amount;
        type = _type;
    }

    public int Amount   
    {
        get { return amount; }   
        set { amount = value; }  
    }

    public int Type
    {
        get { return type; }
        set { type = value; }
    }

    public void SumWood(int _amount)
    {
        if (type != 1)
            amount = 0;

        type = 1;
        amount += _amount;
    }

    public void SumWater(int _amount)
    {
        if (type != 2)
            amount = 0;

        type = 2;
        amount += _amount;
    }

    public void ClearCharge()
    {
        type = 0;
        amount = 0;
    }
}
public class Lumberjack : MonoBehaviour
{
    // Start is called before the first frame update
    public int internalId;
    public Transform trans;
    public TextMeshPro textInfo;
    public TextMeshPro resDebug;
    public string Username;
    public Material[] hats;
    public Material[] shirts;
    public Material[] hair;

    public bool isMoving = false; 
    public bool isPredictingMovement = false;
    public bool isCorrectingMov = false;

    public bool interacter = false;
    public float deltaTime = 0f;

    //0 for nothing, 1 for wood, 2 for water
    public Charge charge = null;
    //visuals
    public GameObject avatar;

    public Lumberjack()
    {

    }

    public void Init(int _id, bool interacter = false)
    {
        SetUsername("Player" + _id);
        SetId(_id);
        SetOutfit(_id);
        SetInteracter(interacter);
        charge = new Charge(0, 0);
        PrintDebug();
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

    public void SetInteracter(bool value)
    {
        Debug.Log("Interacter set to: " + value);
        interacter = value;
    }

    public void PrintDebug()
    {
        switch (charge.Type)
        {
            case 0:
                resDebug.text = "Empty!";
                resDebug.color = Color.grey;
                break;
            case 1:
                Debug.Log("Printing WOOD");
                resDebug.text = "Wood: " + charge.Amount;
                resDebug.color = Color.red;
                break;
            case 2:
                resDebug.text = "Water: " + charge.Amount;
                resDebug.color = Color.blue;
                break;
            default:
                break;
        }
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
        deltaTime = Time.deltaTime;
    }

    public void Move(float[] _positions, Vector3 _directions, float IP)
    {
        float timeElapsed = 0f;
        //Vector3 newPosition = new Vector3(_positions[0], _positions[1], _positions[2]);


        SmoothRotation(_directions, IP);
        //Debug.Log("Moving Doll" + internalId + "to:" + _positions[0] + _positions[2]);
        //IP HAS TO BE SO SIMILAR TO PP

        Debug.Log("Moving");
        StartCoroutine(Lerp(_positions, _directions, IP));

        
        

        //trans.position = new Vector3(_positions[0], trans.position.y, _positions[2]);

        //if(prediction.isWrong)
        //CorrectMovement();

        //MovementPrediction();
    }

    IEnumerator Lerp(float[] _positions, Vector3 _directions, float IP)
    {
        Vector3 transBuffer = trans.transform.position;
        Vector3 newPositions = new Vector3(_positions[0], _positions[1], _positions[2]);

        float timeElapsed = 0;
        while (timeElapsed < IP)
        {
            trans.position = Vector3.Lerp(transBuffer, newPositions, timeElapsed / IP);
            timeElapsed += Time.deltaTime;
        }
        //Debug.Log("Time elapsed:" + timeElapsed + " s");

        trans.position = newPositions;

        yield break;
    }

    public void MovementPrediction()
    {
        //take quaternion direction and go through it
    }

    public void MovementCorrection()
    {
        //correct the movement 
    }

    public void SmoothRotation(Vector3 directions, float IP)
    {
        //Debug.Log("Rotating" + directions);
        Quaternion rotation = Quaternion.LookRotation(directions, Vector3.up);
        trans.rotation = Quaternion.RotateTowards(trans.rotation, rotation, 360f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (interacter == false)
            return;

        Debug.Log("Lumberjack Entered the Trigger");
        if (other.tag == "Water")
        {
            charge.SumWater(5);
            
        }
        else if(other.tag == "Three")
        {
            charge.SumWood(5);
        }

        PrintDebug();
    }
}
